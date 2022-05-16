using UnityEngine;

namespace WaveCaveGames.ChessGame{

	public enum PieceType{
		Pawn, Rook, Knight, Bishop, Queen, King
	}
	public enum PieceColor{
		White, Black
	}
	public class PieceManager : MonoBehaviour
	{
		public PieceType pieceType;
		public PieceColor color;
		[HideInInspector] public bool hasMoved;

		void OnMouseDown(){
			ClickThis();
		}
		public void ClickThis(){
			GameManager gm = FindObjectOfType<GameManager>();
			gm.ClickPiece(this);
		}
	}
}
