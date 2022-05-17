using System;
using System.Collections.Generic;
using System.Diagnostics;

/// <summary>
/// The chess board, here moves are generated, checked for legality, performed and reverted.
/// </summary>
[System.Serializable]
public class cgBoard
{
    #region properties / fields
    /// <summary>
    /// The 0-64 index presentation of the board. This is where the magic happends. Below is a detailed list of what values can be expected, and what each index position corresponds to on the board.
    /// </summary>
    public List<sbyte> squares = new List<sbyte>(64);
    /**
     * 0 = unoccupied square
     * 
     * 1 = white pawn
     * 2 = white rook
     * 3 = white knight
     * 4 = white bishop
     * 5 = white queen
     * 6 = white king
     * 
     * -1 = black pawn
     * -2 = black rook
     * -3 = black knight
     * -4 = black bishop
     * -5 = black queen
     * -6 = black king
     * 
     * 
       0  1  2  3  4  5  6  7
    *  8  9  10 11 12 13 14 15
    *  16 17 18 19 20 21 22 23
    *  24 25 26 27 28 29 30 31
    *  32 33 34 35 36 37 38 39 
    *  40 41 42 43 44 45 46 47
    *  48 49 50 51 52 53 54 55
    *  56 57 58 59 60 61 62 63
    * *
     * */

    /// <summary>
    /// A list of all moves performed on the board in chronological order since the start of the game.
    /// </summary>
    public List<cgSimpleMove> moves = new List<cgSimpleMove>();

    /// <summary>
    /// Total number of moves that have been performed(regardless of reverting).
    /// </summary>
    public int moveCount = 0;

    /// <summary>
    /// Total number of reverts that have been performed.
    /// </summary>
    public int revertCount = 0;


    /// <summary>
    /// Has white castled? used to add value on board avaluation.
    /// </summary>
    public bool whiteHasCastled = false;

    /// <summary>
    /// Has black castled? used to subtract value on board avaluation.
    /// </summary>
    public bool blackHasCastled = false;

    /// <summary>
    /// the index at which squares are out of bounds, in other words the index that exceeds the board.
    /// </summary>
    public byte illegalCellIndex = 65;

    /// <summary>
    /// Width of the board.
    /// </summary>
    public byte boardWidth = 8;

    /// <summary>
    /// Height of board
    /// </summary>
    public byte boardHeight = 8;

    public bool castlingAllowed = true;
    /// <summary>
    /// Cell names according to index position.
    /// </summary>
    public string[] SquareNames = {
        "a8","b8","c8","d8","e8","f8","g8","h8",
        "a7","b7","c7","d7","e7","f7","g7","h7",
        "a6","b6","c6","d6","e6","f6","g6","h6",
        "a5","b5","c5","d5","e5","f5","g5","h5",
        "a4","b4","c4","d4","e4","f4","g4","h4",
        "a3","b3","c3","d3","e3","f3","g3","h3",
        "a2","b2","c2","d2","e2","f2","g2","h2",
        "a1","b1","c1","d1","e1","f1","g1","h1",
                                       "Out ouf bounds"};
    /// <summary>
    /// The default starting position.
    /// </summary>
    public static List<sbyte> defaultStartPosition = new List<sbyte> {
            -2, -3, -4, -5, -6, -4, -3, -2,
            -1, -1, -1, -1, -1, -1, -1, -1,
             0,  0,  0,  0,  0,  0,  0,  0,
             0,  0,  0,  0,  0,  0,  0,  0,
             0,  0,  0,  0,  0,  0,  0,  0,
             0,  0,  0,  0,  0,  0,  0,  0,
             1,  1,  1,  1,  1,  1,  1,  1,
             2,  3,  4,  5,  6,  4,  3,  2
            };

    [UnityEngine.SerializeField]
    private byte _enPassantSquare = 0;
    [UnityEngine.SerializeField]
    private byte _enPassantCapturesOn = 0;
    [UnityEngine.SerializeField]
    private bool _whiteTurnToMove = true;
    [UnityEngine.SerializeField]
    private byte _whiteLeftRookMoves = 0;
    [UnityEngine.SerializeField]
    private byte _whiteRightRookMoves = 0;
    [UnityEngine.SerializeField]
    private byte _blackLeftRookMoves = 0;
    [UnityEngine.SerializeField]
    private byte _blackRightRookMoves = 0;
    [UnityEngine.SerializeField]
    private byte _whiteKingMoves = 0;
    [UnityEngine.SerializeField]
    private byte _blackKingMoves = 0;
    [UnityEngine.SerializeField]
    private byte[] _blackRooksStart;
    [UnityEngine.SerializeField]
    private byte[] _whiteRooksStart;
    [UnityEngine.SerializeField]
    private byte _blackKingStart;
    [UnityEngine.SerializeField]
    private byte _whiteKingStart;
    [UnityEngine.SerializeField]
    private byte[] _whiteLeftCastleCrossSquares;
    [UnityEngine.SerializeField]
    private byte[] _whiteRightCastleCrossSquares;
    [UnityEngine.SerializeField]
    private byte[] _blackLeftCastleCrossSquares;
    [UnityEngine.SerializeField]
    private byte[] _blackRightCastleCrossSquares;
    [UnityEngine.SerializeField]
    private byte _whiteKingLeftCastleTo;
    [UnityEngine.SerializeField]
    private byte _whiteKingRightCastleTo;
    [UnityEngine.SerializeField]
    private byte _blackKingLeftCastleTo;
    [UnityEngine.SerializeField]
    private byte _blackKingRightCastleTo;
    [UnityEngine.SerializeField]
    private byte _whiteRookLeftCastleTo;
    [UnityEngine.SerializeField]
    private byte _whiteRookRightCastleTo;
    [UnityEngine.SerializeField]
    private byte _blackRookLeftCastleTo;
    [UnityEngine.SerializeField]
    private byte _blackRookRightCastleTo;


    [UnityEngine.SerializeField]
    private byte _whitePromotionBelow = 8;
    [UnityEngine.SerializeField]
    private byte _blackPromotionAbove = 54;
    [UnityEngine.SerializeField]
    private byte _pawnDoubleMoveDistance = 16;
    [UnityEngine.SerializeField]
    private byte _halfwaySquare = 32;
    /// <summary>
    /// Is it whites turn to move?
    /// </summary>
    public bool whiteTurnToMove
    {
        get { return _whiteTurnToMove; }
    }


    /// <summary>
    /// All possible moves generated by MoveGenerator, stored by an identifying string using the format (piecetype)+(indexpostion)
    /// </summary>
    [UnityEngine.SerializeField]
    //public SerializableDictionary<string, cgMoveSet> _allHypotheticalMoves = null;
    private Dictionary<string, cgMoveSet> _allHypotheticalMoves = null;

