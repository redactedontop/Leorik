﻿using System.Runtime.CompilerServices;
using static Leorik.Core.Bitboard;

namespace Leorik.Core
{
    public class BoardState
    {
        public ulong White;
        public ulong Black;
        public ulong Pawns;
        public ulong Knights;
        public ulong Bishops;
        public ulong Rooks;
        public ulong Queens;
        public ulong Kings;
        public ulong CastleFlags;
        public ulong EnPassant;

        public Color SideToMove;
        public Evaluation Eval;
        public ulong ZobristHash;

        public const ulong BlackQueensideRookBit = 0x0100000000000000UL;//1UL << Notation.ToSquare("a8");
        public const ulong BlackKingsideRookBit = 0x8000000000000000UL;//1UL << Notation.ToSquare("h8");
        public const ulong BlackCastlingBits = BlackQueensideRookBit | BlackKingsideRookBit;

        public const ulong WhiteQueensideRookBit = 0x0000000000000001UL;//1UL << Notation.ToSquare("a1");
        public const ulong WhiteKingsideRookBit = 0x0000000000000080UL;//1UL << Notation.ToSquare("h1");
        public const ulong WhiteCastlingBits = WhiteQueensideRookBit | WhiteKingsideRookBit;

        public BoardState()
        {
        }

