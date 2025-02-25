﻿using Leorik.Core;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using static System.Formats.Asn1.AsnWriter;

namespace Leorik.Search
{
    public class History
    {
        private const int MaxPly = 99;
        private const int Squares = 64;
        private const int Pieces = 14; //including colored 'none'

        private ulong TotalPositive = 0;
        private ulong TotalPlayed = 0;

        long NullMovePassesSum = 0;
        long NullMovePassesCount = 1;

        private readonly ulong[,] Positive = new ulong[Squares, Pieces];
        private readonly ulong[,] All = new ulong[Squares, Pieces];
        private readonly Move[] Moves = new Move[MaxPly];
        private readonly Move[] Killers = new Move[MaxPly];
        private readonly Move[,] Counter = new Move[Squares, Pieces];
        private readonly Move[,] FollowUp = new Move[Squares, Pieces];

        const int CORR_HASH_TABLE_SIZE = 19997; //prime!
        
        struct CorrEntry
        {
            public long Numerator;
            public long Denominator;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Add(long corr, long inc)
            {
                Numerator += corr;
                Denominator += inc;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Get() => (int)(Numerator / (Denominator + 100));
        }
        private readonly CorrEntry[] PawnCorrection = new CorrEntry[2 * CORR_HASH_TABLE_SIZE];
        private readonly CorrEntry[] MinorPieceCorrection = new CorrEntry[2 * CORR_HASH_TABLE_SIZE];
        private readonly CorrEntry[] MajorPieceCorrection = new CorrEntry[2 * CORR_HASH_TABLE_SIZE];
        //private readonly CorrEntry[] BlackPieceCorrection = new CorrEntry[2 * CORR_HASH_TABLE_SIZE];
        //private readonly CorrEntry[] WhitePieceCorrection = new CorrEntry[2 * CORR_HASH_TABLE_SIZE];


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int PieceIndex(Piece piece) => (byte)piece >> 1; //BlackPawn = 0...

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Good(int ply, int depth, ref Move move)
        {
            ulong inc = (ulong)(depth * depth);
            TotalPositive += inc;

            //no killer, followup, counter tracking for captures
            if (move.CapturedPiece() != Piece.None)
                return;

            int iMoving = PieceIndex(move.MovingPiece());
            Positive[move.ToSquare, iMoving] += inc;
            Killers[ply] = move;

            if (ply < 2)
                return;

            Move prev = Moves[ply - 2];
            FollowUp[prev.ToSquare, PieceIndex(prev.MovingPiece())] = move;

            prev = Moves[ply - 1];
            Counter[prev.ToSquare, PieceIndex(prev.MovingPiece())] = move;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Played(int ply, int depth, ref Move move)
        {
            Moves[ply] = move;

            ulong inc = (ulong)(depth * depth);
            TotalPlayed += inc;

            if (move.CapturedPiece() != Piece.None)
                return;

            int iMoving = PieceIndex(move.MovingPiece());
            All[move.ToSquare, iMoving] += inc;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Value(ref Move move)
        {
            int iMoving = PieceIndex(move.MovingPiece());
            float a = Positive[move.ToSquare, iMoving];
            float b = All[move.ToSquare, iMoving];
            //local-ratio / average-ratio
            return TotalPlayed * a / (b * TotalPositive + 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Move GetKiller(int ply)
        {
            return Killers[ply];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Move GetCounter(int ply)
        {
            Move prev = Moves[ply - 1];
            return Counter[prev.ToSquare, PieceIndex(prev.MovingPiece())];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Move GetFollowUp(int ply)
        {
            if (ply < 2)
                return default;

            Move prev = Moves[ply - 2];
            return FollowUp[prev.ToSquare, PieceIndex(prev.MovingPiece())];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void NullMovePass(int eval, int beta)
        {
            NullMovePassesCount++;
            NullMovePassesSum += eval - beta;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsExpectedFailHigh(int eval, int beta)
        {
            int avgNullMovePass = (int)(NullMovePassesSum / NullMovePassesCount);
            return eval > beta + avgNullMovePass;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetCorrection(BoardState board)
        {
            int stm = (board.SideToMove == Color.Black) ? 1 : 0;

            int index = CorrectionIndex(board.Pawns) + stm;
            int result = PawnCorrection[index].Get();

            //Score of Leorik-3.0.12 vs Leorik-3.0.11v16: 1783 - 1573 - 6644  [0.510] 10000
            //...      Leorik-3.0.12 playing White: 1115 - 564 - 3321  [0.555] 5000
            //...      Leorik-3.0.12 playing Black: 668 - 1009 - 3323  [0.466] 5000
            //...      White vs Black: 2124 - 1232 - 6644  [0.545] 10000
            //Elo difference: 7.3 +/- 3.9, LOS: 100.0 %, DrawRatio: 66.4 %

            index = CorrectionIndex(board.Knights | board.Bishops) + stm;
            result += MinorPieceCorrection[index].Get();

            index = CorrectionIndex(board.Queens | board.Rooks) + stm;
            result += MajorPieceCorrection[index].Get();

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdateCorrection(BoardState board, int depth, int delta)
        {
            long inc = depth * depth;
            long corr = inc * Math.Clamp(delta, -100, +100);
            int stm = (board.SideToMove == Color.Black) ? 1 : 0;

            int index = CorrectionIndex(board.Pawns) + stm;
            PawnCorrection[index].Add(corr, inc);
            
            index = CorrectionIndex(board.Knights | board.Bishops) + stm;
            MinorPieceCorrection[index].Add(corr, inc);

            index = CorrectionIndex(board.Queens | board.Rooks) + stm;
            MajorPieceCorrection[index].Add(corr, inc);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int CorrectionIndex(ulong bits) => (int)(bits % CORR_HASH_TABLE_SIZE) * 2;
    }
}