    /// <summary>
    /// Create a new instance of the board.
    /// </summary>
    /// <param name="positions"></param>
    #endregion

    public cgBoard(List<sbyte> positions = null, byte boardWidth = 8, byte boardHeight = 8,bool castlingAllowed = true, Dictionary<string, cgMoveSet> allHypotheticalMoves = null)
    {
        this.castlingAllowed = castlingAllowed;
        if (positions != null) squares = positions;
        else
        {
            foreach (sbyte sb in defaultStartPosition) squares.Add(sb);
        }

        _generateAllPossibleMoves(boardWidth, boardHeight);
        //else _allHypotheticalMoves = allHypotheticalMoves;
        //UnityEngine.Debug.Log("Cg board instantiated. " + squares.Count + " hypotheticals: " + _allHypotheticalMoves);

    }
    public void generateHypotheticalMoves()
    {
        _generateAllPossibleMoves(boardWidth, boardHeight);
    }
    /// <summary>
    /// Perform the provided move, capturing if necessary.
    /// </summary>
    /// <param name="move">The move to perform.</param>
    public void move(cgSimpleMove move)
    {
        moves.Add(move);
        moveCount++;
        if (move.to == illegalCellIndex || move.to >= squares.Count || move.to < 0)
        {//Illegal index for move.

            //Console.WriteLine("Failed move: " + move.to+" "+squares.Count+" "+move.from+" castling: "+(move is cgCastlingMove));
            return;
        }
        if (move is cgCastlingMove)
        {
            //if move is castling there is a secondary move to also perform.
            sbyte tempType = squares[(move as cgCastlingMove).secondFrom];
            squares[move.to] = squares[move.from];
            squares[move.from] = 0;
            squares[(move as cgCastlingMove).secondTo] = tempType;
            if (Math.Abs(squares[(move as cgCastlingMove).secondFrom]) == 2)
            {
                squares[(move as cgCastlingMove).secondFrom] = 0;
            }
            if (_whiteTurnToMove) whiteHasCastled = true;
            else blackHasCastled = true;
        }
        else
        {
            move.capturedType = squares[move.to];
            squares[move.to] = squares[move.from];
            squares[move.from] = 0;
        }


        if (move is cgEnPassantMove)
        {
            move.capturedType = squares[(move as cgEnPassantMove).attackingSquare];
            squares[(move as cgEnPassantMove).attackingSquare] = 0;
        }

        //Auto promoting pawns on last rank to a queen.
        if (move.to < _whitePromotionBelow && squares[move.to] == 1)
        {
            move.queened = true;
            squares[move.to] = 5;
        }
        else if (move.to > _blackPromotionAbove && squares[move.to] == -1)
        {
            move.queened = true;
            squares[move.to] = -5;
        }

        //Counting the moves of each rook for castling rights.
        if (squares[move.to] == -2 && move.from == _blackRooksStart[0]) _blackLeftRookMoves++;
        else if (_blackRooksStart.Length > 1 && squares[move.to] == -2 && move.from == _blackRooksStart[1]) _blackRightRookMoves++;
        else if (squares[move.to] == 2 && move.from == _whiteRooksStart[0]) _whiteLeftRookMoves++;
        else if (_whiteRooksStart.Length > 1 && squares[move.to] == 2 && move.from == _whiteRooksStart[1]) _whiteRightRookMoves++;
        
        //Counting the moves of each king for castling rights.
        if (squares[move.to] == 6) _whiteKingMoves++;
        else if (squares[move.to] == -6) _blackKingMoves++;

        //Pass turn to opposite side.
        _whiteTurnToMove = !_whiteTurnToMove;

    }

    /// <summary>
    /// Does the provided color still have its long castling rights?
    /// </summary>
    /// <param name="white">The color whoms castling right should be checked,false=black</param>
    /// <returns>Does still have its long castling rights?</returns>
    public bool longCastlingRights(bool white)
    {
        if (white) return (_whiteKingMoves == 0 && _whiteLeftRookMoves == 0 && !whiteHasCastled);
        else return (_blackKingMoves == 0 && _blackLeftRookMoves == 0 && !blackHasCastled);
    }

    /// <summary>
    /// Does the provided color still have its short castling rights?
    /// </summary>
    /// <param name="white">The color whoms castling right should be checked,false=black</param>
    /// <returns>Does still have its short castling rights?</returns>
    public bool shortCastlingRights(bool white)
    {
        if (white) return (_whiteKingMoves == 0 && _whiteRightRookMoves == 0 && !whiteHasCastled);
        else return (_blackKingMoves == 0 && _blackRightRookMoves == 0 && !blackHasCastled);
    }

    /// <summary>
    /// Revert the last performed move.
    /// </summary>
    public void revert()
    {
        if (moves.Count > 0)
        {
            revertCount++;
            if (moves[moves.Count - 1].to == illegalCellIndex)
            {
                moves.RemoveAt(moves.Count - 1);
                return;
            }

            //revert piwce placement and add previously captured piece on departing square(if not an enpassant move)

            if (moves[moves.Count - 1] is cgCastlingMove)
            {
                sbyte tempType = squares[(moves[moves.Count - 1] as cgCastlingMove).secondTo];
                squares[moves[moves.Count - 1].from] = squares[moves[moves.Count - 1].to];
                if (moves[moves.Count - 1].to != moves[moves.Count - 1].from)
                    squares[moves[moves.Count - 1].to] = 0;
                squares[(moves[moves.Count - 1] as cgCastlingMove).secondFrom] = tempType;
                if (Math.Abs(squares[(moves[moves.Count - 1] as cgCastlingMove).secondTo]) == 2)
                {
                    squares[(moves[moves.Count - 1] as cgCastlingMove).secondTo] = 0;
                }
                if (!_whiteTurnToMove) whiteHasCastled = false;
                else blackHasCastled = false;
            }
            else
            {
                squares[moves[moves.Count - 1].from] = squares[moves[moves.Count - 1].to];
                squares[moves[moves.Count - 1].to] = moves[moves.Count - 1].capturedType;
                if (moves[moves.Count - 1] is cgEnPassantMove)
                {
                    squares[(moves[moves.Count - 1] as cgEnPassantMove).attackingSquare] = (sbyte)(moves[moves.Count - 1] as cgEnPassantMove).capturedType;
                    squares[(moves[moves.Count - 1] as cgEnPassantMove).to] = 0;
                }
            }
            //if move was a pawn getting queened then revert the piece back to pawn aswell.
            if (moves[moves.Count - 1].queened)
            {
                //bool white = squares[moves[moves.Count - 1].from] > 0 ? true : false;
                squares[moves[moves.Count - 1].from] = (sbyte)(moves[moves.Count - 1].from > this._halfwaySquare ? -1 : 1);
            }

            //if piece is rook register its moves for castling rights purposes.
            if (_blackRooksStart.Length > 0 && squares[moves[moves.Count - 1].from] == -2 && moves[moves.Count - 1].from == _blackRooksStart[0]) _blackLeftRookMoves--;
            else if (_blackRooksStart.Length > 1 && squares[moves[moves.Count - 1].from] == -2 && moves[moves.Count - 1].from == _blackRooksStart[1]) _blackRightRookMoves--;
            else if (_whiteRooksStart.Length > 0 && squares[moves[moves.Count - 1].from] == 2 && moves[moves.Count - 1].from == _whiteRooksStart[0]) _whiteLeftRookMoves--;
            else if (_whiteRooksStart.Length > 1 && squares[moves[moves.Count - 1].from] == 2 && moves[moves.Count - 1].from == _whiteRooksStart[1]) _whiteRightRookMoves--;

            //If piece is king register its moves for castling rights purposes
            if (squares[moves[moves.Count - 1].from] == 6) _whiteKingMoves--;
            else if (squares[moves[moves.Count - 1].from] == -6) _blackKingMoves--;

            //Debug.Log("reverted " + pceGlobal.CellNames[moves[moves.Count - 1].to] + "->" + pceGlobal.CellNames[moves[moves.Count - 1].from]+" squares now occupied there:"+squares[moves[moves.Count-1].to]+" & "+squares[moves[moves.Count-1].from]);
            moves.RemoveAt(moves.Count - 1);
            _whiteTurnToMove = !_whiteTurnToMove;
        }
    }

