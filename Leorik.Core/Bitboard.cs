﻿//#define PEXT
#define KISS

using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;

namespace Leorik.Core
{
    public static class Bitboard
    {
        public static readonly ulong[] KingTargets =
        {
            0x0000000000000302, 0x0000000000000705, 0x0000000000000E0A, 0x0000000000001C14,
            0x0000000000003828, 0x0000000000007050, 0x000000000000E0A0, 0x000000000000C040,
            0x0000000000030203, 0x0000000000070507, 0x00000000000E0A0E, 0x00000000001C141C,
            0x0000000000382838, 0x0000000000705070, 0x0000000000E0A0E0, 0x0000000000C040C0,
            0x0000000003020300, 0x0000000007050700, 0x000000000E0A0E00, 0x000000001C141C00,
            0x0000000038283800, 0x0000000070507000, 0x00000000E0A0E000, 0x00000000C040C000,
            0x0000000302030000, 0x0000000705070000, 0x0000000E0A0E0000, 0x0000001C141C0000,
            0x0000003828380000, 0x0000007050700000, 0x000000E0A0E00000, 0x000000C040C00000,
            0x0000030203000000, 0x0000070507000000, 0x00000E0A0E000000, 0x00001C141C000000,
            0x0000382838000000, 0x0000705070000000, 0x0000E0A0E0000000, 0x0000C040C0000000,
            0x0003020300000000, 0x0007050700000000, 0x000E0A0E00000000, 0x001C141C00000000,
            0x0038283800000000, 0x0070507000000000, 0x00E0A0E000000000, 0x00C040C000000000,
            0x0302030000000000, 0x0705070000000000, 0x0E0A0E0000000000, 0x1C141C0000000000,
            0x3828380000000000, 0x7050700000000000, 0xE0A0E00000000000, 0xC040C00000000000,
            0x0203000000000000, 0x0507000000000000, 0x0A0E000000000000, 0x141C000000000000,
            0x2838000000000000, 0x5070000000000000, 0xA0E0000000000000, 0x40C0000000000000
        };

        public static readonly ulong[] KnightTargets =
        {
            0x0000000000020400, 0x0000000000050800, 0x00000000000A1100, 0x0000000000142200,
            0x0000000000284400, 0x0000000000508800, 0x0000000000A01000, 0x0000000000402000,
            0x0000000002040004, 0x0000000005080008, 0x000000000A110011, 0x0000000014220022,
            0x0000000028440044, 0x0000000050880088, 0x00000000A0100010, 0x0000000040200020,
            0x0000000204000402, 0x0000000508000805, 0x0000000A1100110A, 0x0000001422002214,
            0x0000002844004428, 0x0000005088008850, 0x000000A0100010A0, 0x0000004020002040,
            0x0000020400040200, 0x0000050800080500, 0x00000A1100110A00, 0x0000142200221400,
            0x0000284400442800, 0x0000508800885000, 0x0000A0100010A000, 0x0000402000204000,
            0x0002040004020000, 0x0005080008050000, 0x000A1100110A0000, 0x0014220022140000,
            0x0028440044280000, 0x0050880088500000, 0x00A0100010A00000, 0x0040200020400000,
            0x0204000402000000, 0x0508000805000000, 0x0A1100110A000000, 0x1422002214000000,
            0x2844004428000000, 0x5088008850000000, 0xA0100010A0000000, 0x4020002040000000,
            0x0400040200000000, 0x0800080500000000, 0x1100110A00000000, 0x2200221400000000,
            0x4400442800000000, 0x8800885000000000, 0x100010A000000000, 0x2000204000000000,
            0x0004020000000000, 0x0008050000000000, 0x00110A0000000000, 0x0022140000000000,
            0x0044280000000000, 0x0088500000000000, 0x0010A00000000000, 0x0020400000000000
        };

