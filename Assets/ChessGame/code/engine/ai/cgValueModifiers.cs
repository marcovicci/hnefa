
using System.Collections;
using System.Collections.Generic;
using System;
/// <summary>
/// The following values dictate the playing and searching style of the engine greatly.
/// </summary>
public class cgValueModifiers {

    #region Piece values
    /// <summary>
    /// The value of a pawn, used by the engine to evaluate board states.
    /// </summary>
    public const int Value_Pawn = 1000;

    /// <summary>
    /// The value of a rook, used by the engine to evaluate board states.
    /// </summary>
    public const int Value_Rook = 5000;

    /// <summary>
    /// The value of a knight, used by the engine to evaluate board states.
    /// </summary>
    public const int Value_Knight = 3000;

    /// <summary>
    /// The value of a bishop, used by the engine to evaluate board states. There seems to be a general consensus among chess experts that the bishop, especially in the lategame, is slightly better than the knight hence the slightly better value of a bishop when compared to a knight.
    /// </summary>
    public const int Value_Bishop = 3150;

    /// <summary>
    /// The value of a queen, used by the engine to evaluate board states.
    /// </summary>
    public const int Value_Queen = 9000;

    /// <summary>
    /// The value of a king, used by the engine to evaluate board states. This value should always be arbitrarily high.
    /// </summary>
    public const int Value_King = 50000;

    /// <summary>
    /// How much the right to castle short is valued at.
    /// </summary>
    public const short Value_CastlingShortRights = 22;

    /// <summary>
    /// How much the right to castle long is valued at
    /// </summary>
    public const short Value_CastlingLongRights = 15;

    /// <summary>
    /// How much having castled is valued at.
    /// </summary>
    public const sbyte Value_Castle = 85;

    /// <summary>
    /// A bonus value given for having both bishops. Leading the AI to be less likely to exchange say a bishop for a knight, which otherwise would be seen as an even trade.
    /// </summary>
    public const int Value_BishopPairBonus = 200;
    /// <summary>
    /// A bonus value for having both rooks. Leading the AI to be less likely to exchange a rook for say a bishop + 2 pawns, which otherwise would be seen as an even trade.
    /// </summary>
    public const int Value_RookPairBonus = 200;
    #endregion

    #region AlphaBeta modifiers
    //I recommend NOT altering the AlphaBeta values, these are to increase the pruning speed of the engine, altering these may lead to way longer analysis times.
    public const sbyte AlphaBeta_Weight_Capture = sbyte.MaxValue;
    public const sbyte AlphaBeta_Weight_LongCastle = 30;
    public const sbyte AlphaBeta_Weight_ShortCastle = 30;
    public const sbyte AlphaBeta_Weight_Check = 50;
    public const sbyte AlphaBeta_Strong_Delineation = 20;
    #endregion


    //the following values have not been added yet, but are here as an inspiration for further patches and improvements to the AI.
    private const int _ConnectedRooks = 30;     //If the two rooks connect with each other.
    private const int _RookOnOpenFile = 60;     //If the rook is on a file with no pawns blocking.
    private const int _RookOnSemiOpenFile = 30; //If the rook is on a file with a single pawn blocking
    private const int _KnightOutpost = 50;      //A knight on a file where both enemy pawns on neighbouring files have moved beyond of the knight - making it unkickable by pawn.
    private const int _BishopPawn = 20;         //Bishop guarding and guarded-by a pawn.   Should count heavier if said pawn is base of pawn chain.
    private const int _MultiplePawnFile = -25;  //Counts on each of the pawns(of same color) on the same file
    private const int _PawnChain = 30;          //If a pawn is protecting another pawn
    private const int _RookOppositeQueen = 50;  //if a rook is on the same file or row as an enemy queen.

    #region Positional values
    /// <summary>
    /// The positional value of the bishop on each square.
    /// Notice the high  value for a7 a2 g7 and g2, this means fianchettos are valued high.
    /// </summary>
    public static sbyte[] Positions_Bishop =
    {
    -20,0,  0,  0,  0,  0,  0,  -20,
    0,  22, 5,  5,  5,  5,  22, 0,
    10, 0,  12, 15, 15, 12, 5,  10,
    5,  10, 12, 25, 25, 12, 10, 5,
    5,  10, 12, 25, 25, 12, 10, 5,
    10, 5,  12, 12, 15, 12, 5,  10,
    0,  22, 5 ,  5,  5,  5, 22, 0,
    -20,0,  0,  -10,  0, -10,  0,  -20};

    /// <summary>
    /// The positional value of the rook on each square.
    /// </summary>
    public static sbyte[] Positions_Rook =
    {
    0,  5,  15,  18,  18,  15,  5,  0,
    0,  0,  0,  0,  0,  0,  0,  0,
    0,  0,  0,  0,  0,  0,  0,  0,
    0,  0,  0,  0,  0,  0,  0,  0,
    0,  0,  0,  0,  0,  0,  0,  0,
    0,  0,  0,  0,  0,  0,  0,  0,
    0,  0,  0,  0,  0,  0,  0,  0,
    0,  5,  7,  18,18,  7,  5,  0};

    /// <summary>
    /// The positional value of the knight on each square.
    /// Note: the knight is great in the center.
    /// </summary>
    public static sbyte[] Positions_Knight =
    {
    0,  -5,  0,  0,  0,  0,  -5,  0,
    0,  0,   0,  5,  5,  0,  0,  0,
    3,  5,  25, 25, 25, 25, 5,  3,
    0, 15,  25, 40, 40, 25, 15,  0,
    0, 15,  25, 40, 40, 25, 15,  0,
    3,  5,  25, 25, 25, 25, 5,  3,
    0,  0,   0,  5,  5,  0,  0,  0,
    0,  -5,  0,  0,  0,  0,  -5,  0};

    /// <summary>
    /// The positional value of the pawn on each square.
    /// Note: a strong pawn center is valued high.
    /// </summary>
    public static sbyte[] Positions_Pawn =
    {
    0,  0,  0,  0,  0,  0,  0,  0,
    0,  0,  0,  0,  0,  0,  0,  0,
    3,  3,  5,  5,  5,  5,  3,  3,
    -10,  3,  12, 15, 23, 12, 5, -10,
    -10,  0,  15, 15, 22, 12, 7, -10,
    5,  5,  15, 15, 15, 15, 5,  5,
    0,  0,  0,  0,  0,  0,  0,  0,
    0,  0,  0,  0,  0,  0,  0,  0};

    /// <summary>
    /// The positional value of the queen on each square.
    /// </summary>
    public static sbyte[] Positions_Queen =
    {
    0,  0,  0,  0,  1,  0,  0,  0,
    0,  2, 15,  0,  6,  0,  0,  0,
    3,  5,  0,  5,  5,  5,  3,  3,
   12,  0,  0,  0,  0,  0,  0, 15,
    0,  0,  0,  0,  0,  0,  0,  0,
    5,  5, 15, 15,  5,  5,  0,  0,
    0,  0, 12, 12,  5,  0,  0,  0,
    0,  0,  0,  0,  1,  0,  0,  0};
    #endregion

}