    /// <summary>
    /// Revert the board all the way to the start.
    /// </summary>
    /// <returns>This board, for method chaining.</returns>
    public cgBoard revertToStart()
    {
        for (int i = moves.Count; i > 0; i--)
        {
            this.revert();
        }
        return this;
    }


    /// <summary>
    /// Evaluate the current board, adding together material values of both sides, positional values, castling values etc.
    /// </summary>
    public int Evaluate()
    {
        int lazyVal = 0;
        sbyte[] rooks = new sbyte[] { 0, 0 };
        sbyte[] bishops = new sbyte[] { 0, 0 };
        for (int i = 0; i < squares.Count; i++)
        {
            switch (squares[i])
            {
                case 0:
                    break;
                case 1:
                    lazyVal += cgValueModifiers.Value_Pawn;
                    lazyVal += cgValueModifiers.Positions_Pawn[i];
                    break;
                case 2:
                    lazyVal += cgValueModifiers.Value_Rook;
                    lazyVal += cgValueModifiers.Positions_Rook[i];
                    rooks[0]++;
                    break;
                case 3:
                    lazyVal += cgValueModifiers.Value_Knight;
                    lazyVal += cgValueModifiers.Positions_Knight[i];
                    break;
                case 4:
                    lazyVal += cgValueModifiers.Value_Bishop;
                    lazyVal += cgValueModifiers.Positions_Bishop[i];
                    bishops[0]++;
                    break;
                case 5:
                    lazyVal += cgValueModifiers.Value_Queen;
                    break;
                case 6:
                    lazyVal += cgValueModifiers.Value_King;
                    break;
                case -1:
                    lazyVal -= cgValueModifiers.Value_Pawn;
                    lazyVal -= cgValueModifiers.Positions_Pawn[i];
                    break;
                case -2:
                    lazyVal -= cgValueModifiers.Value_Rook;
                    lazyVal -= cgValueModifiers.Positions_Rook[i];
                    rooks[1]++;
                    break;
                case -3:
                    lazyVal -= cgValueModifiers.Value_Knight;
                    lazyVal -= cgValueModifiers.Positions_Knight[i];
                    bishops[1]++;
                    break;
                case -4:
                    lazyVal -= cgValueModifiers.Value_Bishop;
                    lazyVal -= cgValueModifiers.Positions_Bishop[i];
                    break;
                case -5:
                    lazyVal -= cgValueModifiers.Value_Queen;
                    break;
                case -6:
                    lazyVal -= cgValueModifiers.Value_King;
                    break;
                default:
                    continue;
            }
        }

        //Pair bonuses
        if (bishops[0] > 1)
            lazyVal += cgValueModifiers.Value_BishopPairBonus;
        if (bishops[1] > 1)
            lazyVal -= cgValueModifiers.Value_BishopPairBonus;
        if (rooks[1] > 1)
            lazyVal -= cgValueModifiers.Value_RookPairBonus;
        if (rooks[0] > 1)
            lazyVal += cgValueModifiers.Value_RookPairBonus;


        //add castling rights as value.
        if (longCastlingRights(true)) lazyVal += cgValueModifiers.Value_CastlingLongRights;
        if (longCastlingRights(false)) lazyVal -= cgValueModifiers.Value_CastlingLongRights;

        if (shortCastlingRights(true)) lazyVal += cgValueModifiers.Value_CastlingShortRights;
        if (shortCastlingRights(false)) lazyVal -= cgValueModifiers.Value_CastlingShortRights;

        if (blackHasCastled) lazyVal -= cgValueModifiers.Value_Castle;
        if (whiteHasCastled) lazyVal += cgValueModifiers.Value_Castle;
        return lazyVal;
    }

    #region moves
    private cgMoveSet _findMoveSetFor(int piece, int indexPosition)
    {
        if (piece < 0 && piece != -1) piece = Math.Abs(piece);
        string key = piece.ToString() + indexPosition.ToString();
        if (!_allHypotheticalMoves.ContainsKey(key))
        {
            //UnityEngine.Debug.Log("KEY NOT FOUNDL: " + key+" DICT "+_allHypotheticalMoves.Count);
        }
        return _allHypotheticalMoves[key];

    }

    /// <summary>
    /// Final verification, checks if the kings is attempting to perform a capture that leads to the king being taken next, which is the only illegal move findLegalMoves does not check(because it would be very intensive for the AI to search for this kind of move and also slightly redundant, as such a move leads immediately to loss of king which is the most highly valued piece on the board and thusly would be weighed as a terrible move).
    /// </summary>
    /// <param name="testMove">The move to be verified</param>
    /// <returns>Is this an illegal king capture?</returns>
    public bool verifyLegality(cgSimpleMove testMove)
    {
        if (testMove.to == illegalCellIndex) return false;
        //else if (Math.Abs(squares[testMove.from]) != 6) return true;
        else
        {
            move(testMove);
            foreach (cgSimpleMove mov in findLegalMoves(_whiteTurnToMove))
            {
                if (Math.Abs(squares[mov.to]) == 6)
                {
                    revert();
                    return false;
                }
            }
            revert();
            return true;
        }


    }

