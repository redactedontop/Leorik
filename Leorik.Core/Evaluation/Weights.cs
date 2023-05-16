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
        public static readonly short[] PhaseValues = new short[6] { 0, 237, 266, 469, 1372, 0 };

        public static readonly (short, short)[] Features = new (short, short)[10 * 64]
        {
            //Pawns
            ( 100,   0), ( 100,   0), ( 100,   0), ( 100,   0), ( 100,   0), ( 100,   0), ( 100,   0), ( 100,   0),
            ( 232, 110), ( 191, 135), ( 210, 102), ( 221,  65), ( 183,  72), ( 186, 100), ( 160, 156), ( 160, 177),
            ( 105, 169), ( 127, 152), ( 117, 114), ( 158,  43), ( 167,  41), ( 212,  -7), ( 224,  75), ( 171, 101),
            ( 114, 126), ( 137, 115), ( 108,  99), ( 130,  63), ( 134,  74), ( 134,  76), ( 138,  88), ( 127, 118),
            (  98, 112), ( 112, 117), ( 120,  69), ( 115,  64), ( 134,  84), ( 130,  76), ( 134,  79), ( 124,  76),
            (  97, 104), (  99,  90), (  92,  93), ( 101, 105), ( 103, 100), ( 110,  91), ( 126,  58), ( 120,  59),
            (  93, 110), ( 106, 113), ( 103,  96), ( 106, 112), ( 118, 123), ( 148,  74), ( 162,  21), ( 122,  52),
            ( 100,   0), ( 100,   0), ( 100,   0), ( 100,   0), ( 100,   0), ( 100,   0), ( 100,   0), ( 100,   0),
            //Knights
            ( 293, 186), ( 465, 161), ( 478, 172), ( 584,  43), ( 566,  73), ( 499, 131), ( 477, 121), ( 339,  54),
            ( 451, 158), ( 469, 199), ( 512, 178), ( 577, 127), ( 542, 148), ( 588,  41), ( 472, 151), ( 499, 113),
            ( 448, 220), ( 504, 212), ( 534, 202), ( 566, 193), ( 658,  45), ( 644,  52), ( 546, 155), ( 491, 148),
            ( 468, 197), ( 491, 213), ( 529, 225), ( 549, 225), ( 526, 244), ( 559, 211), ( 515, 207), ( 518, 140),
            ( 457, 189), ( 482, 206), ( 511, 230), ( 503, 255), ( 522, 240), ( 519, 221), ( 517, 193), ( 474, 174),
            ( 420, 188), ( 476, 212), ( 488, 221), ( 497, 240), ( 508, 220), ( 499, 202), ( 495, 192), ( 460, 164),
            ( 417, 151), ( 439, 182), ( 459, 181), ( 478, 191), ( 479, 193), ( 469, 177), ( 439, 172), ( 446, 128),
            ( 378,  57), ( 441, 165), ( 433, 165), ( 448, 190), ( 448, 190), ( 446, 159), ( 427,  96), ( 377,  61),
            //Bishops
            ( 515, 242), ( 505, 281), ( 520, 255), ( 504, 262), ( 490, 285), ( 503, 264), ( 558, 223), ( 549, 195),
            ( 490, 259), ( 521, 255), ( 527, 230), ( 533, 240), ( 544, 231), ( 527, 244), ( 502, 281), ( 504, 201),
            ( 516, 240), ( 539, 240), ( 545, 226), ( 566, 201), ( 590, 178), ( 635, 165), ( 588, 182), ( 555, 201),
            ( 515, 230), ( 527, 229), ( 541, 239), ( 571, 226), ( 553, 258), ( 553, 228), ( 530, 239), ( 526, 226),
            ( 523, 217), ( 529, 236), ( 535, 241), ( 551, 254), ( 551, 240), ( 529, 240), ( 540, 213), ( 534, 202),
            ( 526, 232), ( 545, 225), ( 534, 239), ( 538, 242), ( 534, 246), ( 538, 220), ( 545, 222), ( 539, 207),
            ( 531, 199), ( 527, 220), ( 536, 214), ( 519, 238), ( 527, 214), ( 532, 187), ( 545, 190), ( 519, 175),
            ( 512, 244), ( 532, 180), ( 523, 221), ( 514, 218), ( 524, 213), ( 509, 224), ( 538, 180), ( 487, 205),
            //Rooks
            ( 761, 499), ( 760, 500), ( 789, 469), ( 793, 466), ( 818, 428), ( 840, 410), ( 822, 414), ( 836, 393),
            ( 744, 522), ( 732, 552), ( 778, 507), ( 806, 483), ( 807, 473), ( 863, 357), ( 827, 398), ( 864, 354),
            ( 725, 547), ( 744, 517), ( 744, 526), ( 774, 476), ( 816, 426), ( 859, 371), ( 852, 377), ( 806, 419),
            ( 713, 533), ( 714, 519), ( 737, 513), ( 743, 504), ( 735, 496), ( 771, 469), ( 773, 456), ( 738, 480),
            ( 691, 527), ( 687, 548), ( 712, 511), ( 705, 516), ( 715, 496), ( 715, 507), ( 738, 463), ( 699, 493),
            ( 675, 531), ( 696, 504), ( 687, 521), ( 698, 489), ( 711, 478), ( 708, 478), ( 735, 440), ( 705, 452),
            ( 678, 487), ( 690, 482), ( 703, 485), ( 708, 487), ( 712, 462), ( 700, 472), ( 727, 400), ( 669, 471),
            ( 705, 485), ( 707, 485), ( 721, 485), ( 729, 468), ( 729, 459), ( 724, 466), ( 703, 485), ( 708, 428),
            //Queens
            (1605, 494), (1648, 486), (1657, 453), (1674, 455), (1683, 488), (1705, 440), (1697, 428), (1672, 432),
            (1608, 506), (1614, 556), (1633, 582), (1655, 576), (1658, 619), (1694, 489), (1639, 578), (1685, 496),
            (1616, 431), (1624, 475), (1634, 580), (1675, 517), (1694, 553), (1754, 441), (1716, 462), (1702, 507),
            (1604, 442), (1624, 490), (1625, 536), (1645, 608), (1659, 595), (1668, 558), (1670, 550), (1664, 538),
            (1604, 395), (1617, 475), (1628, 463), (1637, 484), (1640, 525), (1643, 494), (1655, 483), (1647, 506),
            (1597, 423), (1617, 390), (1618, 457), (1620, 437), (1623, 409), (1636, 447), (1651, 449), (1629, 383),
            (1596, 371), (1608, 387), (1615, 378), (1619, 370), (1622, 382), (1622, 288), (1605, 256), (1571, 450),
            (1602, 359), (1591, 314), (1602, 325), (1629, 401), (1610, 306), (1577, 284), (1535, 329), (1574, 342),
            //Kings
            ( -62, -78), (  21,  -5), (  35, -17), (  34,  19), (-161, 248), (  60, -21), (   3, -16), ( -20, -43),
            (  -5, -39), (  62,   3), (  77,  26), (  72,  57), (  78,  71), (   5, 152), (  71,  51), (  15, -22),
            (   3,   8), (  58,  29), (  66,  58), (  44,  86), (  94,  43), ( 101,  52), (  77,  54), (   8,   0),
            ( -48,  25), (  37,  39), ( -61, 180), (  23,  94), (  46,  63), (  16,  97), (   3,  91), ( -39,  15),
            ( -18, -42), (  14,  16), (  11,  53), ( -36, 122), ( -48, 132), ( -61, 134), ( -28,  68), ( -89,  41),
            (  -7, -69), (  47, -56), (   5,  24), ( -52, 115), ( -55, 109), ( -69, 109), (   7, -24), ( -18, -69),
            (  57,-194), (  31, -77), (  40, -51), ( -17,  31), ( -21,  25), (   8, -12), (  73,-132), (  71,-214),
            ( -36,-172), (  54,-203), (  46,-159), ( -91, -10), (  39,-180), (  -2,-118), (  80,-233), (  52,-285),
            //Isolated Pawns
            (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0),
            (   9, -43), (  75, -94), (  23, -58), (  11, -36), (  69, -42), (  12,   9), (  -5,   5), ( -32,  21),
            (  19, -44), (   3, -62), (   2, -21), (  28, -38), (  40,  -9), (   4,  28), (  29, -54), (  -5, -24),
            (  -5,  -6), (  -9, -36), (   1, -15), ( -12,  -6), (  -5,   6), (  24, -31), (  20, -53), (  -2, -31),
            (  -9,   0), ( -13,  -6), ( -15,   9), ( -24,  25), ( -25,  -1), ( -23,  11), ( -24,  -3), ( -38,  23),
            ( -10,   6), ( -16,  11), (  -5,   3), (  -8, -11), ( -18,   9), (   9, -22), ( -13,  -1), ( -24,  14),
            (  -8,   4), ( -21, -13), (  -3,  -9), ( -32,   1), ( -27,  20), ( -20,   5), ( -11,   2), ( -12,   0),
            (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0),
            //Passed Pawns
            (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0),
            ( 132, 110), (  91, 135), ( 110, 102), ( 121,  65), (  83,  72), (  86, 100), (  60, 156), (  60, 177),
            ( 125, 178), ( 146, 152), ( 128, 135), (  58, 137), (  65,  82), ( 126,  92), (  52, 178), (  26, 257),
            (  65, 109), (  66,  94), (  55,  77), (  45,  54), (  27,  38), (  52,  60), (  67,  89), (  21, 139),
            (  35,  57), (  23,  60), (   8,  65), (  18,  29), (  10,  27), (  19,  42), (  41,  62), (   9,  96),
            (   4,  27), (  -1,  43), (   3,  16), (  -2,  -1), (   6,   9), (  -6,  40), (  22,  16), (  46, -34),
            (   9,  -8), (  14,  18), (   9,  10), ( -15,   2), (  13, -20), (   7, -16), (  27,  -8), (  34, -34),
            (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0),
            //Protected Pawns
            (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0),
            (  22,  -2), ( 109,  38), (  53,  34), (  75,  16), (  57,  33), (  40,  37), (  45,  46), (  77,  70),
            (  30,  16), (  22,  52), (  77,  -1), ( 106, -11), ( 111,  -9), (  63,  44), (  -1,  37), (  18,   7),
            (  -6,  31), (   5,   2), (  30, -11), (  12,  23), (  42,   6), (  24,   4), (   8,   9), (  -7,   7),
            (   7,  21), (  19,  -2), (  13,  19), (  23,  27), (  16,  14), (   9,  13), (  14,  -4), (   4,   6),
            (   9,   6), (  18,  26), (  25,  13), (  31,  21), (  31,  21), (  21,  21), (  28,  -6), (  20,   4),
            (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0),
            (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0),
            //Connected Pawns
            (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0),
            (  66,  57), (  86,  73), (  49,  41), (  60,  49), (  53,  37), (  35,  22), (  41,  35), (  29,  25),
            (  69,  51), ( 112,  80), (  78,  52), ( 130,  75), ( 110,  69), (  85,  34), ( 143,  72), (  74,  54),
            (   8,  24), (  28,  49), (  37,  35), (  54,  35), (  78,  -6), (  80, -15), (  32,  36), (  28,  13),
            (   3,   0), (  15,  15), (   9,  10), (  19,  39), (  25,   4), (  26,   4), (  15, -18), (   6,  17),
            (   3,  -4), (   7,  21), (  12,  10), (  14, -18), (  21,   2), (  18, -19), (   1,  33), (   8, -30),
            (   4,   2), (   4, -10), (   2,  26), (  16,  -2), (  11,  26), (  -8, -21), (   3,  41), (  -3, -23),
            (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0), (   0,   0),
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
            (-11,-18), (0,0), (0,0), (79,170), (0,0), (0,0), (0,0), (0,0), (0,0), (0,0), (0,0), (0,0), (0,0),
            //Knight:
            (0,0), (0,0), (0,0), (0,0), (0,0), (0,0), (0,0), (0,0), (0,0),
            //Bishop:
            (-56,-57), (-47,-57), (-35,-52), (-30,-34), (-19,-14), (-8,-7), (-4,1), (0,0), (3,2), (9,-3), (8,-9), (16,-14), (47,-56), (46,-69),
            //Rook:
            (-47,-10), (-41,3), (-26,18), (-24,9), (-20,11), (-13,7), (-8,8), (0,0), (9,2), (16,-2), (22,2), (30,-3), (40,-6), (55,-38), (47,-23),
            //Queen:
            (-147,-492), (-85,-262), (-66,-219), (-57,-196), (-32,-89), (-33,-98), (-30,-82), (-23,-69), (-19,-50), (-13,-38), (-9,-40), (-7,-30), (-1,-6), (0,-6), (0,0), (5,25), (4,32), (3,38), (6,40), (12,27), (20,18), (22,-9), (41,-44), (48,-66), (45,-93), (27,-96), (-144,-183), (-156,-166),
            //King:
            (3,-18), (15,34), (11,13), (9,1), (0,0), (-4,-18), (-14,-12), (-44,7), (-74,-7),
        };
    }
}