        public static readonly ulong[] DiagonalMask =
        {
            0x8040201008040200, 0x0080402010080500, 0x0000804020110A00, 0x0000008041221400,
            0x0000000182442800, 0x0000010204885000, 0x000102040810A000, 0x0102040810204000,
            0x4020100804020002, 0x8040201008050005, 0x00804020110A000A, 0x0000804122140014,
            0x0000018244280028, 0x0001020488500050, 0x0102040810A000A0, 0x0204081020400040,
            0x2010080402000204, 0x4020100805000508, 0x804020110A000A11, 0x0080412214001422,
            0x0001824428002844, 0x0102048850005088, 0x02040810A000A010, 0x0408102040004020,
            0x1008040200020408, 0x2010080500050810, 0x4020110A000A1120, 0x8041221400142241,
            0x0182442800284482, 0x0204885000508804, 0x040810A000A01008, 0x0810204000402010,
            0x0804020002040810, 0x1008050005081020, 0x20110A000A112040, 0x4122140014224180,
            0x8244280028448201, 0x0488500050880402, 0x0810A000A0100804, 0x1020400040201008,
            0x0402000204081020, 0x0805000508102040, 0x110A000A11204080, 0x2214001422418000,
            0x4428002844820100, 0x8850005088040201, 0x10A000A010080402, 0x2040004020100804,
            0x0200020408102040, 0x0500050810204080, 0x0A000A1120408000, 0x1400142241800000,
            0x2800284482010000, 0x5000508804020100, 0xA000A01008040201, 0x4000402010080402,
            0x0002040810204080, 0x0005081020408000, 0x000A112040800000, 0x0014224180000000,
            0x0028448201000000, 0x0050880402010000, 0x00A0100804020100, 0x0040201008040201
        };

        public static readonly ulong[] OrthogonalMask =
        {
            0x01010101010101FE, 0x02020202020202FD, 0x04040404040404FB, 0x08080808080808F7,
            0x10101010101010EF, 0x20202020202020DF, 0x40404040404040BF, 0x808080808080807F,
            0x010101010101FE01, 0x020202020202FD02, 0x040404040404FB04, 0x080808080808F708,
            0x101010101010EF10, 0x202020202020DF20, 0x404040404040BF40, 0x8080808080807F80,
            0x0101010101FE0101, 0x0202020202FD0202, 0x0404040404FB0404, 0x0808080808F70808,
            0x1010101010EF1010, 0x2020202020DF2020, 0x4040404040BF4040, 0x80808080807F8080,
            0x01010101FE010101, 0x02020202FD020202, 0x04040404FB040404, 0x08080808F7080808,
            0x10101010EF101010, 0x20202020DF202020, 0x40404040BF404040, 0x808080807F808080,
            0x010101FE01010101, 0x020202FD02020202, 0x040404FB04040404, 0x080808F708080808,
            0x101010EF10101010, 0x202020DF20202020, 0x404040BF40404040, 0x8080807F80808080,
            0x0101FE0101010101, 0x0202FD0202020202, 0x0404FB0404040404, 0x0808F70808080808,
            0x1010EF1010101010, 0x2020DF2020202020, 0x4040BF4040404040, 0x80807F8080808080,
            0x01FE010101010101, 0x02FD020202020202, 0x04FB040404040404, 0x08F7080808080808,
            0x10EF101010101010, 0x20DF202020202020, 0x40BF404040404040, 0x807F808080808080,
            0xFE01010101010101, 0xFD02020202020202, 0xFB04040404040404, 0xF708080808080808,
            0xEF10101010101010, 0xDF20202020202020, 0xBF40404040404040, 0x7F80808080808080
        };