    /// <summary>
    /// Returns all legal moves for the provided color for the current board.
    /// Note: it may return a move in which the king captures an enemy piece to which the enemy can then next capture the king, use findStrictLegalMoves to avoid - however for the engine the computation to verify such a move are too costly.
    /// And since such a move immediatly leads to king capture, the AI will once it check the next moves find it to be a very very bad move.
    /// </summary>
    /// <param name="asWhite">Move as white?</param>
    /// <returns>All legal moves for provided color</returns>
    public List<cgSimpleMove> findLegalMoves(bool asWhite)
    {
        List<cgSimpleMove> legalMoves = new List<cgSimpleMove>();
        List<cgSimpleMove> enemyMoves = new List<cgSimpleMove>();

        bool check = false;
        _enPassantSquare = illegalCellIndex;
        _enPassantCapturesOn = illegalCellIndex;

        //test if there's an en passant opportunity.
        if (moves.Count > 0 && moves[moves.Count - 1].to != illegalCellIndex)
        {

            if (Math.Abs(squares[moves[moves.Count - 1].to]) == 1)
            {
                //its a pawn
                if (Math.Abs(moves[moves.Count - 1].from - moves[moves.Count - 1].to) == _pawnDoubleMoveDistance)
                {
                    //pawn did a double move the last move, this means we got ourselves an enpassant opportunity.
                    _enPassantSquare = (byte)(moves[moves.Count - 1].from + ((moves[moves.Count - 1].from >= _halfwaySquare) ? -boardWidth : boardWidth));
                    _enPassantCapturesOn = moves[moves.Count - 1].to;
                    //UnityEngine.Debug.Log("Moves[last] = " + (moves[moves.Count - 1].from));
                    //UnityEngine.Debug.Log("Double move twas done last move. EnPassant on: "+_enPassantSquare);
                }
            }
        }
        for (int i = 0; i < squares.Count; i++)
        {
            int piece = squares[i];
            if ((piece > 0 && asWhite) || (piece < 0 && !asWhite))
            {
                cgMoveSet allPotentialeMoves = _findMoveSetFor(piece, i);
                //UnityEngine.Debug.Log("Moves:" + moves);
                if (allPotentialeMoves != null)
                {
                    legalMoves.AddRange(_removeIllegalMoves(allPotentialeMoves));
                }
            }
            else if (piece != 0)
            {
                //piece is not empty and is not same colour as moving player.
                enemyMoves.AddRange(_removeIllegalMoves(_findMoveSetFor(piece, i)));
            }
            //legalMoves.AddRange(weakLegalMoves);
        }

        //examine whether castling is possible, first by seeing if castling rights exist(i.e king and rooks hasnt moved).
        bool shortCastling = shortCastlingRights(asWhite) && castlingAllowed;
        bool longCastling = longCastlingRights(asWhite) && castlingAllowed;

        //second by seing if rooks have been captured or not.
        if (shortCastling) shortCastling = asWhite ? squares[_whiteRooksStart[1]] == 2 : squares[_blackRooksStart[1]] == -2;
        if (longCastling) longCastling = asWhite ? squares[_whiteRooksStart[0]] == 2 : squares[_blackRooksStart[0]] == -2;
        //thirdly see if the squares necessary for castling are unoccupied.
        if (!asWhite && shortCastling)
        {// shortCastling = (squares[61] == 0 && squares[62] == 0) ? true : false;

            foreach (byte b in _blackRightCastleCrossSquares)
            {
                if (squares[b] != 0)//A square we are crossing to castle is occupied by a piece
                {
                    shortCastling = false;
                    break;
                }
            }
        }
        else if (asWhite && shortCastling)
        {

            foreach (byte b in _whiteRightCastleCrossSquares)
            {
                if (squares[b] != 0)//A square we are crossing to castle is occupied by a piece
                {
                    shortCastling = false;
                    break;
                }
            }
        }
        if (asWhite && longCastling)// longCastling = (squares[58] == 0 && squares[59] == 0 && squares[57] == 0) ? true : false;
        {
            foreach (byte b in _whiteLeftCastleCrossSquares)
            {
                if (squares[b] != 0)//A square we are crossing to castle is occupied by a piece
                {
                    longCastling = false;
                    break;
                }
            }
        }
        else if (!asWhite && longCastling)
        {
            foreach (byte b in _blackLeftCastleCrossSquares)
            {
                if (squares[b] != 0)//A square we are crossing to castle is occupied by a piece
                {
                    longCastling = false;
                    break;
                }
            }

        }

        foreach (cgSimpleMove move in enemyMoves)
        {
            if (Math.Abs(squares[move.to]) == 6)
            {
                //there is an enemy move attacking our king. We cannot castle now and we have to deal with this check.
                check = true;
                shortCastling = false;
                longCastling = false;
            }
            //check if any enemy move which is attacking a non-king piece of mine is on a ray that leads to my king, to see if any of my pieces are pinned and thusly illegal to move out of said ray.
            else if (Math.Abs(squares[move.to]) != 0)
            {
                List<byte> ray = _getFullRayFor(_findMoveSetFor(squares[move.from], move.from), move.to);
                if (ray != null)
                {
                    byte hitsAlongRay = 0;
                    sbyte hitKingAt = -1;
                    for (byte b = 0; b < ray.Count; b++)
                    {
                        if ((squares[ray[b]] == 6 && squares[move.from] < 0) || (squares[ray[b]] == -6 && squares[move.from] > 0))
                        {
                            //on a ray attacking my king.
                            hitKingAt = (sbyte)b;
                            break;
                        }
                        else if ((squares[ray[b]] > 0 ) || (squares[ray[b]] < 0))
                        {
                            //count a piece on the ray.
                            hitsAlongRay++;
                        }

                    }
                    //this enemy move is on a ray to attack my king, and its only being blocked by one piece(hitsalongray == 1) - so that piece is pinned and cannot move, so we delete all its mvoes(except any move it can make that captures the enemy piece or moves that are on the ray between king and attacking piece).
                    if (hitKingAt != -1 && hitsAlongRay == 1)
                    {
                        // UnityEngine.Debug.Log(squares[move.from] + " pins " + squares[move.to] + " at " + this.SquareNames[move.to] + " the ray hits king at " + hitKingAt + " theres " + hitsAlongRay + " blocking pieces inbetween");
                        ray.RemoveRange(hitKingAt, ray.Count - hitKingAt);
                        for (byte c = (byte)legalMoves.Count; c > 0; c--)
                        {
                            //Console.WriteLine("ray contains move: " + cgGlobal.MoveToString(legalMoves[c-1]) + " " + ray.Contains(legalMoves[c-1].from));
                            if (ray.Contains(legalMoves[c - 1].from))
                            {
                                //If the move is not blocking or attacking the enemy piece that is causing the pin or is directly attacking the enemy king.
                                if (!ray.Contains(legalMoves[c - 1].to) && legalMoves[c - 1].to != move.from && Math.Abs(squares[legalMoves[c-1].to]) != 6) {
                                    legalMoves.RemoveAt(c - 1);
                                }
                            }
                        }
                        //Console.WriteLine(cgGlobal.ListToString(legalMoves));
                        //Console.WriteLine(cgGlobal.ListToString(enemyMoves));
                        //Console.WriteLine("Legal moves: " + legalMoves.Count+" Enemy Moves:"+enemyMoves.Count );
                    }

                }
            }
            //disallowing castling(if its not already disallowed) if any enemy move attacks any castlign square
            if (shortCastling)
            {
                if (asWhite)
                {
                    foreach (byte b in _whiteRightCastleCrossSquares)
                    {
                        if (move.to == b)
                        {
                            shortCastling = false;
                            break;
                        }
                    }
                }
                else if (!asWhite)
                {
                    foreach (byte b in _blackRightCastleCrossSquares)
                    {
                        if (move.to == b)
                        {
                            longCastling = false;
                            break;
                        }
                    }
                }
            }
            if (longCastling)
            {
                if (asWhite)
                {

                    foreach (byte b in _whiteLeftCastleCrossSquares)
                    {
                        if (move.to == b)
                        {
                            longCastling = false;
                            break;
                        }
                    }
                }
                else if (!asWhite)
                {
                    foreach (byte b in _blackLeftCastleCrossSquares)
                    {
                        if (move.to == b)
                        {
                            longCastling = false;
                            break;
                        }
                    }
                }
            }
        }
        //We've jumped through 5 hoops to test the legality of castling, if shortcastling or longcastling is still true, we're in the clear.
        if (longCastling && asWhite) legalMoves.Add(new cgCastlingMove(_whiteKingStart, _whiteKingLeftCastleTo, cgValueModifiers.AlphaBeta_Weight_LongCastle, _whiteRooksStart[0], _whiteRookLeftCastleTo));
        else if (longCastling && !asWhite) legalMoves.Add(new cgCastlingMove(_blackKingStart, _blackKingLeftCastleTo, cgValueModifiers.AlphaBeta_Weight_LongCastle, _blackRooksStart[0], _blackRookLeftCastleTo));
        if (shortCastling && asWhite) legalMoves.Add(new cgCastlingMove(_whiteKingStart, _whiteKingRightCastleTo, cgValueModifiers.AlphaBeta_Weight_ShortCastle, _whiteRooksStart[1], _whiteRookRightCastleTo));
        else if (shortCastling && !asWhite) legalMoves.Add(new cgCastlingMove(_blackKingStart, _blackKingRightCastleTo, cgValueModifiers.AlphaBeta_Weight_ShortCastle, _blackRooksStart[1], _blackRookRightCastleTo));
        legalMoves.Sort(delegate (cgSimpleMove x, cgSimpleMove y)
        {
            return x.positionalVal.CompareTo(y.positionalVal);
        });
        legalMoves.Reverse();

        //check if any king move is illegal(if it would move to a square an enemy may attack).
        for (short b = (short)(legalMoves.Count); b > 0; b--)
        {
            if (Math.Abs(squares[legalMoves[b - 1].from]) == 6)
            {
                foreach (cgSimpleMove mov in enemyMoves)
                {

                    if (mov.to == legalMoves[b - 1].to && Math.Abs(squares[mov.from]) != 1)
                    {
                        legalMoves.RemoveAt(b - 1);
                        break;
                    }
                }
            }
        }

        //to make certain the engine gets to check.
        if (legalMoves.Count == 0) legalMoves.Add(new cgSimpleMove(illegalCellIndex, illegalCellIndex));
        //since there are no enemy moves checking my king we return all legal moves(note legal moves may still contain illegal moves that lead to my king captured next- which is illegal in chess).
        if (!check) return legalMoves;
        else if (check)
        {
            //there is atleast one enemy move currently checking my king. :(
            List<cgSimpleMove> checkingMoves = new List<cgSimpleMove>();
            foreach (cgSimpleMove movve in enemyMoves) if (Math.Abs(squares[movve.to]) == 6) checkingMoves.Add(movve);
            if (checkingMoves.Count == 1)
            {
                //as the king is only attacked by a single enemy move we may capture said single piece or block the path(if its a ray i.e attacking piece is not a knight or is not adjacent to the king)
                List<byte> legalMoveToSquares = _getBlockAttackSquares(checkingMoves[0]);
                for (int u = legalMoves.Count; u > 0; u--)
                {
                    if (legalMoves[u - 1].to == illegalCellIndex) continue;
                    if (!legalMoveToSquares.Contains(legalMoves[u - 1].to) && Math.Abs(squares[legalMoves[u - 1].from]) != 6)
                    {
                        legalMoves.RemoveAt(u - 1);
                    }
                }
            }
            else
            {
                //King is attacked by more than 1 enemy move, we have to move the king or lose.
                for (int u = legalMoves.Count; u > 0; u--)
                {
                    if (legalMoves[u - 1].from < illegalCellIndex && Math.Abs(squares[legalMoves[u - 1].from]) != 6) legalMoves.RemoveAt(u - 1);
                }
            }

        }

        //to make certain the engine gets to check.
        if (legalMoves.Count == 0) legalMoves.Add(new cgSimpleMove(illegalCellIndex, illegalCellIndex));

        return legalMoves;
        //UnityEngine.Debug.Log("Legal moves:" + legalMoves.Count);
    }

