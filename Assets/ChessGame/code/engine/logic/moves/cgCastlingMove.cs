
/// <summary>
/// Castling moves the king and a rook, this class has additional properties to handle this additional information.
/// </summary>
[System.Serializable]
public class cgCastlingMove : cgSimpleMove
{
    /// <summary>
    /// The square the rook being castled is departing from.
    /// </summary>
    public byte secondFrom;

    /// <summary>
    /// The square the rook being castled will arrive at.
    /// </summary>
    public byte secondTo;
    public cgCastlingMove(byte fromp, byte top, sbyte posVal,byte s_from, byte s_to)
        :base(fromp,top,posVal)
    {
        secondFrom = s_from;
        secondTo = s_to;
    }
    public override cgSimpleMove duplicate(){
        cgCastlingMove dup = new cgCastlingMove(this.from,this.to,this.positionalVal,this.secondFrom,this.secondTo);
        dup.capturedType = this.capturedType;
        dup.val = this.val;
        dup.bestResponse =bestResponse;
        dup.queened =this.queened;
        return dup;
        
    }
}

