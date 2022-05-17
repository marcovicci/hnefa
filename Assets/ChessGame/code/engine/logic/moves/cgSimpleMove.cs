/// <summary>
/// A simple non-passant and non-castling move, stores information such as the index of the departing square and the index of the arriving square.
/// </summary>
[System.Serializable]
public class cgSimpleMove 
{
    /// <summary>
    /// The square being departed from.
    /// </summary>
    public byte from;

    /// <summary>
    /// The square being arrived at.
    /// </summary>
    public byte to;

    /// <summary>
    /// The type of the piece being captured.
    /// </summary>
    public sbyte capturedType = 0;

    /// <summary>
    /// The positional value, used by the AlphaBeta Search algorithm, the higher this value the earlier the algorithm examines it - this does not mean the AI is more likely to pick it.
    /// </summary>
    public sbyte positionalVal;

    /// <summary>
    /// The actual value of the total board after this move has been performed based on material, pattern and positional values. Calculated by cgBoard.Evaluate
    /// </summary>
    public int val;

    /// <summary>
    /// Used by the engine for debugging purposes.
    /// </summary>
    public cgSimpleMove bestResponse;
    
    /// <summary>
    /// Did this move lead to a pawn promoting to a queen(queening)?
    /// </summary>
    public bool queened;

    /// <summary>
    /// A simple move, moving a single piece to another square.
    /// </summary>
    /// <param name="fromp">The 0-64 index of the square being departed from.</param>
    /// <param name="top">The 0-64 index of the square beeing arrived at.</param>
    /// <param name="posVal">The positional value, used by engine to sort moves in a best-first manner.</param>
    public cgSimpleMove(byte fromp, byte top,sbyte posVal = (sbyte)0)
    {
        from = fromp;
        to = top;
        positionalVal = posVal;
    }

    /// <summary>
    /// Duplicate this move. Used when duplicating a board which has had moves performed on it, said moves are also duplicated.
    /// </summary>
    /// <returns></returns>
    public virtual cgSimpleMove duplicate(){
        cgSimpleMove dup = new cgSimpleMove(this.from,this.to,this.positionalVal);
        dup.capturedType = this.capturedType;
        dup.val = this.val;
        dup.bestResponse =bestResponse;
        dup.queened =this.queened;
        return dup;
        
    }
}