    /// <summary>
    /// Returns all legal moves for the provided color for the current board.
    ///
    /// </summary>
    /// <param name="asWhite">Move as white?</param>
    /// <returns>All legal moves for provided color</returns>
    public List<cgSimpleMove> findStrictLegalMoves(bool asWhite)
    {
        List<cgSimpleMove> legalMoves = findLegalMoves(asWhite);
        for (int i = legalMoves.Count; i > 0; i--)
        {
            if (!verifyLegality(legalMoves[i - 1])) legalMoves.RemoveAt(i - 1);
        }
        return legalMoves;

    }
    /// <summary>
    /// Returns all squares the block the provided move.
    /// </summary>
    /// <param name="forMove">The move that should be blocked</param>
    /// <returns>The squares that block the move.</returns>
    private List<byte> _getBlockAttackSquares(cgSimpleMove forMove)
    {
        List<byte> returns = new List<byte>();

        returns.Add(forMove.from);
        int type = Math.Abs(squares[forMove.from]);
        if (type == 2 || type == 4 || type == 5)
        {
            returns.AddRange(_getRayIn(_findMoveSetFor(type, forMove.from), forMove.to));
        }
        return returns;
    }

    /// <summary>
    /// Finds the ray in provided moveset that leads to provided destination.
    /// </summary>
    /// <param name="mset">The moveset to search.</param>
    /// <param name="to">The destination to find.</param>
    /// <returns>The ray that leads to the destination.</returns>
    private List<byte> _getRayIn(cgMoveSet mset, byte to)
    {
        byte startORay = 0;
        byte endORay = 0;
        for (byte i = 0; i < mset.moves.Count; i++)
        {
            if (mset.moves[i] == -1) startORay = i;
            else if (mset.moves[i] == to)
            {
                endORay = i;
                break;
            }
        }
        List<byte> returns = new List<byte>();
        for (int u = startORay; u < (endORay); u++)
        {
            returns.Add((byte)mset.moves[u]);
        }
        return returns;
    }

