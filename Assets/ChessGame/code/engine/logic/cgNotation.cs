using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;
/// <summary>
/// Reads / writes game notation. Supports algebraic and coordinate notations.
/// </summary>
public class cgNotation
{
    /// <summary>
    /// All moves being notated
    /// </summary>
    public List<cgSimpleMove> moves = new List<cgSimpleMove>();

    /// <summary>
    /// Notationtypes, algrebraic = e4, bxd3, O-O etc. Coordinate: e2-e4 d1-c2 c3-h8 etc.
    /// </summary>
    public enum NotationType { Algebraic, Coordinate }

    /// <summary>
    /// PGN format is the standard of chess.
    /// </summary>
    public enum FormatType { None, PGN }

    public cgBoard board;
    /// <summary>
    /// Notates the a game from the provided board.
    /// </summary>
    /// <param name="fromBoard">The board containing all moves to be notated</param>
    public cgNotation(cgBoard fromBoard)
    {
        board = fromBoard;
        foreach (cgSimpleMove move in fromBoard.moves) AddMove(move);
    }

    /// <summary>
    /// Add a move to the list of moves.
    /// </summary>
    /// <param name="move"></param>
    public void AddMove(cgSimpleMove move)
    {
        moves.Add(move);
    }

    /// <summary>
    /// Add a move based on a notation type.
    /// </summary>
    /// <param name="move">the string representation of the move(e4, bxd3 or e2-e4 etc.)</param>
    /// <param name="notationType"></param>
    public void AddMove(string move, NotationType notationType)
    {
        if (notationType == NotationType.Coordinate)
        {
            if (move == "0-0")
            {
                //cgSimpleMove smove = new cgCastlingMove(0,0,0,0,0);

            }
            else if (move == "0-0-0")
            {

            }
            else
            {
                byte start =0;
                byte end= 0;
                for (byte i = 0; i < board.SquareNames.Length; i++)
                {
                    string str = board.SquareNames[i];
                   
                    if(str == move.Substring(0, 2))
                    {
                        start = i;
                    }
                    if(str == move.Substring(3, 2))
                    {
                        end = i;
                    }
                }

                cgSimpleMove smove = new cgSimpleMove(start, end);
                moves.Add(smove);
            }
        }

    }

