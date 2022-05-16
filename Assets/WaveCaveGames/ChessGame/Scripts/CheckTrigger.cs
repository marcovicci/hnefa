using UnityEngine;
using WaveCaveGames.ChessGame;

namespace WaveCaveGames.ChessGame{
	
	public class CheckTrigger : MonoBehaviour
	{
		[HideInInspector] public PieceManager piece;
		[HideInInspector] public bool castledPlace;

		void OnMouseDown(){
			ClickThis();
		}
		public void ClickThis(){
			GameManager gm = FindObjectOfType<GameManager>();
			if (piece != null) DestroyImmediate(piece.gameObject);
			if (gm.pawnPassedCheck == this) DestroyImmediate(gm.passedPawn.gameObject);
			PieceManager targetPiece = gm.clickedPiece;
			targetPiece.hasMoved = true;
			gm.FindCheck(targetPiece.transform.position).piece = null;
			gm.pieceMoveMark[0].position = targetPiece.transform.position;
			gm.pieceMoveMark[1].position = transform.position;
			targetPiece.transform.position = transform.position;
			piece = targetPiece;
			//en passant
			if (gm.pawnPassedCheck != null) {
				if (piece.transform.position != gm.pawnPassedCheck.transform.position) gm.pawnPassedCheck.piece = null;
				gm.pawnPassedCheck = null;
			}
			bool isPawnPassed = false;
			CheckTrigger check1 = gm.FindCheck(new Vector3(piece.transform.position.x - ((gm.isBlackTurn) ? gm.checkSize : -gm.checkSize), piece.transform.position.y, piece.transform.position.z));
			CheckTrigger check2 = gm.FindCheck(new Vector3(gm.pieceMoveMark[0].transform.position.x + ((gm.isBlackTurn) ? gm.checkSize : -gm.checkSize), gm.pieceMoveMark[0].transform.position.y, gm.pieceMoveMark[0].transform.position.z));
			isPawnPassed = check1 == check2;
			if (piece.pieceType == PieceType.Pawn && isPawnPassed) {
				CheckTrigger pawnPassedCheck = gm.FindCheck(new Vector3(piece.transform.position.x - ((gm.isBlackTurn) ? gm.checkSize : -gm.checkSize), piece.transform.position.y, piece.transform.position.z));
				gm.pawnPassedCheck = pawnPassedCheck;
				gm.passedPawn = piece;
			}
			//pawn promotion
			if (piece.pieceType == PieceType.Pawn && gm.FindCheck(new Vector3(piece.transform.position.x + ((gm.isBlackTurn) ? gm.checkSize : -gm.checkSize), piece.transform.position.y, piece.transform.position.z)) == null) {
				switch (PlayerPrefs.GetInt("ChessPromotedPiece")) {
				case 0:
					piece.pieceType = PieceType.Queen;
					piece.GetComponent<MeshFilter>().sharedMesh = gm.whitePiecePrefabs.queen.GetComponent<MeshFilter>().sharedMesh;
					break;
				case 1:
					piece.pieceType = PieceType.Rook;
					piece.GetComponent<MeshFilter>().sharedMesh = gm.whitePiecePrefabs.rook.GetComponent<MeshFilter>().sharedMesh;
					break;
				case 2:
					piece.pieceType = PieceType.Knight;
					piece.GetComponent<MeshFilter>().sharedMesh = gm.whitePiecePrefabs.knight.GetComponent<MeshFilter>().sharedMesh;
					break;
				case 3:
					piece.pieceType = PieceType.Bishop;
					piece.GetComponent<MeshFilter>().sharedMesh = gm.whitePiecePrefabs.bishop.GetComponent<MeshFilter>().sharedMesh;
					break;
				default:
					piece.pieceType = PieceType.Queen;
					piece.GetComponent<MeshFilter>().sharedMesh = gm.whitePiecePrefabs.queen.GetComponent<MeshFilter>().sharedMesh;
					break;
				}
			}
			//castling
			if (castledPlace) {
				if (this == gm.checkTriggers[1] || this == gm.checkTriggers[2]) {
					targetPiece = gm.blackPieces[0];
				} else if (this == gm.checkTriggers[5] || this == gm.checkTriggers[6]) {
					targetPiece = gm.blackPieces[7];
				} else if (this == gm.checkTriggers[57] || this == gm.checkTriggers[58]) {
					targetPiece = gm.whitePieces[0];
				} else if (this == gm.checkTriggers[61] || this == gm.checkTriggers[62]) {
					targetPiece = gm.whitePieces[7];
				}
				targetPiece.hasMoved = true;
				gm.FindCheck(targetPiece.transform.position).piece = null;
				CheckTrigger rookPlace = gm.FindCheck(new Vector3(transform.position.x, transform.position.y, transform.position.z + ((gm.pieceMoveMark[0].position.z > gm.pieceMoveMark[1].position.z) ? gm.checkSize : -gm.checkSize)));
				targetPiece.transform.position = rookPlace.transform.position;
				rookPlace.piece = targetPiece;
				castledPlace = false;
			}
			//call game manager functions
			gm.clickedPiece = null;
			gm.ChangePlayerTurn();
			gm.CheckIfKingUnderAttack();
			gm.CheckIfWin();
		}
	}
}