    /// <summary>
    /// Gets full ray(not cut at destination) in moveset that includes index position.
    /// </summary>
    /// <param name="set">The moveset to search</param>
    /// <param name="includesIndex">The index to find</param>
    /// <returns>the full ray that includes the index position</returns>
    private List<byte> _getFullRayFor(cgMoveSet set, byte includesIndex)
    {
        byte startOfRay = 0;
        byte endOfRay = illegalCellIndex;
        bool hitRay = false;
        if (set.type != 2 && set.type != 4 && set.type != 5) return null;
        for (byte i = 0; i < set.moves.Count; i++)
        {
            if (set.moves[i] == -1 && !hitRay) startOfRay = (byte)(i + 1);
            else if (set.moves[i] == -1 && hitRay)
            {
                endOfRay = (byte)(i);
                break;
            }
            if (set.moves[i] == includesIndex) hitRay = true;
        }
        if (!hitRay) return null;
        else
        {
            List<byte> ray = new List<byte>();
            for (int u = startOfRay; u < endOfRay; u++) if (set.moves[u] > -1) ray.Add((byte)set.moves[u]);

            //Debug.Log("retrieving entire ray for type:" + set.type + " searching for:" + includesIndex + " found at:" + startOfRay + " to:" + endOfRay + " we hit the ray:" + hitRay);
            //Debug.Log("ray length: " + ray.Count);
            return ray;
        }
    }
    /// <summary>
    /// Examines a moveset(all moves for a piece on a given square), and removes all illegal moves in this moveset based on the current board.
    /// </summary>
    /// <param name="moveSet">The moveset to examine</param>
    /// <returns>All unblocked moves</returns>
    private List<cgSimpleMove> _removeIllegalMoves(cgMoveSet moveSet)
    {
        List<cgSimpleMove> returns = new List<cgSimpleMove>();
        bool asWhite = squares[moveSet.from] > 0 ? true : false;
        bool rayGoing = true;
        bool canAttack = (moveSet.type == -1 || moveSet.type == 1) ? false : true;
        bool canMove = true;
        for (int i = 0; i < moveSet.moves.Count; i++)
        {
            if (moveSet.type == 3 || moveSet.type == 6) rayGoing = true; //add knight or king moves regardless of ray.
            if (moveSet.moves[i] == -1)
            {
                rayGoing = true;
                continue;
            }
            if (moveSet.moves[i] == -2)
            {
                //-2 signifies the beginning of attack only moves(pawn's forward diagonal attacks)
                canAttack = true;
                canMove = false;
                continue;
            }
            if (rayGoing || !canMove)
            {
                // UnityEngine.Debug.Log("move "+moveSet.moves[i]+" "+moveSet.from+" "+squares.Count+" "+moveSet.type+" "+this.boardHeight+" "+this.boardWidth);
                if (squares[moveSet.moves[i]] == 0 && canMove) returns.Add(new cgSimpleMove(moveSet.from, (byte)moveSet.moves[i], moveSet.positionalValues[i]));
                else if ((squares[moveSet.moves[i]] > 0 && !asWhite) || (squares[moveSet.moves[i]] < 0 && asWhite))
                {
                    if (canAttack)
                    {
                        //If I am white and attacking a black piece then I can take, or if I am black and attacking a white I can take - (Unless I'm a pawn);
                        returns.Add(new cgSimpleMove(moveSet.from, (byte)moveSet.moves[i], cgValueModifiers.AlphaBeta_Weight_Capture));
                        if (Math.Abs(squares[moveSet.moves[i]]) == 6) returns[returns.Count - 1].positionalVal = cgValueModifiers.AlphaBeta_Weight_Check;
                        //UnityEngine.Debug.Log("piece on" + moveSet.from + " takes on" + moveSet.moves[i]+" as white: "+asWhite+" piece on destination:"+_board.squares[moveSet.moves[i]]);
                        //continue;
                    }
                    rayGoing = false; //cannot continue further down this ray.

                    continue;
                }
                //test if an en passant move is possible
                else if (squares[moveSet.moves[i]] == 0 && canAttack && !canMove && _enPassantSquare == moveSet.moves[i]) returns.Add(new cgEnPassantMove(moveSet.from, (byte)moveSet.moves[i], cgValueModifiers.AlphaBeta_Weight_Capture, _enPassantCapturesOn));

                else if ((squares[moveSet.moves[i]] > 0 && asWhite) || (squares[moveSet.moves[i]] < 0 && !asWhite))
                {
                    //Black on black or white on white. Cannot make the move, and am now blocked from going further down this ray.
                    rayGoing = false;
                    continue;
                }
            }
        }
        return returns;
    }

