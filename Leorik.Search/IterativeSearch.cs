using Leorik.Core;
using System.Runtime.CompilerServices;
using static Leorik.Core.Evaluation;

namespace Leorik.Search
{
    public class IterativeSearch : ISearch
    {
        public const int MAX_PLY = 99;
        private const int MIN_ALPHA = -CheckmateScore;
        private const int MAX_BETA = CheckmateScore;
        private const int MAX_MOVES = 225;
        //https://www.stmintz.com/ccc/index.php?id=425058
        private const int ASPIRATION_WINDOW = 40;

        private readonly BoardState[] Positions;
        private readonly Move[] Moves;
        public readonly Move[] RootMoves;
        private readonly Move[] PrincipalVariations;
        private readonly int[] RootMoveOffsets;
        private readonly History _history;
        private readonly StaticExchange _see = new();
        private readonly ulong[] _legacy; //hashes of positions that we need to eval as repetitions
        private readonly SearchOptions _options;
        private KillSwitch _killSwitch;
        public long NodesVisited { get; private set; }
        public int Depth { get; private set; }
        public int Score { get; private set; }
        public bool Aborted { get; private set; }
        public Span<Move> PrincipalVariation => GetFirstPVfromBuffer(PrincipalVariations, Depth);

        public IterativeSearch(BoardState board, SearchOptions options, ulong[]? history, Move[]? moves)
        {
            _options = options;
            _history = new History();
            _legacy = history ?? Array.Empty<ulong>();
            Moves = new Move[MAX_PLY * MAX_MOVES];
            MoveGen moveGen = new(Moves, 0);
            if (moves?.Length > 0)
            {
                RootMoves = moves;
            }
            else
            {
                moveGen.Collect(board);
                RootMoves = new Move[moveGen.Next];
                Array.Copy(Moves, RootMoves, RootMoves.Length);
            }
            //PV-length = depth + (depth - 1) + (depth - 2) + ... + 1
            const int d = MAX_PLY + 1;
            PrincipalVariations = new Move[(d * d + d) / 2];
            //Initialize BoardState Stack
            Positions = new BoardState[MAX_PLY];
            for (int i = 0; i < MAX_PLY; i++)
                Positions[i] = new BoardState();
            Positions[0].Copy(board);

            //Initialize a random bonus added to each root move
            Random random = _options.Seed >= 0 ? new(_options.Seed) : Random.Shared;
            RootMoveOffsets = new int[RootMoves.Length];
            for (int i = 0; i < RootMoveOffsets.Length; i++)
                RootMoveOffsets[i] = random.Next(_options.Temperature);
        }

