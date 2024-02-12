﻿using Leorik.Core;
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

        private readonly ulong[,] Positive = new ulong[Squares, Pieces];
        private readonly ulong[,] All = new ulong[Squares, Pieces];
        private readonly Move[] Moves = new Move[MaxPly];
        private readonly Move[] Killers = new Move[MaxPly];
        private readonly Move[,] Counter = new Move[Squares, Pieces];
        private readonly Move[,] FollowUp = new Move[Squares, Pieces];


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int PieceIndex(Piece piece)
        {
            return ((byte)piece >> 1); //BlackPawn = 0...
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Good(int ply, int depth, ref Move move)
        {
            ulong inc = Inc(depth);
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

            ulong inc = Inc(depth);
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
        public ulong Inc(int depth)
        {
            return (ulong)(depth * depth);
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
    }
}