    /// <summary>
    /// There are 3896 unique moves for any piece on any given square on an otherwise unoccupied 8x8 board, we will generate all these moves once here save each in a 'cgMoveSet' and then look these up later when needed.
    /// This saves a huge amount of computation when we are generating millions of moves for deep analysis later on.
    /// </summary>
    private void _generateAllPossibleMoves(byte boardWidth, byte boardHeight)
    {
        this.boardWidth = boardWidth;
        this.boardHeight = boardHeight;

        _createRules(boardWidth, boardHeight);
        //UnityEngine.Debug.Log("Generating all possible moves.");
        _allHypotheticalMoves = new Dictionary<string, cgMoveSet>();
        cgMoveGenerator moveGenerator = new cgMoveGenerator(boardWidth, boardHeight);
        for (int i = 0; i < squares.Count; i++)
        {
            _allHypotheticalMoves["5" + i.ToString()] = new cgMoveSet(moveGenerator.EmulateQueenAt(i), i, 5); ;
            _allHypotheticalMoves["2" + i.ToString()] = new cgMoveSet(moveGenerator.EmulateRookAt(i), i, 2);
            _allHypotheticalMoves["4" + i.ToString()] = new cgMoveSet(moveGenerator.EmulateBishopAt(i), i, 4);
            _allHypotheticalMoves["3" + i.ToString()] = new cgMoveSet(moveGenerator.EmulateKnightAt(i), i, 3);
            _allHypotheticalMoves["6" + i.ToString()] = new cgMoveSet(moveGenerator.EmulateKingAt(i), i, 6);

            //we ignore the starting row and the last row for pawns, as they either start in front of it, or get a promotion immediately when arriving on it.
            _allHypotheticalMoves["1" + i.ToString()] = new cgMoveSet(moveGenerator.EmulatePawnAt(i, true), i, 1);
            _allHypotheticalMoves["-1" + i.ToString()] = new cgMoveSet(moveGenerator.EmulatePawnAt(i, false), i, -1);
        }
        //Find castling squares - so find rooks start and king start, so castling rights can be verified.
        List<byte> whiteRooks = new List<byte>();
        List<byte> blackRooks = new List<byte>();
        for (byte i = 0; i < squares.Count; i++)
        {
            if (squares[i] == -2)
                blackRooks.Add(i);

            else if (squares[i] == 2)
                whiteRooks.Add(i);
            else if (squares[i] == -6)
                _blackKingStart = i;
            else if (squares[i] == 6)
                _whiteKingStart = i;
        }
        whiteHasCastled = true;
        blackHasCastled = true;
        _whiteRightRookMoves = byte.MaxValue;
        _whiteLeftRookMoves = byte.MaxValue;
        _blackRightRookMoves = byte.MaxValue;
        _blackLeftRookMoves = byte.MaxValue;

        _whiteRooksStart = whiteRooks.ToArray();
        _blackRooksStart = blackRooks.ToArray();
        if (_whiteRooksStart.Length > 0)
        {
            _findCastlingSquares(_whiteKingStart, _whiteRooksStart[0],
            out _whiteLeftCastleCrossSquares,  out _whiteKingLeftCastleTo, out _whiteRookLeftCastleTo);
            _whiteLeftRookMoves = 0;
            whiteHasCastled = false;
        }
        if (_whiteRooksStart.Length > 1)
        {
            _findCastlingSquares(_whiteKingStart, _whiteRooksStart[1],
            out _whiteRightCastleCrossSquares, out _whiteKingRightCastleTo, out _whiteRookRightCastleTo);
            _whiteRightRookMoves = 0;
            whiteHasCastled = false;
        }
        if (_blackRooksStart.Length > 0)
        {
            _findCastlingSquares(_blackKingStart, _blackRooksStart[0],
            out _blackLeftCastleCrossSquares,out _blackKingLeftCastleTo, out _blackRookLeftCastleTo);
            _blackLeftRookMoves = 0;
            blackHasCastled = false;
        }
        if (_blackRooksStart.Length > 1)
        {
            _findCastlingSquares(_blackKingStart, _blackRooksStart[1],
            out _blackRightCastleCrossSquares, out _blackKingRightCastleTo,out _blackRookRightCastleTo);
            _blackRightRookMoves = 0;
            blackHasCastled = false;
        }

        //UnityEngine.Debug.Log("All moves: " + _allHypotheticalMoves.Keys.Count + " " + whiteHasCastled + " " + blackHasCastled + " " + _whiteRightRookMoves + " " + _whiteLeftRookMoves + "  " + _blackRightRookMoves + " " + _blackLeftRookMoves);
        //UnityEngine.Debug.Log("" + _blackRooksStart.Length + " " + _whiteRooksStart.Length);

    }
    /// <summary>
    /// Finds the castling positions for king and rook, and the squares crossed between them.
    /// </summary>
    /// <param name="kingPos"></param>
    /// <param name="rookPos"></param>
    /// <returns>0 = Crossing squares, 1 = king post castle spot, 2 = rook post castle spot</returns>
    private void _findCastlingSquares(byte kingPos,byte rookPos, out byte[] crossingSquares, out byte kingDestination, out byte rookDestination)
    {
        crossingSquares = _range(kingPos, rookPos);
        kingDestination = kingPos;
        rookDestination = rookPos;

        if (kingPos < rookPos)
        {//King is to the left of the rook.
            kingDestination += (byte)(System.Math.Floor((double)((crossingSquares.Length + 2) / 2)));
            rookDestination = (byte)(kingDestination - 1);
        }
        else
        {// King is to the right of the rook.
            kingDestination -= (byte)(System.Math.Floor((double)((crossingSquares.Length + 2) / 2)));
            rookDestination = (byte)(kingDestination + 1);
        }
    }
    /// <summary>
    /// Partially create rules for a board of custom width and height.
    /// </summary>
    /// <param name="width">The number rows in the board</param>
    /// <param name="height">The number of columns in the board</param>
    private void _createRules(byte width, byte height)
    {
        _blackPromotionAbove = (byte)(width * (height - 1));
        _whitePromotionBelow = (byte)(width);
        _pawnDoubleMoveDistance = (byte)(width * 2);
        _halfwaySquare = (byte)System.Math.Abs((width * (height / 2)));
        illegalCellIndex = (byte)((byte)(height * width) + 1);

        //Build SquareNames
        char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToLower().ToCharArray();
        List<string> newsquares = new List<string>();
        for (var c = height; c > 0; c--)
        {
            for (var r = 0; r < width; r++)
            {
                string start = alphabet[r].ToString();
                string end = (c).ToString();
                newsquares.Add(start + end);
            }
        }

        SquareNames = newsquares.ToArray();

    }

    /// <summary>
    /// Converts a move to a human readable string. Useful for debugging purposes.
    /// </summary>
    /// <param name="move"></param>
    /// <returns>A readable string for output logging.</returns>
    public string moveToString(cgSimpleMove move)
    {
        if (move is cgCastlingMove)
        {
            cgCastlingMove cmove = (cgCastlingMove)move;
            return "(castling) " + cmove.from + "(" + squares[cmove.from] + ") -> " + cmove.to + " (" + squares[cmove.to] + ")  " + cmove.secondFrom + "(" + squares[cmove.secondFrom] + ") -> " + cmove.secondTo + "(" + squares[cmove.secondTo] + ")";
        }
        if (move.from < illegalCellIndex)
            return "(" + this.squares[move.from] + ") "+ move.from + "->" + move.to +  (this.squares[move.to]!=0 ? ("(" + this.squares[move.to] + ")") : "");
        return "invalid move";
    }

    #endregion

    #region read / paste
    /// <summary>
    /// Generates a board matching the provided notation.
    /// </summary>
    /// <param name="notation">The notation of the game to be recreated</param>
    public void LoadGame(cgNotation notation)
    {
        foreach (cgSimpleMove newmove in notation.moves)
        {
            List<cgSimpleMove> nmoves = findLegalMoves(_whiteTurnToMove);
            bool found = false;
            foreach (cgSimpleMove safemove in nmoves)
            {
                if (safemove.to == newmove.to && safemove.from == newmove.from)
                {
                    move(safemove);
                    found = true;
                }
            }
            if (!found)
            {
                Debug.WriteLine("couldn't find: " + this.moveToString(newmove));
                move(newmove);
            }
        }
    }