        public BoardState(BoardState other) : this()
        {
            Copy(other);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanWhiteCastleLong()
        {
            return (CastleFlags & WhiteQueensideRookBit) != 0 && ((Black | White) & 0x000000000000000EUL) == 0;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanWhiteCastleShort()
        {
            return (CastleFlags & WhiteKingsideRookBit) != 0 && ((Black | White) & 0x0000000000000060UL) == 0;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanBlackCastleLong()
        {
            return (CastleFlags & BlackQueensideRookBit) != 0 && ((Black | White) & 0x0E00000000000000UL) == 0;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanBlackCastleShort()
        {
            return (CastleFlags & BlackKingsideRookBit) != 0 && ((Black | White) & 0x6000000000000000UL) == 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetBit(int square, Piece piece)
        {
            ulong bbPiece = 1UL << square;
            switch (piece & Piece.ColorMask)
            {
                case Piece.Black:
                    Black |= bbPiece;
                    break;
                case Piece.White:
                    White |= bbPiece;
                    break;
            }
            switch (piece & Piece.TypeMask)
            {
                case Piece.Pawn:
                    Pawns |= bbPiece;
                    break;
                case Piece.Knight:
                    Knights |= bbPiece;
                    break;
                case Piece.Bishop:
                    Bishops |= bbPiece;
                    break;
                case Piece.Rook:
                    Rooks |= bbPiece;
                    break;
                case Piece.Queen:
                    Queens |= bbPiece;
                    break;
                case Piece.King:
                    Kings |= bbPiece;
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Copy(BoardState other)
        {
            CopyUnmasked(other);
            EnPassant = other.EnPassant;
            SideToMove = other.SideToMove;
            Eval = other.Eval;
            ZobristHash = other.ZobristHash;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdateEval()
        {
            Eval = new Evaluation(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdateEval(BoardState from, ref Move move)
        {
            Eval = from.Eval;
            Eval.Update(ref move);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool PlayAndUpdate(BoardState from, ref Move move)
        {
            if (from.SideToMove == Color.White)
            {
                PlayWhite(from, ref move);
                if (IsAttackedByBlack(LSB(Kings & White)))
                    return false;
            }
            else
            {
                PlayBlack(from, ref move);
                if (IsAttackedByWhite(LSB(Kings & Black)))
                    return false;
            }
            UpdateEval(from, ref move);
            //UpdateHash(from, ref move);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool PlayAndFullUpdate(BoardState from, ref Move move)
        {
            if (from.SideToMove == Color.White)
            {
                PlayWhite(from, ref move);
                if (IsAttackedByBlack(LSB(Kings & White)))
                    return false;
            }
            else
            {
                PlayBlack(from, ref move);
                if (IsAttackedByWhite(LSB(Kings & Black)))
                    return false;
            }
            UpdateEval(from, ref move);
            UpdateHash(from, ref move);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Play(Move move)
        {
            return PlayAndFullUpdate(this, ref move);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Play(BoardState from, ref Move move)
        {
            if (from.SideToMove == Color.White)
            {
                PlayWhite(from, ref move);
                if (IsAttackedByBlack(LSB(Kings & White)))
                    return false;
            }
            else
            {
                PlayBlack(from, ref move);
                if (IsAttackedByWhite(LSB(Kings & Black)))
                    return false;
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PlayBlack(BoardState from, ref Move move)
        {
            ulong bbTo = 1UL << move.ToSquare;
            ulong bbFrom = 1UL << move.FromSquare;
            CopyBitboards(from, from.White & bbTo);
            EnPassant = 0;
            SideToMove = Color.White;
            Black ^= bbFrom | bbTo;

            switch (move.Flags & ~Piece.ColorMask)
            {
                case Piece.Pawn:
                    Pawns ^= bbFrom | bbTo;
                    EnPassant = (bbTo << 8) & (bbFrom >> 8);
                    break;
                case Piece.QueenPromotion:
                    Pawns &= ~bbFrom;
                    Queens |= bbTo;
                    break;
                case Piece.RookPromotion:
                    Pawns &= ~bbFrom;
                    Rooks |= bbTo;
                    break;
                case Piece.BishopPromotion:
                    Pawns &= ~bbFrom;
                    Bishops |= bbTo;
                    break;
                case Piece.KnightPromotion:
                    Pawns &= ~bbFrom;
                    Knights |= bbTo;
                    break;
                case Piece.EnPassant:
                    Pawns ^= bbFrom | bbTo | bbTo << 8;
                    White &= ~(bbTo << 8);
                    break;
                case Piece.Knight:
                    Knights ^= bbFrom | bbTo;
                    break;
                case Piece.Bishop:
                    Bishops ^= bbFrom | bbTo;
                    break;
                case Piece.Rook:
                    Rooks ^= bbFrom | bbTo;
                    CastleFlags &= ~bbFrom;
                    break;
                case Piece.Queen:
                    Queens ^= bbFrom | bbTo;
                    break;
                case Piece.King:
                    Kings ^= bbFrom | bbTo;
                    CastleFlags &= ~BlackCastlingBits;
                    break;
                case Piece.CastleShort:
                    Kings ^= bbFrom | bbTo;
                    CastleFlags &= ~BlackCastlingBits;
                    Rooks ^= 0xA000000000000000UL;
                    Black ^= 0xA000000000000000UL;
                    break;
                case Piece.CastleLong:
                    Kings ^= bbFrom | bbTo;
                    CastleFlags &= ~BlackCastlingBits;
                    Rooks ^= 0x0900000000000000UL;
                    Black ^= 0x0900000000000000UL;
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PlayWhite(BoardState from, ref Move move)
        {
            ulong bbTo = 1UL << move.ToSquare;
            ulong bbFrom = 1UL << move.FromSquare;

            CopyBitboards(from, from.Black & bbTo);
            EnPassant = 0;
            SideToMove = Color.Black;
            White ^= bbFrom | bbTo;

            switch (move.Flags & ~Piece.ColorMask)
            {
                case Piece.Pawn:
                    Pawns ^= bbFrom | bbTo;
                    EnPassant = (bbTo >> 8) & (bbFrom << 8);
                    break;
                case Piece.QueenPromotion:
                    Pawns &= ~bbFrom;
                    Queens |= bbTo;
                    break;
                case Piece.RookPromotion:
                    Pawns &= ~bbFrom;
                    Rooks |= bbTo;
                    break;
                case Piece.BishopPromotion:
                    Pawns &= ~bbFrom;
                    Bishops |= bbTo;
                    break;
                case Piece.KnightPromotion:
                    Pawns &= ~bbFrom;
                    Knights |= bbTo;
                    break;
                case Piece.EnPassant:
                    Pawns ^= bbFrom | bbTo | bbTo >> 8;
                    Black &= ~(bbTo >> 8);
                    break;
                case Piece.Knight:
                    Knights ^= bbFrom | bbTo;
                    break;
                case Piece.Bishop:
                    Bishops ^= bbFrom | bbTo;
                    break;
                case Piece.Rook:
                    Rooks ^= bbFrom | bbTo;
                    CastleFlags &= ~bbFrom;
                    break;
                case Piece.Queen:
                    Queens ^= bbFrom | bbTo;
                    break;
                case Piece.King:
                    Kings ^= bbFrom | bbTo;
                    CastleFlags &= ~WhiteCastlingBits;
                    break;
                case Piece.CastleShort:
                    Kings ^= bbFrom | bbTo;
                    CastleFlags &= ~WhiteCastlingBits;
                    Rooks ^= 0x00000000000000A0UL;
                    White ^= 0x00000000000000A0UL;
                    break;
                case Piece.CastleLong:
                    Kings ^= bbFrom | bbTo;
                    CastleFlags &= ~WhiteCastlingBits;
                    Rooks ^= 0x0000000000000009UL;
                    White ^= 0x0000000000000009UL;
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CopyBitboards(BoardState from, ulong mask)
        {
            if (mask == 0)
                CopyUnmasked(from);
            else
                CopyMasked(from, ~mask);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CopyUnmasked(BoardState from)
        {
            White = from.White;
            Black = from.Black;
            Pawns = from.Pawns;
            Knights = from.Knights;
            Bishops = from.Bishops;
            Rooks = from.Rooks;
            Queens = from.Queens;
            Kings = from.Kings;
            CastleFlags = from.CastleFlags;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CopyMasked(BoardState from, ulong mask)
        {
            White = from.White & mask;
            Black = from.Black & mask;
            Pawns = from.Pawns & mask;
            Knights = from.Knights & mask;
            Bishops = from.Bishops & mask;
            Rooks = from.Rooks & mask;
            Queens = from.Queens & mask;
            Kings = from.Kings & mask;
            CastleFlags = from.CastleFlags & mask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsChecked(Color color)
        {
            if (color == Color.White)
                return IsAttackedByBlack(LSB(Kings & White));
            else
                return IsAttackedByWhite(LSB(Kings & Black));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsAttackedByWhite(int square)
        {
            ulong pieces = White & Knights;
            if (pieces > 0 && (pieces & KnightTargets[square]) > 0)
                return true;

            pieces = White & Kings;
            if (pieces > 0 && (pieces & KingTargets[square]) > 0)
                return true;

            pieces = White & (Queens | Bishops);
            if (pieces > 0 && (pieces & DiagonalMask[square]) > 0 && (pieces & GetBishopTargets(Black | White, square)) > 0)
                return true;

            pieces = White & (Queens | Rooks);
            if (pieces > 0 && (pieces & OrthogonalMask[square]) > 0 && (pieces & GetRookTargets(Black | White, square)) > 0)
                return true;

            //Warning: pawn attacks do not consider en-passent!
            pieces = White & Pawns;
            ulong left = (pieces & 0xFEFEFEFEFEFEFEFEUL) << 7;
            ulong right = (pieces & 0x7F7F7F7F7F7F7F7FUL) << 9;
            return ((left | right) & 1UL << square) > 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsAttackedByBlack(int square)
        {
            ulong pieces = Black & Knights;
            if (pieces > 0 && (pieces & KnightTargets[square]) > 0)
                return true;

            pieces = Black & Kings;
            if (pieces > 0 && (pieces & KingTargets[square]) > 0)
                return true;

            pieces = Black & (Queens | Bishops);
            if (pieces > 0 && (pieces & DiagonalMask[square]) > 0 && (pieces & GetBishopTargets(Black | White, square)) > 0)
                return true;

            pieces = Black & (Queens | Rooks);
            if (pieces > 0 && (pieces & OrthogonalMask[square]) > 0 && (pieces & GetRookTargets(Black | White, square)) > 0)
                return true;

            //Warning: pawn attacks do not consider en-passent!
            pieces = Black & Pawns;
            ulong left = (pieces & 0xFEFEFEFEFEFEFEFEUL) >> 9;
            ulong right = (pieces & 0x7F7F7F7F7F7F7F7FUL) >> 7;
            return ((left | right) & 1UL << square) > 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Piece GetPiece(int square)
        {
            /*
            2nd Bit = White or Black? (implies the Piece bit to be set to)
            Black = 1,      //01
            White = 3,      //11

            3rd+ Bits = Type of Piece
            Pawn = 4,       //00100
            Knight = 8,     //01000
            Bishop = 12,    //01100
            Rook = 16,      //10000
            Queen = 20,     //10100
            King = 24,      //11000
            */
            return (Piece)(Bit(Black | White, square, 0) |
                           Bit(White, square, 1) |
                           Bit(Pawns | Bishops | Queens, square, 2) |
                           Bit(Knights | Bishops | Kings, square, 3) |
                           Bit(Kings | Rooks | Queens, square, 4));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int Bit(ulong bb, int square, int shift) => (int)((bb >> square) & 1) << shift;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdateHash()
        {
            //Side to move
            ZobristHash = (SideToMove == Color.Black) ? Zobrist.SideToMove : 0;

            //Pieces
            for (ulong bits = White | Black; bits != 0; bits = ClearLSB(bits))
            {
                int square = LSB(bits);
                ZobristHash ^= Zobrist.PieceSquare(GetPiece(square), square);
            }

            //En passent & Castling
            for (ulong bits = CastleFlags | EnPassant; bits != 0; bits = ClearLSB(bits))
                ZobristHash ^= Zobrist.Castling(LSB(bits));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdateHash(BoardState from, ref Move move)
        {
            ZobristHash = from.ZobristHash;
            
            ZobristHash ^= Zobrist.SideToMove;
            ZobristHash ^= Zobrist.PieceSquare(move.MovingPiece(), move.FromSquare);
            ZobristHash ^= Zobrist.PieceSquare(move.CapturedPiece(), move.ToSquare);
            ZobristHash ^= Zobrist.PieceSquare(move.NewPiece(), move.ToSquare);

            switch (move.Flags)
            {
                case Piece.EnPassant | Piece.BlackPawn:
                    ZobristHash ^= Zobrist.PieceSquare(Piece.WhitePawn, move.ToSquare + 8);
                    break;
                case Piece.EnPassant | Piece.WhitePawn:
                    ZobristHash ^= Zobrist.PieceSquare(Piece.BlackPawn, move.ToSquare - 8);
                    break;
                case Piece.CastleShort | Piece.Black:
                    ZobristHash ^= Zobrist.PieceSquare(Piece.BlackRook, 63);
                    ZobristHash ^= Zobrist.PieceSquare(Piece.BlackRook, 61);
                    break;
                case Piece.CastleLong | Piece.Black:
                    ZobristHash ^= Zobrist.PieceSquare(Piece.BlackRook, 56);
                    ZobristHash ^= Zobrist.PieceSquare(Piece.BlackRook, 59);
                    break;
                case Piece.CastleShort | Piece.White:
                    ZobristHash ^= Zobrist.PieceSquare(Piece.WhiteRook, 7);
                    ZobristHash ^= Zobrist.PieceSquare(Piece.WhiteRook, 5);
                    break;
                case Piece.CastleLong | Piece.White:
                    ZobristHash ^= Zobrist.PieceSquare(Piece.WhiteRook, 0);
                    ZobristHash ^= Zobrist.PieceSquare(Piece.WhiteRook, 3);
                    break;
            }

            //En passent & Castling
            for (ulong bits = (CastleFlags ^ from.CastleFlags) | (EnPassant ^ from.EnPassant); bits != 0; bits = ClearLSB(bits))
                ZobristHash ^= Zobrist.Castling(LSB(bits));
        }
    }
}