        public static readonly ulong[] Diagonals =
        {
            0x8040201008040201, 0x0080402010080402, 0x0000804020100804, 0x0000008040201008,
            0x0000000080402010, 0x0000000000804020, 0x0000000000008040, 0x0000000000000080,
            0x4020100804020100, 0x8040201008040201, 0x0080402010080402, 0x0000804020100804,
            0x0000008040201008, 0x0000000080402010, 0x0000000000804020, 0x0000000000008040,
            0x2010080402010000, 0x4020100804020100, 0x8040201008040201, 0x0080402010080402,
            0x0000804020100804, 0x0000008040201008, 0x0000000080402010, 0x0000000000804020,
            0x1008040201000000, 0x2010080402010000, 0x4020100804020100, 0x8040201008040201,
            0x0080402010080402, 0x0000804020100804, 0x0000008040201008, 0x0000000080402010,
            0x0804020100000000, 0x1008040201000000, 0x2010080402010000, 0x4020100804020100,
            0x8040201008040201, 0x0080402010080402, 0x0000804020100804, 0x0000008040201008,
            0x0402010000000000, 0x0804020100000000, 0x1008040201000000, 0x2010080402010000,
            0x4020100804020100, 0x8040201008040201, 0x0080402010080402, 0x0000804020100804,
            0x0201000000000000, 0x0402010000000000, 0x0804020100000000, 0x1008040201000000,
            0x2010080402010000, 0x4020100804020100, 0x8040201008040201, 0x0080402010080402,
            0x0100000000000000, 0x0201000000000000, 0x0402010000000000, 0x0804020100000000,
            0x1008040201000000, 0x2010080402010000, 0x4020100804020100, 0x8040201008040201
        };

        public static readonly ulong[] AntiDiagonals =
        {
            0x0000000000000001, 0x0000000000000102, 0x0000000000010204, 0x0000000001020408,
            0x0000000102040810, 0x0000010204081020, 0x0001020408102040, 0x0102040810204080,
            0x0000000000000102, 0x0000000000010204, 0x0000000001020408, 0x0000000102040810,
            0x0000010204081020, 0x0001020408102040, 0x0102040810204080, 0x0204081020408000,
            0x0000000000010204, 0x0000000001020408, 0x0000000102040810, 0x0000010204081020,
            0x0001020408102040, 0x0102040810204080, 0x0204081020408000, 0x0408102040800000,
            0x0000000001020408, 0x0000000102040810, 0x0000010204081020, 0x0001020408102040,
            0x0102040810204080, 0x0204081020408000, 0x0408102040800000, 0x0810204080000000,
            0x0000000102040810, 0x0000010204081020, 0x0001020408102040, 0x0102040810204080,
            0x0204081020408000, 0x0408102040800000, 0x0810204080000000, 0x1020408000000000,
            0x0000010204081020, 0x0001020408102040, 0x0102040810204080, 0x0204081020408000,
            0x0408102040800000, 0x0810204080000000, 0x1020408000000000, 0x2040800000000000,
            0x0001020408102040, 0x0102040810204080, 0x0204081020408000, 0x0408102040800000,
            0x0810204080000000, 0x1020408000000000, 0x2040800000000000, 0x4080000000000000,
            0x0102040810204080, 0x0204081020408000, 0x0408102040800000, 0x0810204080000000,
            0x1020408000000000, 0x2040800000000000, 0x4080000000000000, 0x8000000000000000
        };

        //returns the index of the least significant bit of the bitboard, bb can't be 0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LSB(ulong bb) => BitOperations.TrailingZeroCount(bb);

        //resets the least significant bit of the bitboard
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ClearLSB(ulong bb) => bb & (bb - 1);
        //public static ulong ClearLSB(ulong bb) => Bmi1.X64.ResetLowestSetBit(bb);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int PopCount(ulong bb) => BitOperations.PopCount(bb);