    /// <summary>
    /// Writes the full game notation from the current moves stored in Moves list.
    /// </summary>
    /// <param name="type">What notationtype should it be?</param>
    /// <param name="formatType">Should it be PGN format or not?</param>
    /// <returns>A string with full game notation.</returns>
    public string writeFullNotation(NotationType type, FormatType formatType = FormatType.None)
    {
        string str = "";
        cgBoard disambiguationBoard = board.duplicate().revertToStart();
        if (type == NotationType.Coordinate)
        {
            foreach (cgSimpleMove pcem in moves)
            {
                str += (disambiguationBoard.SquareNames[pcem.from] + "-" + disambiguationBoard.SquareNames[pcem.to]) + " ";
            }
        }
        if (formatType == FormatType.PGN)
        {
            string q = "\"";
            str = " [Event " + q + "Pro Chess" + q + "]\n";
            str += " [Site " + q + "Undefined site" + q + "]\n";
            str += " [Date " + q + DateTime.Now + q + "]\n";
            str += " [White " + q + "Chessplayer1" + q + "]\n";
            str += " [Black " + q + "Chessplayer2" + q + "]\n";
            str += " [Result " + q + "1/2-1/2" + q + "]\n";
        }
        if (type == NotationType.Algebraic)
        {
            foreach (cgSimpleMove pcem in moves)
            {
                if (disambiguationBoard.moves.Count % 2 == 0) str += (Math.Floor(disambiguationBoard.moves.Count / 2f) + 1).ToString() + ". ";

                int typ = Math.Abs(disambiguationBoard.squares[pcem.from]);
                List<cgSimpleMove> othermoves = disambiguationBoard.findLegalMoves(disambiguationBoard.whiteTurnToMove);
                List<cgSimpleMove> ambiguousMoves = new List<cgSimpleMove>();
                foreach (cgSimpleMove othermove in othermoves) if (othermove.to == pcem.to && othermove.from != pcem.from && Math.Abs(disambiguationBoard.squares[othermove.from]) == typ) ambiguousMoves.Add(othermove);
                if (typ == 1 && pcem.capturedType != 0) str += disambiguationBoard.SquareNames[pcem.from].Substring(0, 1);
                if (typ == 2) str += "R";
                if (typ == 3) str += "N";
                if (typ == 4) str += "B";
                if (typ == 5) str += "Q";
                if (typ == 6 && !(pcem is cgCastlingMove)) str += "K";
                //if (typ == 6) str += "K";

                if (ambiguousMoves.Count > 0 && typ != 1)
                {
                    bool fileMatch = false;
                    bool rankMatch = false;
                    foreach (cgSimpleMove ambiguousMove in ambiguousMoves)
                    {
                        if (disambiguationBoard.SquareNames[ambiguousMove.from].Substring(0, 1) == disambiguationBoard.SquareNames[pcem.from].Substring(0, 1)) fileMatch = true;
                        if (disambiguationBoard.SquareNames[ambiguousMove.from].Substring(1, 1) == disambiguationBoard.SquareNames[pcem.from].Substring(1, 1)) rankMatch = true;
                    }
                    if (!fileMatch) str += disambiguationBoard.SquareNames[pcem.from].Substring(0, 1);
                    else if (fileMatch && !rankMatch) str += disambiguationBoard.SquareNames[pcem.from].Substring(1, 1);
                    else if (fileMatch && rankMatch) str += disambiguationBoard.SquareNames[pcem.from];
                }
                if (pcem.capturedType != 0) str += "x";
                if (pcem is cgCastlingMove)
                {
                    if (pcem.to == 2 || pcem.to == 58) str += "O-O-O";
                    else str += "O-O";
                }
                else str += disambiguationBoard.SquareNames[pcem.to];
                if (pcem.queened) str += "=Q";
                str += " ";
                disambiguationBoard.move(pcem);
            }
        }

        return str;
    }
    /// <summary>
    /// A slight adjustment to GetFullNotation, in which new lines are added to indent the text in an orderly fashion to be displayed to the player.
    /// </summary>
    /// <returns></returns>
    public string getLogFriendlyNotation()
    {
        string str = writeFullNotation(NotationType.Algebraic);

        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] == '.')
            {
                if (str[i - 1] == ' ') str = str.Insert(i - 1, "\n");
                if ((i - 2) > 0 && str[i - 2] == ' ') str = str.Insert(i - 2, "\n");
                if ((i - 3) > 0 && str[i - 3] == ' ') str = str.Insert(i - 3, "\n");


                i += 2;
            }
        }
        return str;
    }

    /// <summary>
    /// Read the notations in the provided string.
    /// </summary>
    /// <param name="curgame">the string to decipher.</param>
    public void Read(string curgame)
    {
        int lastIndex = 0;
        int nextIndex = 0;
        string move = curgame.Substring(lastIndex, nextIndex);
        NotationType ntype = NotationType.Algebraic; //= NotationType.Coordinate;
        //Debug.Log("first move:" + move);
        //if (move.Contains("-")) ntype = NotationType.Coordinate;
        moves = new List<cgSimpleMove>();
        if (ntype == NotationType.Coordinate)
        {
            while (lastIndex != -1)
            {
               
                byte fromp = board.IndexFromCellName(move.Substring(0, 2));
                byte top = board.IndexFromCellName(move.Substring(3, 2));
                moves.Add(new cgSimpleMove(fromp, top));
                nextIndex = curgame.IndexOf(" ", lastIndex + 1);
                if (nextIndex == -1) break;
                move = curgame.Substring(lastIndex + 1, nextIndex - lastIndex);
                lastIndex = nextIndex;
                //Debug.Log("current move being analyzed="+move);
            }
        }
        else if (ntype == NotationType.Algebraic)
        {
            cgBoard disambBoard = new cgBoard();
            cgSimpleMove chosenMove;
            while (lastIndex != -1)
            {
                chosenMove = null;

                nextIndex = curgame.IndexOf(" ", nextIndex + 1);
                if (nextIndex == -1 || lastIndex == -1) break;
                move = curgame.Substring(lastIndex + 1, nextIndex - lastIndex);
                bool legitMove = (!move.Contains(".") && move.Length > 1 && !move.Contains("\n")) ? true : false;

                move = move.Trim(' ');
                //move = move.Trim('\n');
                //Debug.Log("trimmed:" + move+" contains .:"+move.Contains(".")+" contains newline:"+move.Contains("\n")+" legit move:"+legitMove);
                if (move.Contains("{")) nextIndex = curgame.IndexOf("}", lastIndex + 1);
                else if (move.Contains("[")) nextIndex = curgame.IndexOf("]", lastIndex + 1);
                else if (legitMove)
                {
                    //Debug.Log("found to be legit move.");
                    byte tosquare;
                    byte pushback = 2;
                    byte type = 1;
                    //bool promotion = false;
                    bool shortCastling = (move == "O-O");
                    bool longCastling = (move == "O-O-O");
                    if (move.Contains("="))
                    {
                        //promotion = true;
                        move.Remove(move.IndexOf("="), 2);
                    }
                    else if (move.Contains("+")) move.Remove(move.IndexOf("+"), 1);
                    else if (move.Contains("!")) move.Remove(move.IndexOf("!"), 1);
                    else if (move.Contains("?")) move.Remove(move.IndexOf("?"), 1);
                    tosquare = board.IndexFromCellName(move.Substring(move.Length - pushback, 2));
                    if (move[0] == 'R') type = 2;
                    if (move[0] == 'N') type = 3;
                    if (move[0] == 'B') type = 4;
                    if (move[0] == 'Q') type = 5;
                    if (move[0] == 'K') type = 6;

                    List<cgSimpleMove> ambiguousMoves = new List<cgSimpleMove>();
                    foreach (cgSimpleMove legalMove in disambBoard.findLegalMoves(disambBoard.whiteTurnToMove))
                    {
                        if (shortCastling && legalMove is cgCastlingMove)
                        {
                            if (legalMove.to == 6 || legalMove.to == 62)
                            {
                                chosenMove = legalMove;
                                break;
                            }
                        }
                        else if (longCastling && legalMove is cgCastlingMove)
                        {
                            if (legalMove.to == 2 || legalMove.to == 58)
                            {
                                chosenMove = legalMove;
                                break;
                            }
                        }
                        if (Math.Abs(disambBoard.squares[legalMove.from]) == type && legalMove.to == tosquare) ambiguousMoves.Add(legalMove);
                    }
                    if (ambiguousMoves.Count == 0 && chosenMove == null)
                    {
                        //Debug.WriteLine("found no matching move for the string: " + move+" type:"+type+" tosquare:"+tosquare+" chosenMove:"+chosenMove+" castling:"+shortCastling);
                        break;
                    }
                    else if (ambiguousMoves.Count == 1) chosenMove = ambiguousMoves[0];
                    else if (ambiguousMoves.Count > 1)
                    {
                        //UnityEngine.Debug.Log("2 or mroe ambiguousmoves");
                        //2 or more ambiguous moves in which the piece type matches and the destination square matches. Further disambiguation needed.
                        List<cgSimpleMove> matching = new List<cgSimpleMove>();
                        foreach (cgSimpleMove mov in ambiguousMoves)
                        {

                            if (board.SquareNames[mov.from].Contains(move.Substring(1+(type==1?-1:0), 1))) matching.Add(mov);
                        }

                        if (matching.Count == 1) chosenMove = matching[0];//only 1 of the ambiguous moves have the correct rank and/or file.
                        else
                        {
                            foreach (cgSimpleMove mov in ambiguousMoves)
                            {
                                if (board.SquareNames[mov.from].Contains(move.Substring(1, 2)))
                                {
                                    chosenMove = ambiguousMoves[0];
                                    break;
                                }
                            }
                        }
                    }
                    if (chosenMove != null)
                    {
                        disambBoard.move(chosenMove);
                        moves.Add(chosenMove);
                    }
                }
                Debug.WriteLine("legitmove:" + legitMove);
                lastIndex = nextIndex;
            }
        }
    }
}
