﻿using System.Numerics;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

namespace Leorik.Core
{
    public struct Evaluation
    {
        public static readonly int PhaseSum = 5000;

        public short PhaseValue;
        public EvalTerm Pawns;
        public EvalTerm Material;
        public EvalTerm Mobility;

        public float Phase => NormalizePhase(PhaseValue);

        public short Score { get; private set; }

        public short RawScore => (short)(EvalBase() + NormalizePhase(PhaseValue) * EvalEndgame());

        //King-Relative Params
        private Vector128<float> _white;
        private Vector128<float> _black;

        public Evaluation(BoardState board)
        {
            PawnStructure.Update(board, ref Pawns);
            MobilityEval.Update(board, ref Mobility);
            AddPieces(board);
            UpdateScore(board);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Evaluation(Evaluation eval)
        {
            this = eval;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Copy(Evaluation eval)
        {
            this = eval;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Update(BoardState board, ref Move move)
        {
            Mobility = default;
            MobilityEval.Update(board, ref Mobility);
            PawnStructure.Update(board, ref Pawns);
            TryUpdateMaterial(board, ref move);
            UpdateScore(board);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void TryUpdateMaterial(BoardState board, ref Move move)
        {
            if (move.MovingPieceType() == Piece.King)
            {
                PhaseValue = 0;
                Material = default;
                AddPieces(board);
            }
            else
            {
                UpdateMaterial(ref move);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int EvalBase() => Pawns.Base + (int)Material.Base + Mobility.Base;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int EvalEndgame() => Pawns.Endgame + (int)Material.Endgame + Mobility.Endgame;

        public void UpdateScore(BoardState board)
        {
            float score = EvalBase() + NormalizePhase(PhaseValue) * EvalEndgame();
            Score = (short)(Endgame.IsDrawn(board) ? (int)score >> 4 : score);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddPieces(BoardState board)
        {
            UpdateKingParams(board);

            for (ulong bits = board.White; bits != 0; bits = Bitboard.ClearLSB(bits))
            {
                int square = Bitboard.LSB(bits);
                Piece piece = board.GetPiece(square);
                AddWhitePiece(piece, square);
            }

            for (ulong bits = board.Black; bits != 0; bits = Bitboard.ClearLSB(bits))
            {
                int square = Bitboard.LSB(bits);
                Piece piece = board.GetPiece(square);
                AddBlackPiece(piece, square);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateMaterial(ref Move move)
        {
            if ((move.MovingPiece() & Piece.ColorMask) == Piece.White)
            {
                RemoveWhitePiece(move.MovingPiece(), move.FromSquare);
                AddWhitePiece(move.NewPiece(), move.ToSquare);

                if (move.CapturedPiece() != Piece.None)
                    RemoveBlackPiece(move.CapturedPiece(), move.ToSquare);
            }
            else
            {
                RemoveBlackPiece(move.MovingPiece(), move.FromSquare);
                AddBlackPiece(move.NewPiece(), move.ToSquare);

                if (move.CapturedPiece() != Piece.None)
                    RemoveWhitePiece(move.CapturedPiece(), move.ToSquare);
            }

            switch (move.Flags)
            {
                case Piece.EnPassant | Piece.BlackPawn:
                    RemoveWhitePiece(Piece.WhitePawn, move.ToSquare + 8);
                    break;
                case Piece.EnPassant | Piece.WhitePawn:
                    RemoveBlackPiece(Piece.BlackPawn, move.ToSquare - 8);
                    break;
                case Piece.CastleShort | Piece.Black:
                    RemoveBlackPiece(Piece.BlackRook, 63);
                    AddBlackPiece(Piece.BlackRook, 61);
                    break;
                case Piece.CastleLong | Piece.Black:
                    RemoveBlackPiece(Piece.BlackRook, 56);
                    AddBlackPiece(Piece.BlackRook, 59);
                    break;
                case Piece.CastleShort | Piece.White:
                    RemoveWhitePiece(Piece.WhiteRook, 7);
                    AddWhitePiece(Piece.WhiteRook, 5);
                    break;
                case Piece.CastleLong | Piece.White:
                    RemoveWhitePiece(Piece.WhiteRook, 0);
                    AddWhitePiece(Piece.WhiteRook, 3);
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateKingParams(BoardState board)
        {
            _white = GetKingParams(Bitboard.LSB(board.White & board.Kings) ^ 56);
            _black = GetKingParams(Bitboard.LSB(board.Black & board.Kings));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Vector128<float> GetKingParams(int kingSquare)
        {
            float kingFile = 0.285714f * Bitboard.File(kingSquare) - 1f;
            float kingRank = 0.285714f * Bitboard.Rank(kingSquare) - 1f;
            return Vector128.Create(
                kingFile * kingFile,
                kingFile,
                kingRank * kingRank,
                kingRank);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddBlackPiece(Piece piece, int squareIndex)
        {
            int pieceIndex = PieceIndex(piece);
            PhaseValue += Weights.PhaseValues[pieceIndex];
            Material.SubtractMaterial(pieceIndex, squareIndex, Vector256.Create(_black, _white));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RemoveBlackPiece(Piece piece, int squareIndex)
        {
            int pieceIndex = PieceIndex(piece);
            PhaseValue -= Weights.PhaseValues[pieceIndex];
            Material.AddMaterial(pieceIndex, squareIndex, Vector256.Create(_black, _white));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddWhitePiece(Piece piece, int squareIndex)
        {
            int pieceIndex = PieceIndex(piece);
            PhaseValue += Weights.PhaseValues[pieceIndex];
            Material.AddMaterial(pieceIndex, squareIndex ^ 56, Vector256.Create(_white, _black));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RemoveWhitePiece(Piece piece, int squareIndex)
        {
            int pieceIndex = PieceIndex(piece);
            PhaseValue -= Weights.PhaseValues[pieceIndex];
            Material.SubtractMaterial(pieceIndex, squareIndex ^ 56, Vector256.Create(_white, _black));
        }

        public const int CheckmateBase = 9000;
        public const int CheckmateScore = 9999;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetMateDistance(int score)
        {
            int plies = CheckmateScore - Math.Abs(score);
            int moves = (plies + 1) / 2;
            return moves;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCheckmate(int score) => Math.Abs(score) > CheckmateBase;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Checkmate(Color color, int ply) => (int)color * (ply - CheckmateScore);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MatedScore(int ply) => ply - CheckmateScore;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MateScore(int ply) => CheckmateScore - ply;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float NormalizePhase(float phaseValue) => (PhaseSum - phaseValue) / PhaseSum;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int PieceIndex(Piece piece) => ((int)piece >> 2) - 1;
    }
}