        const ulong HORIZONTAL = 0x00000000000000FFUL;
        const ulong VERTICAL = 0x0101010101010101UL;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetBishopAttacks(ulong occupation, int square)
        {
            ulong bbPiece = 1UL << square;
            ulong bbBlocker = occupation & ~bbPiece;
            //mask the bits below bbPiece
            ulong bbBelow = bbPiece - 1;
            return GenLines(Diagonals[square], AntiDiagonals[square], bbBlocker, bbBelow);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetRookAttacks(ulong occupation, int square)
        {
            ulong bbPiece = 1UL << square;
            ulong bbBlocker = occupation & ~bbPiece;
            //mask the bits below bbPiece
            ulong bbBelow = bbPiece - 1;
            //horizontal line through square
            ulong bbHorizontal = HORIZONTAL << (square & 56);
            //vertical line through square
            ulong bbVertical = VERTICAL << (square & 7);
            return GenLines(bbHorizontal, bbVertical, bbBlocker, bbBelow);
        }


        public enum SliderGeneration { Leorik, KiSS, PEXT }
#if PEXT
        public const SliderGeneration SliderMode = SliderGeneration.PEXT;
#elif KISS
        public const SliderGeneration SliderMode = SliderGeneration.KiSS;
#else
        public const SliderGeneration SliderMode = SliderGeneration.Leorik;
#endif


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetBishopTargets(ulong occupation, int square)
        {
#if PEXT
            return Pext.Attacks[Pext.BishopOffset[square] + Bmi2.X64.ParallelBitExtract(occupation, Pext.BishopMask[square])];
#elif KISS
            return KiSS.BishopAttacks(occupation, square);
#else
            return GetBishopAttacks(occupation, square);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetRookTargets(ulong occupation, int square)
        {
#if PEXT
            return Pext.Attacks[Pext.RookOffset[square] + Bmi2.X64.ParallelBitExtract(occupation, Pext.RookMask[square])];
#elif KISS
            return KiSS.RookAttacks(occupation, square);
#else
            return GetRookAttacks(occupation, square);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetQueenTargets(ulong occupation, int square) =>
            GetBishopTargets(occupation, square) |
            GetRookTargets(occupation, square);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong GenLines(ulong bbLineA, ulong bbLineB, ulong bbBlocker, ulong bbBelow) =>
            GenLine(bbLineA, bbBlocker & bbLineA, bbBelow) |
            GenLine(bbLineB, bbBlocker & bbLineB, bbBelow);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong GenLine(ulong bbLine, ulong bbBlocker, ulong bbBelow)
        {
            //MaskLow sets all low bits up to and including the lowest blocker above orgin, the rest are zeroed out.
            //MaskHigh sets all low bits up to and including the highest blocker below origin, the rest are zerored out.
            //The bits of the line that are different between the two masks are the valid targets (including the first blockers on each side)
            return (MaskLow(bbBlocker & ~bbBelow) ^ MaskHigh(bbBlocker & bbBelow)) & bbLine;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //identify the highest set bit and shift a mask so the bits below are set and the rest are zeroed
        private static ulong MaskHigh(ulong bb) => 0x7FFFFFFFFFFFFFFFUL >> BitOperations.LeadingZeroCount(bb | 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //identify the lowest set bit and set all bits below while zeroing the rest
        private static ulong MaskLow(ulong bb) => bb ^ (bb - 1);

        //******************************
        //*** ZERO TABLE ALTERNATIVE ***
        //******************************

        //const ulong DIAGONAL = 0x8040201008040201UL;
        //const ulong ANTIDIAGONAL = 0x0102040810204080UL;
        //
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        ////sign of 'ranks' decides between left shift or right shift. Then convert signed ranks to a positiver number of bits to shift by. Each rank has 8 bits e.g. 1 << 3 == 8
        //private static ulong VerticalShift(in ulong bb, in int ranks) => ranks > 0 ? bb >> (ranks << 3) : bb << -(ranks << 3);
        //
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static ulong GetBishopTargets(ulong occupation, int square)
        //{
        //    ulong bbPiece = 1UL << square;
        //    ulong bbBlocker = occupation & ~bbPiece;
        //    //mask the bits below bbPiece
        //    ulong bbBelow = bbPiece - 1;
        //    //compute rank and file of square
        //    int rank = square >> 3;
        //    int file = square & 7;
        //    //diagonal line through square
        //    ulong bbDiagonal = VerticalShift(DIAGONAL, file - rank);
        //    //antidiagonal line through square
        //    ulong bbAntiDiagonal = VerticalShift(ANTIDIAGONAL, 7 - file - rank);
        //    return GenLines(bbDiagonal, bbAntiDiagonal, bbBlocker, bbBelow);
        //}
    }
}
