/// <summary>
/// Most people probably have no clue what this 'En Passant' move is - if you don't then google it, its a legal pawn move in chess, in which a pawn captures a pawn next to it that has just performed its double move, while move diagonally forward.
/// </summary>
[System.Serializable]
public class cgEnPassantMove :cgSimpleMove  {
    public byte attackingSquare;
    public cgEnPassantMove(byte fromp, byte tomp, sbyte posval, byte attackSquare)
        :base(fromp,tomp,posval)
    {
        attackingSquare = attackSquare;
    }
	public override cgSimpleMove duplicate(){
        cgEnPassantMove dup = new cgEnPassantMove(this.from,this.to,this.positionalVal,this.attackingSquare);
        dup.capturedType = this.capturedType;
        dup.val = this.val;
        dup.bestResponse =bestResponse;
        dup.queened =this.queened;
        return dup;
        
    }
}
