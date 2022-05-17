using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Stores information about all possible moves a piece can make on a given index position on the board.
/// </summary>
[System.Serializable]
public class cgMoveSet
{
    /// <summary>
    /// The index location on the board.
    /// </summary>
    public byte from = 0;

    /// <summary>
    /// All possible moves this piece can perform from this location.
    /// </summary>
    public List<sbyte> moves;

    /// <summary>
    /// The piece type being whoms moves are being generated.
    /// </summary>
    public sbyte type;

    /// <summary>
    /// These values are based on the positional values stored in ValueModifiers, and these are used by the AlphaBeta Search Algorithm to optimize search time.
    /// these values are relative, meaning if I'm at a good spot, and move to mediocre spot, it will be counted as negative positional value.
    /// </summary>
    public List<sbyte> positionalValues = new List<sbyte>();

    /// <summary>
    /// Create a move set for a piece type on a specific index location.
    /// </summary>
    /// <param name="movesp">All possible moves</param>
    /// <param name="fromp">From this location</param>
    /// <param name="typesp">The piece type</param>
    public cgMoveSet(List<int> movesp, int fromp, int typesp)
    {
        moves = new List<sbyte>();
        from = (byte)fromp;

        type = (sbyte)typesp;
        foreach (sbyte i in movesp)
        {
            moves.Add((sbyte)i);
            positionalValues.Add(_findPositionalValueAt(i));
        }
    }


    private sbyte _findPositionalValueAt(sbyte at)
    {
        if (at < 0) return 0;
        else
        {
            sbyte current = 0;
            sbyte next = 0;
            if (type == -1 || type == 1)
            {
                current = from < cgValueModifiers.Positions_Pawn.Length ? cgValueModifiers.Positions_Pawn[from] : (sbyte)0;
                next = at < cgValueModifiers.Positions_Pawn.Length ?  cgValueModifiers.Positions_Pawn[at] : (sbyte)0;
            }
            else if (type == 2)
            {
                current = from < cgValueModifiers.Positions_Rook.Length ? cgValueModifiers.Positions_Rook[from] : (sbyte)0;
                next = at < cgValueModifiers.Positions_Rook.Length ? cgValueModifiers.Positions_Rook[at]:(sbyte)0;
            }
            else if (type == 3)
            {
                current = from < cgValueModifiers.Positions_Knight.Length ? cgValueModifiers.Positions_Knight[from]: (sbyte)0;
                next = at < cgValueModifiers.Positions_Knight.Length ? cgValueModifiers.Positions_Knight[at] : (sbyte)0;
            }
            else if (type == 4)
            {
                current = from < cgValueModifiers.Positions_Bishop.Length ? cgValueModifiers.Positions_Bishop[from]:  (sbyte)0;
                next = at < cgValueModifiers.Positions_Bishop.Length ? cgValueModifiers.Positions_Bishop[at]:  (sbyte)0;
            }
            else if (type == 5)
            {
                current = from < cgValueModifiers.Positions_Queen.Length ? cgValueModifiers.Positions_Queen[from]:  (sbyte)0;
                next = at < cgValueModifiers.Positions_Queen.Length ? cgValueModifiers.Positions_Queen[at]:  (sbyte)0;
            }

            return (sbyte)(next - current);
        }

    }


}