    /// <summary>
    /// Translates a coordinate string(e3, d1 etc.) to an index(0-64).
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public byte IndexFromCellName(string p)
    {
        for (byte i = 0; i < SquareNames.Length; i++) if (SquareNames[i] == p) return i;
        return 0;
    }
    #endregion

    #region Game state
    /// <summary>
    /// Is the color checked?
    /// </summary>
    /// <param name="asWhite">Should we check if white is checked(true) or if black is checked(false)</param>
    /// <returns>Is the color checked?</returns>
    public bool isChecked(bool asWhite)
    {
        foreach (cgSimpleMove mov in findLegalMoves(!asWhite))
        {
            if(mov.to == illegalCellIndex) continue;
            if (Math.Abs(squares[mov.to]) == 6) return true;
        }

        return false;
    }
    /// <summary>
    /// Creates a neat human readable string of the current board. Useful for debugging purposes.
    /// </summary>
    /// <returns>A neat human readable string.</returns>
    public string boardToNeatString()
    {
        string boardString = "\n";
        for (var i = 0; i < this.squares.Count; i++)
        {
            boardString += " " + (squares[i] < 0 ? "" : " ") + squares[i] + " ";
            if (i != 0 && ((i + 1) % boardWidth) == 0)
            {
                boardString += "\n";
            }
        }
        boardString += this._enPassantSquare.ToString() + this.shortCastlingRights(true).ToString() + this.longCastlingRights(true).ToString() + this.shortCastlingRights(false).ToString() + this.shortCastlingRights(false).ToString();
        return boardString;
    }
    /// <summary>
    /// Returns the board as not so neat string, useful for generating a unique string of current board state.
    /// </summary>
    /// <returns>A not so neat string with all information about current board.</returns>
    public string boardToString()
    {
        string boardString = "";
        for (var i = 0; i < this.squares.Count; i++)
        {
            boardString += this.squares[i].ToString();
        }
        boardString += this._enPassantSquare.ToString() + this.shortCastlingRights(true).ToString() + this.longCastlingRights(true).ToString() + this.shortCastlingRights(false).ToString() + this.shortCastlingRights(false).ToString();
        return boardString;
    }
    public enum OUTCOME { Undecided = 0, Draw = 1, BlackWin = 2, WhiteWin = 3 }

    //Returns a list of bytes between two values.
    private byte[] _range(byte start, byte end)
    {
        List<byte> returns = new List<byte>();
        if (start < end)
        {
            for (byte i = (byte)(start + 1); i < end; i++)
            {
                returns.Add(i);
            }
        }
        else
        {
            for (byte i = (byte)(start - 1); i > end; i--)
            {
                returns.Add(i);
            }

        }
        return returns.ToArray();
    }
    #endregion

    #region Duplicate
    /// <summary>
    /// Create a new board that is a duplication of this instance.
    /// </summary>
    /// <returns>A duplicate of this instance.</returns>
    public cgBoard duplicate()
    {
//        UnityEngine.Debug.Log("All hypo moves in duplicate: " + this._allHypotheticalMoves);
        cgBoard dup = new cgBoard(squares, this.boardWidth, this.boardHeight, this.castlingAllowed, this._allHypotheticalMoves);
        dup.moves = new List<cgSimpleMove>();
        for (int i = 0; i < this.moves.Count; i++)
            dup.moves.Add(this.moves[i].duplicate());
        dup.setDuplicateValues(this._enPassantSquare, this._enPassantCapturesOn, this._whiteTurnToMove,
            this._whiteLeftRookMoves, this._whiteRightRookMoves, this._blackLeftRookMoves, this._blackRightRookMoves, this._whiteKingMoves, this._blackKingMoves, _blackRooksStart, _whiteRooksStart, _blackKingStart,
    _whiteKingStart, _whiteLeftCastleCrossSquares, _whiteRightCastleCrossSquares, _blackLeftCastleCrossSquares, _blackRightCastleCrossSquares, _whiteKingLeftCastleTo, _whiteKingRightCastleTo, _blackKingLeftCastleTo,
    _blackKingRightCastleTo, _whiteRookLeftCastleTo, _whiteRookRightCastleTo, _blackRookLeftCastleTo, _blackRookRightCastleTo);
        return dup;
    }

    /// <summary>
    /// Setting all private vars for a new duplicate instance, to match its target.
    /// </summary>
    public void setDuplicateValues(byte enpassantSquare, byte enPassantCapture, bool whiteToMove, byte whiteARook, byte whiteHRook,
    byte blackARook, byte blackHRook, byte whiteKing, byte blackKing, byte[] _blackRooksStart, byte[] _whiteRooksStart, byte _blackKingStart,
    byte _whiteKingStart, byte[] _whiteLeftCastleCrossSquares, byte[] _whiteRightCastleCrossSquares, byte[] _blackLeftCastleCrossSquares, byte[] _blackRightCastleCrossSquares,
    byte _whiteKingLeftCastleTo, byte _whiteKingRightCastleTo, byte _blackKingLeftCastleTo, byte _blackKingRightCastleTo, byte _whiteRookLeftCastleTo, byte _whiteRookRightCastleTo,
    byte _blackRookLeftCastleTo, byte _blackRookRightCastleTo)
    {//Setting private vars.
        this._enPassantSquare = enpassantSquare;
        this._enPassantCapturesOn = enPassantCapture;
        this._whiteTurnToMove = whiteToMove;
        this._whiteLeftRookMoves = whiteARook;
        this._whiteRightRookMoves = whiteHRook;
        this._blackLeftRookMoves = blackARook;
        this._blackRightRookMoves = blackHRook;
        this._whiteKingMoves = whiteKing;
        this._blackKingMoves = blackKing;

        this._blackRooksStart = _blackRooksStart;
        this._whiteRooksStart = _whiteRooksStart;
        this._blackKingStart = _blackKingStart;
        this._whiteKingStart = _whiteKingStart;
        this._whiteLeftCastleCrossSquares = _whiteLeftCastleCrossSquares;
        this._whiteRightCastleCrossSquares = _whiteRightCastleCrossSquares;
        this._blackLeftCastleCrossSquares = _blackLeftCastleCrossSquares;
        this._blackRightCastleCrossSquares = _blackRightCastleCrossSquares;
        this._whiteKingLeftCastleTo = _whiteKingLeftCastleTo;
        this._whiteKingRightCastleTo = _whiteKingRightCastleTo;
        this._blackKingLeftCastleTo = _blackKingLeftCastleTo;
        this._blackKingRightCastleTo = _blackKingRightCastleTo;
        this._whiteRookLeftCastleTo = _whiteRookLeftCastleTo;
        this._whiteRookRightCastleTo = _whiteRookRightCastleTo;
        this._blackRookLeftCastleTo = _blackRookLeftCastleTo;
        this._blackRookRightCastleTo = _blackRookRightCastleTo;


    }
    #endregion
}
