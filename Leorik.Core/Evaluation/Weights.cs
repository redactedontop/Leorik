﻿namespace Leorik.Core
{
    public static class Weights
    {
        /*
            Rebalanced so that immobile pieces receive no mobility bonus (0, 0)

            Varied.bin Openings:
            Score of Leorik-2.4.1 vs Leorik-2.4-Pext-Net8: 3081 - 2718 - 4201  [0.518] 10000
            ...      Leorik-2.4.1 playing White: 1657 - 1162 - 2181  [0.549] 5000
            ...      Leorik-2.4.1 playing Black: 1424 - 1556 - 2020  [0.487] 5000
            ...      White vs Black: 3213 - 2586 - 4201  [0.531] 10000
            Elo difference: 12.6 +/- 5.2, LOS: 100.0 %, DrawRatio: 42.0 %

            Titans.bin Openings:
            Score of Leorik-2.4.1 vs Leorik-2.4-Pext-Net8: 2592 - 2274 - 3642  [0.519] 8508
            ...      Leorik-2.4.1 playing White: 1450 - 988 - 1816  [0.554] 4254
            ...      Leorik-2.4.1 playing Black: 1142 - 1286 - 1826  [0.483] 4254
            ...      White vs Black: 2736 - 2130 - 3642  [0.536] 8508
            Elo difference: 13.0 +/- 5.6, LOS: 100.0 %, DrawRatio: 42.8 %

            Changed Rebalancing didn't hurt:
            Score of Leorik-2.4.2 vs Leorik-2.4-Pext-Net8: 814 - 675 - 1068  [0.527] 2557
            ...      Leorik-2.4.2 playing White: 469 - 275 - 534  [0.576] 1278
            ...      Leorik-2.4.2 playing Black: 345 - 400 - 534  [0.478] 1279
            ...      White vs Black: 869 - 620 - 1068  [0.549] 2557
            Elo difference: 18.9 +/- 10.3, LOS: 100.0 %, DrawRatio: 41.8 %
        */
        public static readonly short[] PhaseValues = new short[6] { 0, 257, 289, 488, 1100, 0 };

        public static readonly (short, short)[] Features = new (short, short)[15 * 64]
        {
            //Pawns
            ( 100,   0), ( 100,   0), ( 100,   0), ( 100,   0), ( 100,   0), ( 100,   0), ( 100,   0), ( 100,   0),
            ( 226, 153), ( 192, 167), ( 211, 136), ( 227,  81), ( 181,  97), ( 178, 134), ( 153, 195), ( 145, 222),
            (  99, 228), ( 123, 201), ( 119, 153), ( 156,  68), ( 171,  77), ( 223,  21), ( 226, 113), ( 172, 142),
            ( 111, 173), ( 137, 158), ( 109, 146), ( 133,  90), ( 138, 109), ( 139, 109), ( 143, 133), ( 127, 162),
            ( 100, 152), ( 110, 157), ( 123, 101), ( 123,  91), ( 139, 118), ( 135, 107), ( 139, 113), ( 126, 110),
            (  96, 138), ( 101, 127), (  92, 129), ( 101, 149), ( 107, 139), ( 113, 124), ( 137,  83), ( 126,  89),
            (  94, 153), ( 107, 153), ( 104, 139), ( 104, 150), ( 117, 172), ( 152, 115), ( 178,  48), ( 127,  79),
            ( 100,   0), ( 100,   0), ( 100,   0), ( 100,   0), ( 100,   0), ( 100,   0), ( 100,   0), ( 100,   0),
            //Knights
            ( 322, 216), ( 480, 213), ( 503, 204), ( 634,  18), ( 585,  91), ( 527, 156), ( 514, 148), ( 358,  89),
            ( 458, 210), ( 477, 237), ( 518, 220), ( 597, 160), ( 550, 187), ( 619,  47), ( 487, 187), ( 499, 175),
            ( 451, 270), ( 511, 258), ( 541, 252), ( 577, 236), ( 680,  83), ( 676,  71), ( 567, 203), ( 494, 193),
            ( 478, 236), ( 500, 254), ( 533, 273), ( 553, 280), ( 530, 300), ( 564, 266), ( 522, 254), ( 531, 181),
            ( 465, 221), ( 491, 255), ( 516, 276), ( 506, 304), ( 527, 291), ( 521, 274), ( 524, 260), ( 482, 212),
            ( 426, 207), ( 483, 248), ( 495, 261), ( 501, 288), ( 515, 267), ( 505, 244), ( 502, 231), ( 469, 211),
            ( 425, 185), ( 447, 219), ( 467, 230), ( 484, 228), ( 488, 233), ( 476, 216), ( 446, 211), ( 457, 153),
            ( 398,  90), ( 447, 191), ( 442, 209), ( 459, 229), ( 458, 223), ( 457, 187), ( 437, 120), ( 386, 110),
            //Bishops
            ( 520, 276), ( 502, 312), ( 518, 294), ( 507, 290), ( 464, 359), ( 507, 291), ( 580, 235), ( 574, 205),
            ( 501, 284), ( 525, 285), ( 531, 265), ( 536, 267), ( 553, 249), ( 553, 242), ( 500, 338), ( 510, 210),
            ( 525, 260), ( 544, 266), ( 553, 253), ( 575, 227), ( 605, 188), ( 651, 181), ( 602, 200), ( 566, 227),
            ( 528, 243), ( 537, 253), ( 549, 268), ( 585, 250), ( 559, 294), ( 561, 261), ( 540, 265), ( 532, 249),
            ( 538, 228), ( 540, 251), ( 544, 268), ( 562, 280), ( 562, 268), ( 536, 262), ( 549, 238), ( 545, 225),
            ( 534, 254), ( 554, 246), ( 544, 260), ( 546, 263), ( 544, 269), ( 551, 243), ( 552, 245), ( 550, 229),
            ( 539, 218), ( 536, 226), ( 545, 225), ( 527, 254), ( 536, 231), ( 543, 200), ( 555, 201), ( 529, 173),
            ( 522, 255), ( 544, 203), ( 532, 239), ( 523, 237), ( 532, 228), ( 516, 235), ( 549, 191), ( 497, 206),
            //Rooks
            ( 758, 561), ( 755, 559), ( 797, 516), ( 804, 508), ( 831, 465), ( 864, 436), ( 837, 449), ( 835, 448),
            ( 735, 591), ( 724, 626), ( 768, 580), ( 805, 538), ( 804, 537), ( 878, 390), ( 845, 433), ( 876, 387),
            ( 710, 623), ( 733, 591), ( 735, 598), ( 770, 536), ( 821, 478), ( 866, 416), ( 859, 419), ( 807, 464),
            ( 703, 602), ( 705, 588), ( 725, 578), ( 736, 565), ( 730, 563), ( 761, 545), ( 764, 518), ( 743, 524),
            ( 679, 596), ( 673, 628), ( 703, 574), ( 697, 588), ( 708, 563), ( 705, 577), ( 735, 524), ( 689, 562),
            ( 661, 603), ( 688, 566), ( 684, 582), ( 691, 548), ( 708, 536), ( 702, 545), ( 734, 491), ( 697, 514),
            ( 674, 547), ( 684, 538), ( 698, 541), ( 702, 543), ( 707, 522), ( 695, 528), ( 725, 449), ( 669, 531),
            ( 697, 545), ( 698, 545), ( 714, 544), ( 721, 529), ( 728, 516), ( 718, 515), ( 700, 534), ( 699, 493),
            //Queens
            (1667, 553), (1702, 546), (1722, 506), (1748, 488), (1744, 554), (1788, 474), (1776, 461), (1743, 464),
            (1665, 541), (1665, 596), (1684, 632), (1699, 651), (1705, 699), (1753, 565), (1672, 678), (1734, 546),
            (1678, 467), (1683, 523), (1684, 625), (1732, 582), (1749, 616), (1827, 482), (1782, 538), (1761, 567),
            (1670, 473), (1680, 540), (1681, 584), (1691, 664), (1711, 644), (1716, 632), (1722, 614), (1723, 601),
            (1672, 429), (1677, 519), (1690, 506), (1699, 545), (1698, 577), (1701, 545), (1713, 522), (1705, 556),
            (1668, 454), (1688, 429), (1683, 501), (1683, 482), (1692, 461), (1703, 492), (1715, 487), (1698, 425),
            (1668, 406), (1677, 428), (1687, 419), (1692, 409), (1693, 425), (1700, 321), (1687, 266), (1637, 520),
            (1673, 402), (1669, 338), (1678, 355), (1692, 425), (1688, 333), (1661, 289), (1616, 335), (1666, 357),
            //Kings
            ( -63, -99), (   8, -29), (  23, -56), (  -3, -14), (-155, 220), (  43, -56), (  -6, -37), ( -16, -57),
            ( -29, -75), (  23, -66), (  44, -18), (  52,  43), (  48,  51), ( -31, 153), (  42,  28), (  27, -26),
            ( -19, -29), (  50, -13), (  51,  30), (   6,  80), (  72,   4), (  89,  36), (  77,  55), (  10,  -8),
            ( -62,  12), (  34,  18), ( -69, 166), (  11,  83), (  34,  78), (  24, 103), (   5, 118), ( -35,  20),
            ( -23, -64), (  20,  -3), (   2,  48), ( -42, 128), ( -78, 152), ( -77, 165), ( -22,  62), ( -73,  48),
            (   6,-110), (  84, -60), (  22,  28), ( -73, 135), ( -97, 179), ( -96, 144), (   5,  27), (  21, -55),
            ( 113,-228), ( 104, -92), (  76, -33), ( -22,  98), ( -52, 117), ( -52, 116), (  81, -72), ( 134,-195),
            (  -4,-197), ( 159,-252), ( 115,-209), ( -66,  51), (  -4,-105), ( -45,  20), ( 184,-281), ( 114,-277),
            //Isolated Pawns
            (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0),
            (  17, -57), (  98,-130), (  35, -94), (  19, -48), ( 102, -81), (   9,  20), (  -6,   9), ( -38,  28),
            (  28, -66), (  16, -92), (   3, -23), (  37, -46), (  55, -33), (  -4,  43), (  47, -79), (   5, -37),
            (  -3,  -8), (  -6, -46), (   3, -27), ( -13,  -2), (  -7,   5), (  29, -39), (  28, -68), (   1, -43),
            (  -8,   1), ( -14,  -6), ( -18,  13), ( -26,  30), ( -26,  -8), ( -26,  18), ( -30,   2), ( -40,  22),
            ( -12,  11), ( -21,  18), (  -6,   1), (  -5, -14), ( -17,   6), (   4, -16), ( -18,   5), ( -26,  14),
            ( -10,   0), ( -22, -18), (  -8, -11), ( -32,   3), ( -29,  23), ( -20,  -1), ( -11,  -5), ( -11,  -3),
            (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0),
            //Passed Pawns
            (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0),
            ( 126, 153), (  92, 167), ( 111, 136), ( 127,  81), (  82,  97), (  78, 134), (  53, 195), (  46, 222),
            ( 106, 229), ( 124, 204), ( 114, 171), (  46, 179), (  51, 108), ( 120, 106), (  13, 234), ( -21, 335),
            (  54, 134), (  60, 116), (  50,  94), (  43,  65), (  26,  44), (  44,  70), (  16, 159), (   4, 170),
            (  30,  67), (  19,  68), (   3,  73), (  19,  33), (   8,  33), (  20,  46), (  39,  57), (  23,  74),
            (   3,  30), (  -4,  52), (   5,  16), (   5, -11), (   5,   7), (   0,  20), (  26,   8), (  51, -44),
            (   8,  -4), (  11,  24), (  12,  12), ( -13,   1), (  18, -27), (   6, -25), (  38, -29), (  35, -38),
            (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0),
            //Protected Pawns
            (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0),
            (  19, -18), ( 138,  27), (  54,  25), (  98,   4), (  73,  43), (  41,  43), (  47,  62), ( 119, 109),
            (  31,  14), (  20,  63), (  86,  -9), ( 131, -35), ( 139, -37), (  60,  55), (   2,  38), (  25,   6),
            (  -9,  38), (   5,   6), (  34,  -9), (   9,  33), (  38,  11), (  24,   9), (  11,  -1), (  -5,   5),
            (   3,  25), (  18,   5), (  13,  24), (  21,  36), (  14,  19), (   9,  19), (  16,  -1), (   3,  11),
            (   8,  11), (  16,  32), (  24,  21), (  31,  23), (  30,  27), (  21,  26), (  30,   0), (  17,  11),
            (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0),
            (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0),
            //Connected Pawns
            (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0),
            ( 117,  97), ( 146, 119), (  82,  70), ( 103,  88), (  89,  66), (  65,  46), (  69,  59), (  44,  38),
            (  82,  76), ( 116, 113), (  82,  75), ( 162, 107), ( 128,  91), (  98,  30), ( 189,  78), ( 107,  68),
            (   3,  28), (  27,  59), (  34,  39), (  52,  49), (  72,   2), (  92, -25), (  27,  44), (  33,  17),
            (   4,   5), (   9,  16), (  12,  17), (  11,  44), (  21,   5), (  28,  16), (  14, -25), (   4,  34),
            (   0,   5), (   6,  20), (  11,  14), (  16, -13), (  21,   5), (  19, -24), (   2,  40), (   5, -29),
            (   5,   2), (   3, -15), (   1,  30), (  17,  -4), (  10,  28), (  -5, -27), (  -2,  50), (  -3, -28),
            (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0),
            //KingRelative-Pawns
            (   2,  -1), (  21,  14), (  21,  11), (  28,  23), (  17,  11), (  24,  10), (   7,   5), (  -6,  -6),
            (  19,  16), (  34,  25), (  22,  22), (  19,  16), (  20,  19), ( -12,  46), (  16,  17), (  -2,   1),
            (  12,   9), (  16,   6), (  13,  13), (  16,  17), (  17,  20), (  13,  11), (  12,   0), (  -2,   2),
            ( -11,  18), (   3,   7), ( -10,  21), (  18, -12), ( -38,  44), (  -7,  -1), ( -18,   8), ( -17,   5),
            (  -5,  11), ( -13,  16), ( -10,  12), ( -11,   4), ( -30,  25), ( -21,   5), ( -32,  20), ( -17,  -6),
            ( -11,   4), ( -15,   0), ( -20,  11), ( -19,  11), (  -7, -13), ( -16,  -4), ( -12, -19), ( -17, -16),
            ( -10,  -5), ( -16,   1), ( -14,  -7), (   0, -20), (  -1, -28), (   5, -37), (  -8, -26), (  -9, -33),
            (  -8, -13), ( -24,  -1), ( -18,   5), (  -4, -17), (   5, -19), (   6, -39), ( -10, -22), ( -10, -29),
            //KingRelative-Knights
            (  -1,  -1), (  -3,  -2), (  -3,  -3), (  -1,   0), (   0,   0), (   5,   1), (  -2,  -1), (  -2,  -2),
            (  -1,  -1), (  -6,  -5), (  -9,  -7), (  -8,  -6), (  -5,  -5), ( -19,   3), (   1,   1), (   0,   0),
            (  -5,  -4), (  -6,  -5), (  -4,  -4), (  -9,  -8), (  -8,  -6), (  -4,  -4), (  -6, -10), (  -4,  -4),
            (  -6,   3), (   0,  -3), (  -8,   1), (  18, -11), (  -4,  15), (   9,  -5), (  -6,  -3), (  -1,   0),
            (   1,   7), (   2,   5), (   2,   3), (   6,   0), (  11,  16), (  10,   9), (   4,  24), (  15,   8),
            (   3,  10), (   4,   3), (   8,  -6), (  11,  19), (  22,   7), (  13,  26), (  16,  13), (  11,   4),
            ( -16,   7), ( -13,   7), (   8,  15), (  12,   5), (  13,  30), (  -1,  42), (   4,  27), ( -12,  39),
            (  -2,  -1), (   0,   5), (   4,  -3), (  19,  -6), (  -3,  20), (   8,  14), ( -17,  46), ( -18,  42),
            //KingRelative-Bishops
            (  -1,  -1), (  -1,  -1), (   0,   0), (  -2,  -1), (   3,   3), (   1,  -2), (   2,   1), (  -1,  -1),
            (  -7,  -5), (   5,   2), (  -1,   1), (   1,   1), (  -1,   0), ( -18,   4), (   2,   0), (  -7,  -7),
            (   1,   0), (  -4,  -3), (  -3,  -2), (  -5,  -3), (   1,   1), (  -4,  -5), ( -12, -15), (  -7,  -7),
            ( -15,  -3), (  -4,  -3), (  -4,   6), (  14, -15), (  -8,  14), (   9,  -2), (   2,   3), (  -2,  -2),
            ( -12,  -4), (  -6,   0), (   3,   4), (   6,   5), (  11,   8), (  13,   8), (   7,  24), (  -1,  -1),
            (   0,   6), (  -2,  -2), (   3,  -4), (   4,   4), (  15,  -3), (  11,  16), (   2,  11), (  -1,  10),
            ( -15,   3), (  -3,  -4), (  16,  -5), (  12, -18), (   9,   5), (  17,  -1), (  -8,  23), ( -15,  23),
            (  -3,  -6), (  -7,  18), (   6,   8), (   1, -13), (   0,  29), (  16, -13), ( -16,  32), ( -16,  40),
            //KingRelative-Rooks
            (   2,   0), (  -3,  -3), (  -4,  -4), (  -8,  -6), (  -6,  -5), (   6,   2), (   4,   1), (   2,   1),
            (  -6,  -6), ( -10, -11), ( -15, -14), (  -7,  -4), (  -7,  -3), (  -9,  10), (  -9,  -8), (  -5,  -5),
            (  -7,  -5), (  -4,  -7), (  -2,  -2), (  -5,  -3), (  -1,   3), (  -2,  -1), (  -6,  -7), (  -1,  -2),
            (   2,   9), (  -6,  -3), (  -2,  10), (   8, -12), (  -1,  20), (   8,   3), (   5,  11), (  10,  10),
            (   2,   7), (  -5,   6), (  -1,  11), (   7,  16), (  17,  31), (  18,  25), (  11,  33), (  13,  17),
            (  10,  14), (   3,   1), (   2,  16), (   7,  30), (  -8,  34), (  10,  45), (   8,  30), (  14,  12),
            (  -4,   7), ( -15,   9), ( -16,  11), ( -34,  33), ( -14,  26), (  -9,  29), (   1,  30), (  -8,  22),
            (   8,   0), (  -3,  -5), (  -7, -10), ( -23,   0), (   6, -40), ( -13,  10), (  -9,   5), (  10,  -4),
            //KingRelative-Queens
            (   3,   2), (   4,   2), (   7,   4), (   2,   2), (   7,   4), (   1,   0), (   0,  -1), (   4,   2),
            (   5,   3), (   9,   5), (   6,   4), (   1,   1), (   0,   0), (  -7,   3), (  -4,  -2), (   3,   1),
            (   7,   4), (   9,   4), (   4,   2), (  -5,  -2), (   6,   4), (   0,   1), (   3,   0), (   4,   2),
            (   2,   4), (   8,   4), (   2,   5), (   0,  -7), (  -6,   9), (  -5,  -5), (  -8,  -1), (  -1,   1),
            (   7,   5), (   9,   9), (   3,   3), (   1,   1), ( -11,   1), ( -11,   3), (   8,  17), (   1,   8),
            (  13,   8), (  21,  11), (   9,   0), (   7,  11), (   1,   3), (   7,  17), (  10,  18), (  20,  14),
            (  37,  13), (  44,  31), (  25,  11), (  14,   5), (   9,  11), (   0,  20), (  33,  31), (  31,  23),
            (  17,   5), (  48,  16), (  37,   6), (   7,  -4), ( -26,  -9), (  -8,   8), (  12,  -3), (  21,  -1),
        };

        //Max possible moves:
        //Pawn      = 12 + zero = 13  [00..12]
        //Knight    =  8 + zero =  9  [13..21]
        //Bishop    = 13 + zero = 14  [22..35]
        //Rook      = 14 + zero = 15  [36..50]
        //Queen     = 27 + zero = 28  [51..78]
        //King      =  8 + zero =  9  [79..87]
        //--------------
        //TOTAL     = 82        = 88
        //static short[] PieceMobilityIndices = new short[8] { 0, 0, 13, 22, 36, 51, 79, 88 };

        public static (short, short)[] Mobility = new (short, short)[88]
        {
            //Pawn:
            (-10,-19), (0,0), (0,0), (52,191), (0,0), (0,0), (0,0), (0,0), (0,0), (0,0), (0,0), (0,0), (0,0),
            //Knight:
            (0,0), (0,0), (0,0), (0,0), (0,0), (0,0), (0,0), (0,0), (0,0),
            //Bishop:
            (-46,-45), (-37,-35), (-24,-33), (-18,-9), (-8,12), (0,21), (4,32), (8,33), (11,34), (16,29), (15,19), (27,15), (59,-38), (66,-58),
            //Rook:
            (-55,28), (-44,56), (-34,79), (-28,73), (-24,78), (-17,77), (-10,75), (-3,66), (6,68), (11,67), (18,68), (27,65), (38,61), (60,16), (47,38),
            //Queen:
            (-80,-488), (-36,-234), (-22,-205), (-15,-170), (4,-58), (2,-75), (6,-51), (11,-43), (13,-29), (18,-8), (21,-7), (24,2), (27,23), (30,20), (31,24), (31,56), (27,67), (30,70), (33,65), (38,67), (49,51), (59,15), (79,-21), (90,-41), (96,-70), (79,-73), (-101,-164), (-102,-139),
            //King:
            (2,-15), (9,56), (8,31), (3,16), (-5,7), (-5,-26), (-10,-23), (-40,-9), (-59,-38),
        };
    }
}
