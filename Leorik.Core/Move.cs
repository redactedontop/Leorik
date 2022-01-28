﻿using System.Runtime.CompilerServices;

namespace Leorik.Core
{
    public readonly struct Move
    {
        public readonly Piece Flags;
        public readonly Piece Target;
        public readonly byte FromSquare;
        public readonly byte ToSquare;

        public Move(Piece flags, int fromIndex, int toIndex, Piece target)
        {
            Flags = flags;
            FromSquare = (byte)fromIndex;
            ToSquare = (byte)toIndex;
            Target = target;
        }

        public Move(string uciMoveNotation, Piece flags = Piece.None)
        {
            Target = Piece.None;
            Flags = flags;
            if (uciMoveNotation.Length < 4)
                throw new ArgumentException($"Long algebraic notation expected. '{uciMoveNotation}' is too short!");
            if (uciMoveNotation.Length > 5)
                throw new ArgumentException($"Long algebraic notation expected. '{uciMoveNotation}' is too long!");

            //expected format is the long algebraic notation without piece names
            //https://en.wikipedia.org/wiki/Algebraic_notation_(chess)
            //Examples: e2e4, e7e5, e1g1(white short castling), e7e8q(for promotion)
            string fromSquare = uciMoveNotation.Substring(0, 2);
            string toSquare = uciMoveNotation.Substring(2, 2);
            FromSquare = (byte)Notation.GetSquare(fromSquare);
            ToSquare = (byte)Notation.GetSquare(toSquare);
            //the presence of a 5th character should mean promotion
            if (uciMoveNotation.Length == 5)
            {
                Piece promo = Notation.GetPiece(uciMoveNotation[4]) & ~Piece.ColorMask;
                Flags |= (Piece)((int)promo << 3) | Piece.Pawn;
            }
        }

        public readonly static Move BlackCastlingShort = new("e8g8", Piece.Black | Piece.CastleShort);
        public readonly static Move BlackCastlingLong = new("e8c8", Piece.Black | Piece.CastleLong);
        public readonly static Move WhiteCastlingShort = new("e1g1", Piece.White | Piece.CastleShort);
        public readonly static Move WhiteCastlingLong = new("e1c1", Piece.White | Piece.CastleLong);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Piece NewPiece()
        {
            return Flags < Piece.KnightPromotion || Flags >= Piece.CastleShort
                ? Flags & Piece.PieceMask
                : (Piece)((int)Flags >> 3) & ~Piece.ColorMask | (Flags & Piece.ColorMask);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Piece CapturedPiece()
        {
            return Target;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Piece MovingPiece()
        {
            return Flags & Piece.PieceMask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int MvvLvaScore()
        {
            //Most valuable Victim, Least valuable Attacker
            //EnPassent = -1
            //King capturing Pawn = 1 * 6 - 6 = 0
            //Pawn capturing Queen = 6 * 5 - 1 = 29  
            return 6 * Order(Target) - Order(Flags & Piece.PieceMask);
        }

        //Pawn = 1, Knight = 2, Bishop = 3; Rook = 4, Queen = 5, King = 6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Order(Piece piece) => (int)piece >> 2;

    }
}