        public void Search(int maxDepth)
        {
            while (Depth < maxDepth)
                SearchDeeper();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool ForcedCut(int depth)
        {
            return depth >= MAX_PLY - 1 || NodesVisited >= _options.MaxNodes || _killSwitch.Get();
        }

        private static Span<Move> GetFirstPVfromBuffer(Move[] pv, int depth)
        {
            //return moves until the first is 'default' move but not more than 'depth' number of moves
            int end = Array.IndexOf(pv, default, 0, depth);
            return new Span<Move>(pv, 0, end >= 0 ? end : depth);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int IndexPV(int ply)
        {
            //Detailed Description:
            //https://github.com/lithander/Leorik/blob/b3236087fbc87e1915725c23ff349e46dfedd0f2/Leorik.Search/IterativeSearchNext.cs
            return Depth * ply - (ply * ply - ply) / 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ExtendPV(int ply, int remaining, Move move)
        {
            int index = IndexPV(ply);
            PrincipalVariations[index] = move;
            int stride = Depth - ply;
            int from = index + stride - 1;
            for (int i = 1; i < remaining; i++)
                PrincipalVariations[index + i] = PrincipalVariations[from + i];
            for (int i = remaining; i < stride; i++)
                PrincipalVariations[index + i] = default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void TruncatePV(int ply)
        {
            PrincipalVariations[IndexPV(ply)] = default;
        }

        /// <summary>
        /// In SearchDeeper we choose which root evaluation method to use.
        /// If _options.UseBNS is set then we use our BNS implementation,
        /// otherwise we fall back to the standard aspiration–window search.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SearchDeeper(Func<bool>? killSwitch = null)
        {
            Depth++;
            _killSwitch = new KillSwitch(killSwitch);
            int score = _options.UseBNS ? EvaluateRootBNS(Depth) : EvaluateRoot(Depth);
            Score = (int)Positions[0].SideToMove * score;
        }

        // --- Existing methods remain unchanged ---
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PickBestCapture(int first, int end)
        {
            //find the best move...
            int best = first;
            int bestScore = Moves[first].MvvLvaScore();
            for (int i = first + 1; i < end; i++)
            {
                int score = Moves[i].MvvLvaScore();
                if (score >= bestScore)
                {
                    best = i;
                    bestScore = score;
                }
            }
            //...swap best with first
            if (best != first)
            {
                (Moves[first], Moves[best]) = (Moves[best], Moves[first]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int EvaluateNext(int ply, int remaining, int alpha, int beta, MoveGen moveGen)
        {
            return -EvaluateTT(ply + 1, remaining - 1, -beta, -alpha, ref moveGen);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int EvaluateTT(int ply, int remaining, int alpha, int beta, ref MoveGen moveGen)
        {
            BoardState current = Positions[ply];
            if (Aborted |= ForcedCut(ply))
                return current.SideToMoveScore();
            //Mate distance pruning
            alpha = Math.Max(alpha, MatedScore(ply));
            beta = Math.Min(beta, MateScore(ply + 1));
            if (alpha >= beta)
                return beta;
            //Drop into QSearch
            if (remaining <= 0)
                return EvaluateQuiet(ply, alpha, beta, moveGen);
            TruncatePV(ply);
            //Handle draws!
            if (IsInsufficientMatingMaterial(current))
                return 0;
            if (current.HalfmoveClock > 99)
                return 0;
            //TODO: checkmate > draw?
            if (IsRepetition(ply))
                return 0;
            //TODO: is scoring *any* repetition as zero premature?
            //Transposition table lookup
            ulong hash = current.ZobristHash;
            if (Transpositions.GetScore(hash, remaining, ply, alpha, beta, out Move bm, out int ttScore))
                return ttScore;
            //Main Search!
            int score = Evaluate(ply, remaining, alpha, beta, moveGen, ref bm);
            if (Aborted)
                return score;
            //Update correction history!
            int staticEval = current.SideToMoveScore() + _history.GetCorrection(current);
            int delta = score - staticEval;
            if ((bm.CapturedPiece() == Piece.None) &&     //Best move either does not exist or is not a capture
                !IsCheckmate(score) &&                     //checkmate scores are excluded!
                !(score <= alpha && delta > 0) &&           //fail-lows should not cause positive adjustment
                !(score >= beta && delta < 0) &&            //fail-highs should not cause negative adjustment
                !current.InCheck())                         //exclude positions that are in check!
            {
                _history.UpdateCorrection(current, remaining, delta);
            }
            //Update transposition table
            Transpositions.Store(hash, remaining, ply, alpha, beta, score, bm);
            return score;
        }

        enum Stage { Best, Captures, Killers, Counter, FollowUp, SortedQuiets, Quiets }
        struct PlayState
        {
            public Stage Stage;
            public int Next;
            public byte PlayedMoves;
            public PlayState(int next)
            {
                Next = next;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool Play(int ply, ref PlayState state, ref MoveGen moveGen)
        {
            BoardState current = Positions[ply];
            BoardState next = Positions[ply + 1];
            while (true)
            {
                if (state.Next == moveGen.Next)
                {
                    switch (state.Stage)
                    {
                        case Stage.Best:
                            state.Next = moveGen.CollectCaptures(current);
                            state.Stage = Stage.Captures;
                            continue;
                        case Stage.Captures:
                            state.Next = moveGen.CollectQuiets(current);
                            state.Stage = Stage.Killers;
                            continue;
                        default:
                            return false;
                    }
                }
                switch (state.Stage)
                {
                    case Stage.Captures:
                        PickBestCapture(state.Next, moveGen.Next);
                        break;
                    case Stage.Killers:
                        state.Stage = PickKiller(ply, state.Next, moveGen.Next);
                        break;
                    case Stage.Counter:
                        state.Stage = PickCounter(ply, state.Next, moveGen.Next);
                        break;
                    case Stage.FollowUp:
                        state.Stage = PickFollowUp(ply, state.Next, moveGen.Next);
                        break;
                    case Stage.SortedQuiets:
                        int historyThreshold = state.PlayedMoves >> 2;
                        if (PickBestHistory(state.Next, moveGen.Next) < historyThreshold)
                            state.Stage = Stage.Quiets;
                        break;
                }
                if (next.Play(current, ref Moves[state.Next++]))
                {
                    state.PlayedMoves++;
                    return true;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Stage PickKiller(int ply, int first, int end)
        {
            if (PickMove(first, end, _history.GetKiller(ply)))
                return Stage.Counter;
            return PickCounter(ply, first, end);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Stage PickCounter(int ply, int first, int end)
        {
            if (PickMove(first, end, _history.GetCounter(ply)))
                return Stage.FollowUp;
            return PickFollowUp(ply, first, end);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Stage PickFollowUp(int ply, int first, int end)
        {
            if (PickMove(first, end, _history.GetFollowUp(ply)))
                return Stage.SortedQuiets;
            PickBestHistory(first, end);
            return Stage.SortedQuiets;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsRepetition(int ply)
        {
            ulong hash = Positions[ply].ZobristHash;
            //start with the positions we've been searching for
            for (int i = ply - 4; i >= 0; i -= 2)
            {
                if (Positions[i].ZobristHash == hash)
                    return true;
                //captures and pawn moves reset the halfmove clock for the purpose of enforcing the 50-move rule and also make a repetition impossible
                if (Positions[i].HalfmoveClock <= 1)
                    return false;
            }
            //continue with the history of positions from the startpos, truncated based on the half-move clock
            int start = _legacy.Length - 1 - (ply & 1);
            for (int i = start; i >= 0; i -= 2)
                if (_legacy[i] == hash)
                    return true;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool PickMove(int first, int end, Move move)
        {
            //find the move...
            for (int i = first + 1; i < end; i++)
            {
                if (Moves[i] == move)
                {
                    //...swap best with first
                    (Moves[first], Moves[i]) = (Moves[i], Moves[first]);
                    return true;
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float PickBestHistory(int first, int end)
        {
            //find the best move...
            int best = first;
            float bestScore = _history.Value(ref Moves[first]);
            for (int i = first + 1; i < end; i++)
            {
                float score = _history.Value(ref Moves[i]);
                if (score > bestScore)
                {
                    best = i;
                    bestScore = score;
                }
            }
            //...swap best with first
            if (best != first)
            {
                (Moves[first], Moves[best]) = (Moves[best], Moves[first]);
            }
            return bestScore;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool AllowNullMove(int ply)
        {
            //if the previous iteration found a mate we do the first few plys without null move to try and find the shortest mate or escape
            return !IsCheckmate(Score) || (ply > Depth / 4);
        }

        // --- Original EvaluateRoot method (used when not using BNS) ---
        private int EvaluateRoot(int depth)
        {
            int eval = (int)Positions[0].SideToMove * Score;
            int window = ASPIRATION_WINDOW;
            while (!Aborted)
            {
                int alpha = eval - window;
                int beta = eval + window;
                eval = EvaluateRoot(depth, alpha, beta);
                if (eval > alpha && eval < beta)
                    break;
                window *= 2;
            }
            return eval;
        }

        private int EvaluateRoot(int depth, int alpha, int beta)
        {
            NodesVisited++;
            BoardState root = Positions[0];
            BoardState next = Positions[1];
            MoveGen moveGen = new(Moves, 0);
            //init staged move generation and play all moves
            for (int i = 0; i < RootMoves.Length; i++)
            {
                Move move = RootMoves[i];
                if (!next.Play(root, ref move))
                    continue;
                //Scoring Root Moves with a random bonus: https://www.chessprogramming.org/Ronald_de_Man
                int bonus = IsCheckmate(Score) ? 0 : RootMoveOffsets[i];
                //moves after the PV move are unlikely to raise alpha! searching with a null-sized window around alpha first...
                //...non-tactical late moves are searched at a reduced depth to make this test even faster!
                int R = (move.CapturedPiece() != Piece.None || next.InCheck()) ? 0 : 2;
                if (i > 0 && EvaluateNext(0, depth - R, alpha - bonus, alpha + 1 - bonus, moveGen) + bonus <= alpha)
                    continue;
                int score = EvaluateNext(0, depth, alpha - bonus, beta - bonus, moveGen) + bonus;
                if (score > alpha)
                {
                    alpha = score;
                    ExtendPV(0, depth, move);
                    //promote new best move to the front
                    for (int j = i; j > 0; j--)
                        RootMoves[j] = RootMoves[j - 1];
                    RootMoves[0] = move;
                }
            }
            //checkmate or draw?
            if (alpha <= MIN_ALPHA)
                return root.InCheck() ? MatedScore(0) : 0;
            return alpha;
        }

        // --- New BNS Implementation ---
        /// <summary>
        /// Implements Best Node Search (BNS) at the root.
        /// The idea is to iteratively choose a test value and perform null-window (zero-window)
        /// searches on each root move. Based on how many moves “succeed” (i.e. return a score ≥ test+1),
        /// we adjust our bounds until only one candidate remains.
        /// </summary>
        /// <param name="depth">Search depth</param>
        /// <returns>Evaluated score for the best move</returns>
    private int EvaluateRootBNS(int depth)
{
    NodesVisited++;
    BoardState root = Positions[0];
    BoardState next = Positions[1];
    MoveGen moveGen = new(Moves, 0);
    int alpha = MIN_ALPHA;
    int beta = MAX_BETA;
    int subtreeCount = RootMoves.Length;
    int bestScore = MIN_ALPHA;
    Move bestMove = null;

    do
    {
        int test = NextGuess(alpha, beta, subtreeCount);
        int betterCount = 0;
        Move currentBestMove = null;
        int currentBestScore = MIN_ALPHA;

        for (int i = 0; i < RootMoves.Length; i++)
        {
            Move move = RootMoves[i];
            if (!next.Play(root, ref move))
                continue;

            int bonus = IsCheckmate(Score) ? 0 : RootMoveOffsets[i];
            int R = (move.CapturedPiece() != Piece.None || next.InCheck()) ? 0 : 2;

            // Reduced search for moves after the first
            if (i > 0)
            {
                int reducedTest = test;
                int reducedX = reducedTest - bonus;
                int reducedDepth = depth - R;
                int reducedScore = EvaluateNext(0, reducedDepth, reducedX - 1, reducedX, moveGen);
                if (reducedScore < reducedX)
                {
                    next.Unplay(root, ref move);
                    continue;
                }
            }

            int X = test - bonus;
            int childScore = EvaluateNext(0, depth, X - 1, X, moveGen);
            int currentScore = childScore + bonus;

            if (currentScore >= test)
            {
                betterCount++;
                if (currentScore > currentBestScore)
                {
                    currentBestScore = currentScore;
                    currentBestMove = move;
                    ExtendPV(0, depth, move);
                }
            }

            next.Unplay(root, ref move);
        }

        if (betterCount > 0)
        {
            alpha = test;
            bestScore = currentBestScore;
            bestMove = currentBestMove;

            // Promote the best move to the front
            if (bestMove != null)
            {
                int bestIndex = Array.IndexOf(RootMoves, bestMove);
                if (bestIndex > 0)
                {
                    for (int j = bestIndex; j > 0; j--)
                    {
                        RootMoves[j] = RootMoves[j - 1];
                    }
                    RootMoves[0] = bestMove;
                }
            }
        }
        else
        {
            beta = test;
        }

    } while (beta - alpha >= 2 && betterCount != 1);

    // Checkmate or stalemate detection
    if (bestScore <= MIN_ALPHA)
        return root.InCheck() ? MatedScore(0) : 0;

    return bestScore;
}

private int NextGuess(int alpha, int beta, int subtreeCount)
{
    return alpha + (beta - alpha) * (subtreeCount - 1) / subtreeCount;
}

        // --- Remainder of search methods (Evaluate, EvaluateQuiet, etc.) remain unchanged ---
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int Evaluate(int ply, int remaining, int alpha, int beta, MoveGen moveGen, ref Move bestMove)
        {
            NodesVisited++;
            BoardState current = Positions[ply];
            BoardState next = Positions[ply + 1];
            bool inCheck = current.InCheck();
            int staticEval = current.SideToMoveScore() + _history.GetCorrection(current);

            //consider null move pruning first
            if (!inCheck && staticEval > beta && beta <= alpha + 1 && !current.IsEndgame() && AllowNullMove(ply))
            {
                if (remaining < 6 && _history.IsExpectedFailHigh(staticEval, beta))
                    return beta;
                next.PlayNullMove(current);
                if (EvaluateNext(ply, remaining - 4, beta - 1, beta, moveGen) >= beta)
                    return beta;
                if (remaining >= 6)
                    _history.NullMovePass(staticEval, beta);
            }

            //init staged move generation and play all moves
            PlayState playState = new(moveGen.Collect(bestMove));
            while (Play(ply, ref playState, ref moveGen))
            {
                if (!inCheck && playState.Stage == Stage.Quiets && remaining <= 2 && alpha == beta - 1)
                    return alpha;
                ref Move move = ref Moves[playState.Next - 1];
                _history.Played(ply, remaining, ref move);
                if (remaining > 1 && playState.Stage > Stage.Best)
                {
                    int R = 0;
                    if (!inCheck && playState.Stage >= Stage.Quiets && !next.InCheck())
                        R += 2;
                    if (!inCheck && _see.IsBad(current, ref move))
                        R += 2;
                    if (EvaluateNext(ply, remaining - R, alpha, alpha + 1, moveGen) <= alpha)
                        continue;
                }
                int score = EvaluateNext(ply, remaining, alpha, beta, moveGen);
                if (score <= alpha)
                    continue;
                alpha = score;
                bestMove = move;
                ExtendPV(ply, remaining, bestMove);
                _history.Good(ply, remaining, ref bestMove);
                if (score >= beta)
                    return beta;
            }
            if (playState.PlayedMoves == 0)
                return inCheck ? MatedScore(ply) : 0;
            return alpha;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int EvaluateQuiet(int ply, int alpha, int beta, MoveGen moveGen)
        {
            NodesVisited++;
            BoardState current = Positions[ply];
            if (IsInsufficientMatingMaterial(current))
                return 0;
            bool inCheck = current.InCheck();
            if (!inCheck)
            {
                int standPatScore = current.SideToMoveScore() + _history.GetCorrection(current);
                if (standPatScore >= beta)
                    return beta;
                if (standPatScore > alpha)
                    alpha = standPatScore;
            }
            if (Aborted |= ForcedCut(ply))
                return current.SideToMoveScore();
            BoardState next = Positions[ply + 1];
            bool movesPlayed = false;
            for (int i = moveGen.CollectCaptures(current); i < moveGen.Next; i++)
            {
                PickBestCapture(i, moveGen.Next);
                if (!inCheck && _see.IsBad(current, ref Moves[i]))
                    continue;
                if (next.QuickPlay(current, ref Moves[i]))
                {
                    movesPlayed = true;
                    int score = -EvaluateQuiet(ply + 1, -beta, -alpha, moveGen);
                    if (score >= beta)
                        return beta;
                    if (score > alpha)
                        alpha = score;
                }
            }
            if (!inCheck)
                return alpha;
            for (int i = moveGen.CollectQuiets(current); i < moveGen.Next; i++)
            {
                if (next.QuickPlay(current, ref Moves[i]))
                {
                    movesPlayed = true;
                    int score = -EvaluateQuiet(ply + 1, -beta, -alpha, moveGen);
                    if (score >= beta)
                        return beta;
                    if (score > alpha)
                        alpha = score;
                }
            }
            return movesPlayed ? alpha : MatedScore(ply);
        }
    }
}
