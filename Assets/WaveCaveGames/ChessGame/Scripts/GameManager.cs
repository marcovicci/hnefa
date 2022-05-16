using UnityEngine;
using UnityEngine.UI;
using WaveCaveGames.ChessGame;
using WaveCaveGames.Utilities;

namespace WaveCaveGames.ChessGame{
	
	public class GameManager : MonoBehaviour
	{
		[System.Serializable] public class PieceList{
			public GameObject pawn;
			public GameObject rook;
			public GameObject knight;
			public GameObject bishop;
			public GameObject queen;
			public GameObject king;
		}
		public float checkSize;
		public CheckTrigger checkTrigger;
		public Transform checkTriggerParent;
		public Transform pieceParent;
		public PieceList whitePiecePrefabs;
		public PieceList blackPiecePrefabs;
		public Transform pieceSelectMark;
		public Transform[] pieceMoveMark;
		public Transform kingCheckMark;
		[Header("UI Elements")]
		public GameObject winGameWindow;
		public Text winnerText;
		[HideInInspector] public CheckTrigger[] checkTriggers;
		[HideInInspector] public PieceManager clickedPiece;
		[HideInInspector] public bool isBlackTurn;
		[HideInInspector] public CheckTrigger pawnPassedCheck;
		[HideInInspector] public PieceManager passedPawn;
		[HideInInspector] public PieceManager[] whitePieces;
		[HideInInspector] public PieceManager[] blackPieces;
		public const float sqrt2 = 1.414214f;
		//public CheckTrigger[] ct1;

		void Start(){
			checkTriggers = new CheckTrigger[0];
			whitePieces = new PieceManager[0];
			blackPieces = new PieceManager[0];
			float startPos = -checkSize * 3.5f;
			for (int i = 0; i < 8; i++) {
				float posX = startPos + checkSize * (float)i;
				for (int j = 0; j < 8; j++) {
					float posZ = startPos + checkSize * (float)j;
					GameObject triggerObj = Instantiate(checkTrigger.gameObject, new Vector3(posX, checkTriggerParent.position.y, posZ), Quaternion.identity, checkTriggerParent);
					triggerObj.SetActive(true);
					ArrayUtility.IncreaseArray(ref checkTriggers, triggerObj.GetComponent<CheckTrigger>());
				}
			}
			//create pieces
			GameObject pieceObj = null;
			pieceObj = Instantiate(blackPiecePrefabs.rook, checkTriggers[0].transform.position, Quaternion.identity, pieceParent);
			checkTriggers[0].piece = pieceObj.GetComponent<PieceManager>();
			ArrayUtility.IncreaseArray(ref blackPieces, pieceObj.GetComponent<PieceManager>());
			pieceObj = Instantiate(blackPiecePrefabs.knight, checkTriggers[1].transform.position, Quaternion.identity, pieceParent);
			checkTriggers[1].piece = pieceObj.GetComponent<PieceManager>();
			ArrayUtility.IncreaseArray(ref blackPieces, pieceObj.GetComponent<PieceManager>());
			pieceObj = Instantiate(blackPiecePrefabs.bishop, checkTriggers[2].transform.position, Quaternion.identity, pieceParent);
			checkTriggers[2].piece = pieceObj.GetComponent<PieceManager>();
			ArrayUtility.IncreaseArray(ref blackPieces, pieceObj.GetComponent<PieceManager>());
			pieceObj = Instantiate(blackPiecePrefabs.king, (PlayerPrefs.GetInt("ChessKingPos") == 0) ? checkTriggers[3].transform.position : checkTriggers[4].transform.position, Quaternion.identity, pieceParent);
			if (PlayerPrefs.GetInt("ChessKingPos") == 0) checkTriggers[3].piece = pieceObj.GetComponent<PieceManager>();
			else checkTriggers[4].piece = pieceObj.GetComponent<PieceManager>();
			ArrayUtility.IncreaseArray(ref blackPieces, pieceObj.GetComponent<PieceManager>());
			pieceObj = Instantiate(blackPiecePrefabs.queen, (PlayerPrefs.GetInt("ChessKingPos") == 0) ? checkTriggers[4].transform.position : checkTriggers[3].transform.position, Quaternion.identity, pieceParent);
			if (PlayerPrefs.GetInt("ChessKingPos") == 0) checkTriggers[4].piece = pieceObj.GetComponent<PieceManager>();
			else checkTriggers[3].piece = pieceObj.GetComponent<PieceManager>();
			ArrayUtility.IncreaseArray(ref blackPieces, pieceObj.GetComponent<PieceManager>());
			pieceObj = Instantiate(blackPiecePrefabs.bishop, checkTriggers[5].transform.position, Quaternion.identity, pieceParent);
			checkTriggers[5].piece = pieceObj.GetComponent<PieceManager>();
			ArrayUtility.IncreaseArray(ref blackPieces, pieceObj.GetComponent<PieceManager>());
			pieceObj = Instantiate(blackPiecePrefabs.knight, checkTriggers[6].transform.position, Quaternion.identity, pieceParent);
			checkTriggers[6].piece = pieceObj.GetComponent<PieceManager>();
			ArrayUtility.IncreaseArray(ref blackPieces, pieceObj.GetComponent<PieceManager>());
			pieceObj = Instantiate(blackPiecePrefabs.rook, checkTriggers[7].transform.position, Quaternion.identity, pieceParent);
			checkTriggers[7].piece = pieceObj.GetComponent<PieceManager>();
			ArrayUtility.IncreaseArray(ref blackPieces, pieceObj.GetComponent<PieceManager>());
			for (int i = 8; i < 16; i++) {
				pieceObj = Instantiate(blackPiecePrefabs.pawn, checkTriggers[i].transform.position, Quaternion.identity, pieceParent);
				checkTriggers[i].piece = pieceObj.GetComponent<PieceManager>();
				ArrayUtility.IncreaseArray(ref blackPieces, pieceObj.GetComponent<PieceManager>());
			}
			pieceObj = Instantiate(whitePiecePrefabs.rook, checkTriggers[56].transform.position, Quaternion.identity, pieceParent);
			checkTriggers[56].piece = pieceObj.GetComponent<PieceManager>();
			ArrayUtility.IncreaseArray(ref whitePieces, pieceObj.GetComponent<PieceManager>());
			pieceObj = Instantiate(whitePiecePrefabs.knight, checkTriggers[57].transform.position, Quaternion.identity, pieceParent);
			checkTriggers[57].piece = pieceObj.GetComponent<PieceManager>();
			ArrayUtility.IncreaseArray(ref whitePieces, pieceObj.GetComponent<PieceManager>());
			pieceObj = Instantiate(whitePiecePrefabs.bishop, checkTriggers[58].transform.position, Quaternion.identity, pieceParent);
			checkTriggers[58].piece = pieceObj.GetComponent<PieceManager>();
			ArrayUtility.IncreaseArray(ref whitePieces, pieceObj.GetComponent<PieceManager>());
			pieceObj = Instantiate(whitePiecePrefabs.king, (PlayerPrefs.GetInt("ChessKingPos") == 0) ? checkTriggers[59].transform.position : checkTriggers[60].transform.position, Quaternion.identity, pieceParent);
			if (PlayerPrefs.GetInt("ChessKingPos") == 0) checkTriggers[59].piece = pieceObj.GetComponent<PieceManager>();
			else checkTriggers[60].piece = pieceObj.GetComponent<PieceManager>();
			ArrayUtility.IncreaseArray(ref whitePieces, pieceObj.GetComponent<PieceManager>());
			pieceObj = Instantiate(whitePiecePrefabs.queen, (PlayerPrefs.GetInt("ChessKingPos") == 0) ? checkTriggers[60].transform.position : checkTriggers[59].transform.position, Quaternion.identity, pieceParent);
			if (PlayerPrefs.GetInt("ChessKingPos") == 0) checkTriggers[60].piece = pieceObj.GetComponent<PieceManager>();
			else checkTriggers[59].piece = pieceObj.GetComponent<PieceManager>();
			ArrayUtility.IncreaseArray(ref whitePieces, pieceObj.GetComponent<PieceManager>());
			pieceObj = Instantiate(whitePiecePrefabs.bishop, checkTriggers[61].transform.position, Quaternion.identity, pieceParent);
			checkTriggers[61].piece = pieceObj.GetComponent<PieceManager>();
			ArrayUtility.IncreaseArray(ref whitePieces, pieceObj.GetComponent<PieceManager>());
			pieceObj = Instantiate(whitePiecePrefabs.knight, checkTriggers[62].transform.position, Quaternion.identity, pieceParent);
			checkTriggers[62].piece = pieceObj.GetComponent<PieceManager>();
			ArrayUtility.IncreaseArray(ref whitePieces, pieceObj.GetComponent<PieceManager>());
			pieceObj = Instantiate(whitePiecePrefabs.rook, checkTriggers[63].transform.position, Quaternion.identity, pieceParent);
			checkTriggers[63].piece = pieceObj.GetComponent<PieceManager>();
			ArrayUtility.IncreaseArray(ref whitePieces, pieceObj.GetComponent<PieceManager>());
			for (int i = 48; i < 56; i++) {
				pieceObj = Instantiate(whitePiecePrefabs.pawn, checkTriggers[i].transform.position, Quaternion.identity, pieceParent);
				checkTriggers[i].piece = pieceObj.GetComponent<PieceManager>();
				ArrayUtility.IncreaseArray(ref whitePieces, pieceObj.GetComponent<PieceManager>());
			}
			//disable black piece trigger
			for (int i = 0; i < blackPieces.Length; i++) {
				if (blackPieces[i] != null) blackPieces[i].GetComponent<Collider>().enabled = false;
			}
		}
		public CheckTrigger FindCheck(Vector3 v){
			for (int i = 0; i < checkTriggers.Length; i++) {
				if ((v - checkTriggers[i].transform.position).sqrMagnitude < 0.01f) return checkTriggers[i];
			}
			return null;
		}
		public void FindCheckAndLightUp(Vector3 v, float intervalMultiplier){
			var check = FindCheck(v);
			if (check != null) {
				bool blocked = false;
				bool hasSelfPiece = check.piece != null && check.piece.color == ((isBlackTurn) ? PieceColor.Black : PieceColor.White);
				bool attacked = false;
				if (clickedPiece.pieceType != PieceType.Knight) {
					int distance = int.Parse((Vector3.Distance(clickedPiece.transform.position, v) / checkSize / intervalMultiplier).ToString("0"));
					for (int i = 1; i < distance; i++) {
						CheckTrigger ct = FindCheck(Vector3.Lerp(clickedPiece.transform.position, v, (float)i / (float)distance));
						if (!blocked && ct != null && ct.piece != null) blocked = true;
					}
				}
				if (!blocked && !hasSelfPiece && PlayerPrefs.GetInt("ChessPossibleMoves") == 1) {
					var original = check.piece;
					check.piece = clickedPiece;
					FindCheck(clickedPiece.transform.position).piece = null;
					if (IsKingUnderAttack(false)) attacked = true;
					check.piece = original;
					FindCheck(clickedPiece.transform.position).piece = clickedPiece;
				}
				if (!blocked && !hasSelfPiece && !attacked) {
					check.GetComponent<Collider>().enabled = true;
					check.GetComponent<Renderer>().enabled = true;
				}
			}
		}
		public void FindCheckAndLightUp(Vector3 v, bool castledPlace){
			var check = FindCheck(v);
			if (check != null) {
				bool blocked = false;
				bool hasSelfPiece = check.piece != null && check.piece.color == ((isBlackTurn) ? PieceColor.Black : PieceColor.White);
				bool attacked = false;
				//if (clickedPiece.pieceType != PieceType.Knight) {
					int distance = int.Parse((Vector3.Distance(clickedPiece.transform.position, v) / checkSize).ToString("0"));
					for (int i = 1; i < distance; i++) {
						CheckTrigger ct = FindCheck(Vector3.Lerp(clickedPiece.transform.position, v, (float)i / (float)distance));
						if (!blocked && ct != null && ct.piece != null) blocked = true;
					}
				//}
				if (!blocked && !hasSelfPiece && PlayerPrefs.GetInt("ChessPossibleMoves") == 1) {
					var original = check.piece;
					check.piece = clickedPiece;
					FindCheck(clickedPiece.transform.position).piece = null;
					if (IsKingUnderAttack(false)) attacked = true;
					check.piece = original;
					FindCheck(clickedPiece.transform.position).piece = clickedPiece;
				}
				if (!blocked && !hasSelfPiece && !attacked) {
					check.GetComponent<Collider>().enabled = true;
					check.GetComponent<Renderer>().enabled = true;
					check.castledPlace = castledPlace;
				}
			}
		}
		public CheckTrigger FindAndReturnCheckIfCanGetToTarget(PieceManager p, Vector3 v, float intervalMultiplier, bool checkIfAttacked){
			var check = FindCheck(v);
			if (check != null) {
				bool blocked = false;
				bool hasSelfPiece = check.piece != null && check.piece.color == ((p.color == PieceColor.Black) ? PieceColor.Black : PieceColor.White);
				bool attacked = false;
				if (p.pieceType != PieceType.Knight) {
					int distance = int.Parse((Vector3.Distance(p.transform.position, v) / checkSize / intervalMultiplier).ToString("0"));
					for (int i = 1; i < distance; i++) {
						CheckTrigger ct = FindCheck(Vector3.Lerp(p.transform.position, v, (float)i / (float)distance));
						if (!blocked && ct != null && ct.piece != null) blocked = true;
					}
				}
				if (!blocked && !hasSelfPiece && checkIfAttacked) {
					var original = check.piece;
					check.piece = p;
					FindCheck(p.transform.position).piece = null;
					if (IsKingUnderAttack(false)) attacked = true;
					check.piece = original;
					FindCheck(p.transform.position).piece = p;
				}
				if (!blocked && !hasSelfPiece && !attacked) return check;
				else return null;
			}
			return null;
		}
		public CheckTrigger FindAndReturnCheckIfCanGetToTarget(PieceManager p, Vector3 v, bool castledPlace, bool checkIfAttacked){
			var check = FindCheck(v);
			if (check != null) {
				bool blocked = false;
				bool hasSelfPiece = check.piece != null && check.piece.color == ((p.color == PieceColor.Black) ? PieceColor.Black : PieceColor.White);
				bool attacked = false;
				int distance = int.Parse((Vector3.Distance(p.transform.position, v) / checkSize).ToString("0"));
				for (int i = 1; i < distance; i++) {
					CheckTrigger ct = FindCheck(Vector3.Lerp(p.transform.position, v, (float)i / (float)distance));
					if (!blocked && ct != null && ct.piece != null) blocked = true;
				}
				if (!blocked && !hasSelfPiece && checkIfAttacked) {
					var original = check.piece;
					check.piece = p;
					FindCheck(p.transform.position).piece = null;
					if (IsKingUnderAttack(false)) attacked = true;
					check.piece = original;
					FindCheck(p.transform.position).piece = p;
				}
				if (!blocked && !hasSelfPiece && !attacked) return check;
				else return null;
			}
			return null;
		}
		public void ClickPiece(PieceManager p){
			for (int i = 0; i < checkTriggers.Length; i++) {
				checkTriggers[i].GetComponent<Collider>().enabled = false;
				checkTriggers[i].GetComponent<Renderer>().enabled = false;
			}
			clickedPiece = p;
			pieceSelectMark.GetComponent<Renderer>().enabled = true;
			pieceSelectMark.position = p.transform.position;
			switch (p.pieceType) {
			case PieceType.Pawn:
				Vector3 vector = new Vector3(p.transform.position.x + ((p.color == PieceColor.Black) ? checkSize : -checkSize), p.transform.position.y, p.transform.position.z);
				if (FindCheck(vector).piece == null)
					FindCheckAndLightUp(vector, 1f);
				vector = new Vector3(p.transform.position.x + ((p.color == PieceColor.Black) ? checkSize : -checkSize), p.transform.position.y, p.transform.position.z + checkSize);
				if (FindCheck(vector) != null && FindCheck(vector).piece != null && FindCheck(vector).piece.color == ((p.color == PieceColor.Black) ? PieceColor.White : PieceColor.Black))
					FindCheckAndLightUp(vector, 1f);
				vector = new Vector3(p.transform.position.x + ((p.color == PieceColor.Black) ? checkSize : -checkSize), p.transform.position.y, p.transform.position.z - checkSize);
				if (FindCheck(vector) != null && FindCheck(vector).piece != null && FindCheck(vector).piece.color == ((p.color == PieceColor.Black) ? PieceColor.White : PieceColor.Black))
					FindCheckAndLightUp(vector, 1f);
				vector = new Vector3(p.transform.position.x + ((p.color == PieceColor.Black) ? checkSize : -checkSize) * 2f, p.transform.position.y, p.transform.position.z);
				if (!p.hasMoved && FindCheck(vector) != null && FindCheck(vector).piece == null)
					FindCheckAndLightUp(vector, 1f);
				//en passant
				vector = new Vector3(p.transform.position.x + ((p.color == PieceColor.Black) ? checkSize : -checkSize), p.transform.position.y, p.transform.position.z + checkSize);
				if (FindCheck(vector) != null && FindCheck(vector) == pawnPassedCheck)
					FindCheckAndLightUp(vector, 1f);
				vector = new Vector3(p.transform.position.x + ((p.color == PieceColor.Black) ? checkSize : -checkSize), p.transform.position.y, p.transform.position.z - checkSize);
				if (FindCheck(vector) != null && FindCheck(vector) == pawnPassedCheck)
					FindCheckAndLightUp(vector, 1f);
				break;
			case PieceType.Rook:
				for (int i = 1; i < 8; i++) {
					Vector3 dir1Vector = new Vector3(p.transform.position.x + checkSize * (float)i, p.transform.position.y, p.transform.position.z);
					FindCheckAndLightUp(dir1Vector, 1f);
					Vector3 dir2Vector = new Vector3(p.transform.position.x - checkSize * (float)i, p.transform.position.y, p.transform.position.z);
					FindCheckAndLightUp(dir2Vector, 1f);
					Vector3 dir3Vector = new Vector3(p.transform.position.x, p.transform.position.y, p.transform.position.z + checkSize * (float)i);
					FindCheckAndLightUp(dir3Vector, 1f);
					Vector3 dir4Vector = new Vector3(p.transform.position.x, p.transform.position.y, p.transform.position.z - checkSize * (float)i);
					FindCheckAndLightUp(dir4Vector, 1f);
				}
				break;
			case PieceType.Knight:
				FindCheckAndLightUp(new Vector3(p.transform.position.x + checkSize, p.transform.position.y, p.transform.position.z + checkSize * 2f), 1f);
				FindCheckAndLightUp(new Vector3(p.transform.position.x + checkSize, p.transform.position.y, p.transform.position.z - checkSize * 2f), 1f);
				FindCheckAndLightUp(new Vector3(p.transform.position.x - checkSize, p.transform.position.y, p.transform.position.z + checkSize * 2f), 1f);
				FindCheckAndLightUp(new Vector3(p.transform.position.x - checkSize, p.transform.position.y, p.transform.position.z - checkSize * 2f), 1f);
				FindCheckAndLightUp(new Vector3(p.transform.position.x + checkSize * 2f, p.transform.position.y, p.transform.position.z + checkSize), 1f);
				FindCheckAndLightUp(new Vector3(p.transform.position.x + checkSize * 2f, p.transform.position.y, p.transform.position.z - checkSize), 1f);
				FindCheckAndLightUp(new Vector3(p.transform.position.x - checkSize * 2f, p.transform.position.y, p.transform.position.z + checkSize), 1f);
				FindCheckAndLightUp(new Vector3(p.transform.position.x - checkSize * 2f, p.transform.position.y, p.transform.position.z - checkSize), 1f);
				break;
			case PieceType.Bishop:
				for (int i = 1; i < 8; i++) {
					Vector3 dir1Vector = new Vector3(p.transform.position.x + checkSize * (float)i, p.transform.position.y, p.transform.position.z + checkSize * (float)i);
					FindCheckAndLightUp(dir1Vector, sqrt2);
					Vector3 dir2Vector = new Vector3(p.transform.position.x + checkSize * (float)i, p.transform.position.y, p.transform.position.z - checkSize * (float)i);
					FindCheckAndLightUp(dir2Vector, sqrt2);
					Vector3 dir3Vector = new Vector3(p.transform.position.x - checkSize * (float)i, p.transform.position.y, p.transform.position.z + checkSize * (float)i);
					FindCheckAndLightUp(dir3Vector, sqrt2);
					Vector3 dir4Vector = new Vector3(p.transform.position.x - checkSize * (float)i, p.transform.position.y, p.transform.position.z - checkSize * (float)i);
					FindCheckAndLightUp(dir4Vector, sqrt2);
				}
				break;
			case PieceType.Queen:
				for (int i = 1; i < 8; i++) {
					Vector3 dir1Vector = new Vector3(p.transform.position.x + checkSize * (float)i, p.transform.position.y, p.transform.position.z);
					FindCheckAndLightUp(dir1Vector, 1f);
					Vector3 dir2Vector = new Vector3(p.transform.position.x - checkSize * (float)i, p.transform.position.y, p.transform.position.z);
					FindCheckAndLightUp(dir2Vector, 1f);
					Vector3 dir3Vector = new Vector3(p.transform.position.x, p.transform.position.y, p.transform.position.z + checkSize * (float)i);
					FindCheckAndLightUp(dir3Vector, 1f);
					Vector3 dir4Vector = new Vector3(p.transform.position.x, p.transform.position.y, p.transform.position.z - checkSize * (float)i);
					FindCheckAndLightUp(dir4Vector, 1f);
					Vector3 dir5Vector = new Vector3(p.transform.position.x + checkSize * (float)i, p.transform.position.y, p.transform.position.z + checkSize * (float)i);
					FindCheckAndLightUp(dir5Vector, sqrt2);
					Vector3 dir6Vector = new Vector3(p.transform.position.x + checkSize * (float)i, p.transform.position.y, p.transform.position.z - checkSize * (float)i);
					FindCheckAndLightUp(dir6Vector, sqrt2);
					Vector3 dir7Vector = new Vector3(p.transform.position.x - checkSize * (float)i, p.transform.position.y, p.transform.position.z + checkSize * (float)i);
					FindCheckAndLightUp(dir7Vector, sqrt2);
					Vector3 dir8Vector = new Vector3(p.transform.position.x - checkSize * (float)i, p.transform.position.y, p.transform.position.z - checkSize * (float)i);
					FindCheckAndLightUp(dir8Vector, sqrt2);
				}
				break;
			case PieceType.King:
				FindCheckAndLightUp(new Vector3(p.transform.position.x + checkSize, p.transform.position.y, p.transform.position.z), 1f);
				FindCheckAndLightUp(new Vector3(p.transform.position.x - checkSize, p.transform.position.y, p.transform.position.z), 1f);
				FindCheckAndLightUp(new Vector3(p.transform.position.x, p.transform.position.y, p.transform.position.z + checkSize), 1f);
				FindCheckAndLightUp(new Vector3(p.transform.position.x, p.transform.position.y, p.transform.position.z - checkSize), 1f);
				FindCheckAndLightUp(new Vector3(p.transform.position.x + checkSize, p.transform.position.y, p.transform.position.z + checkSize), 1f);
				FindCheckAndLightUp(new Vector3(p.transform.position.x + checkSize, p.transform.position.y, p.transform.position.z - checkSize), 1f);
				FindCheckAndLightUp(new Vector3(p.transform.position.x - checkSize, p.transform.position.y, p.transform.position.z + checkSize), 1f);
				FindCheckAndLightUp(new Vector3(p.transform.position.x - checkSize, p.transform.position.y, p.transform.position.z - checkSize), 1f);
				if (!p.hasMoved) {
					if (isBlackTurn) {
						if (!blackPieces[0].hasMoved) FindCheckAndLightUp(new Vector3(p.transform.position.x, p.transform.position.y, p.transform.position.z - checkSize * 2f), true);
						if (!blackPieces[7].hasMoved) FindCheckAndLightUp(new Vector3(p.transform.position.x, p.transform.position.y, p.transform.position.z + checkSize * 2f), true);
					} else {
						if (!whitePieces[0].hasMoved) FindCheckAndLightUp(new Vector3(p.transform.position.x, p.transform.position.y, p.transform.position.z - checkSize * 2f), true);
						if (!whitePieces[7].hasMoved) FindCheckAndLightUp(new Vector3(p.transform.position.x, p.transform.position.y, p.transform.position.z + checkSize * 2f), true);
					}
				}
				break;
			}
		}
		public CheckTrigger[] PossibleTargets(PieceManager p, bool checkIfAttacked){
			CheckTrigger[] ct = new CheckTrigger[0];
			switch (p.pieceType) {
			case PieceType.Pawn:
				Vector3 vector = new Vector3(p.transform.position.x + ((p.color == PieceColor.Black) ? checkSize : -checkSize), p.transform.position.y, p.transform.position.z);
				if (FindCheck(vector).piece == null)
					ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, vector, 1f, checkIfAttacked));
				vector = new Vector3(p.transform.position.x + ((p.color == PieceColor.Black) ? checkSize : -checkSize), p.transform.position.y, p.transform.position.z + checkSize);
				if (FindCheck(vector) != null && FindCheck(vector).piece != null && FindCheck(vector).piece.color == ((p.color == PieceColor.Black) ? PieceColor.White : PieceColor.Black))
					ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, vector, 1f, checkIfAttacked));
				vector = new Vector3(p.transform.position.x + ((p.color == PieceColor.Black) ? checkSize : -checkSize), p.transform.position.y, p.transform.position.z - checkSize);
				if (FindCheck(vector) != null && FindCheck(vector).piece != null && FindCheck(vector).piece.color == ((p.color == PieceColor.Black) ? PieceColor.White : PieceColor.Black))
					ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, vector, 1f, checkIfAttacked));
				vector = new Vector3(p.transform.position.x + ((p.color == PieceColor.Black) ? checkSize : -checkSize) * 2f, p.transform.position.y, p.transform.position.z);
				if (!p.hasMoved && FindCheck(vector) != null && FindCheck(vector).piece == null)
					ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, vector, 1f, checkIfAttacked));
				//en passant
				vector = new Vector3(p.transform.position.x + ((p.color == PieceColor.Black) ? checkSize : -checkSize), p.transform.position.y, p.transform.position.z + checkSize);
				if (FindCheck(vector) != null && FindCheck(vector) == pawnPassedCheck)
					ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, vector, 1f, checkIfAttacked));
				vector = new Vector3(p.transform.position.x + ((p.color == PieceColor.Black) ? checkSize : -checkSize), p.transform.position.y, p.transform.position.z - checkSize);
				if (FindCheck(vector) != null && FindCheck(vector) == pawnPassedCheck)
					ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, vector, 1f, checkIfAttacked));
				break;
			case PieceType.Rook:
				for (int i = 1; i < 8; i++) {
					Vector3 dir1Vector = new Vector3(p.transform.position.x + checkSize * (float)i, p.transform.position.y, p.transform.position.z);
					ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, dir1Vector, 1f, checkIfAttacked));
					Vector3 dir2Vector = new Vector3(p.transform.position.x - checkSize * (float)i, p.transform.position.y, p.transform.position.z);
					ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, dir2Vector, 1f, checkIfAttacked));
					Vector3 dir3Vector = new Vector3(p.transform.position.x, p.transform.position.y, p.transform.position.z + checkSize * (float)i);
					ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, dir3Vector, 1f, checkIfAttacked));
					Vector3 dir4Vector = new Vector3(p.transform.position.x, p.transform.position.y, p.transform.position.z - checkSize * (float)i);
					ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, dir4Vector, 1f, checkIfAttacked));
				}
				break;
			case PieceType.Knight:
				ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, new Vector3(p.transform.position.x + checkSize, p.transform.position.y, p.transform.position.z + checkSize * 2f), 1f, checkIfAttacked));
				ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, new Vector3(p.transform.position.x + checkSize, p.transform.position.y, p.transform.position.z - checkSize * 2f), 1f, checkIfAttacked));
				ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, new Vector3(p.transform.position.x - checkSize, p.transform.position.y, p.transform.position.z + checkSize * 2f), 1f, checkIfAttacked));
				ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, new Vector3(p.transform.position.x - checkSize, p.transform.position.y, p.transform.position.z - checkSize * 2f), 1f, checkIfAttacked));
				ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, new Vector3(p.transform.position.x + checkSize * 2f, p.transform.position.y, p.transform.position.z + checkSize), 1f, checkIfAttacked));
				ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, new Vector3(p.transform.position.x + checkSize * 2f, p.transform.position.y, p.transform.position.z - checkSize), 1f, checkIfAttacked));
				ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, new Vector3(p.transform.position.x - checkSize * 2f, p.transform.position.y, p.transform.position.z + checkSize), 1f, checkIfAttacked));
				ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, new Vector3(p.transform.position.x - checkSize * 2f, p.transform.position.y, p.transform.position.z - checkSize), 1f, checkIfAttacked));
				break;
			case PieceType.Bishop:
				for (int i = 1; i < 8; i++) {
					Vector3 dir1Vector = new Vector3(p.transform.position.x + checkSize * (float)i, p.transform.position.y, p.transform.position.z + checkSize * (float)i);
					ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, dir1Vector, sqrt2, checkIfAttacked));
					Vector3 dir2Vector = new Vector3(p.transform.position.x + checkSize * (float)i, p.transform.position.y, p.transform.position.z - checkSize * (float)i);
					ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, dir2Vector, sqrt2, checkIfAttacked));
					Vector3 dir3Vector = new Vector3(p.transform.position.x - checkSize * (float)i, p.transform.position.y, p.transform.position.z + checkSize * (float)i);
					ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, dir3Vector, sqrt2, checkIfAttacked));
					Vector3 dir4Vector = new Vector3(p.transform.position.x - checkSize * (float)i, p.transform.position.y, p.transform.position.z - checkSize * (float)i);
					ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, dir4Vector, sqrt2, checkIfAttacked));
				}
				break;
			case PieceType.Queen:
				for (int i = 1; i < 8; i++) {
					Vector3 dir1Vector = new Vector3(p.transform.position.x + checkSize * (float)i, p.transform.position.y, p.transform.position.z);
					ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, dir1Vector, 1f, checkIfAttacked));
					Vector3 dir2Vector = new Vector3(p.transform.position.x - checkSize * (float)i, p.transform.position.y, p.transform.position.z);
					ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, dir2Vector, 1f, checkIfAttacked));
					Vector3 dir3Vector = new Vector3(p.transform.position.x, p.transform.position.y, p.transform.position.z + checkSize * (float)i);
					ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, dir3Vector, 1f, checkIfAttacked));
					Vector3 dir4Vector = new Vector3(p.transform.position.x, p.transform.position.y, p.transform.position.z - checkSize * (float)i);
					ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, dir4Vector, 1f, checkIfAttacked));
					Vector3 dir5Vector = new Vector3(p.transform.position.x + checkSize * (float)i, p.transform.position.y, p.transform.position.z + checkSize * (float)i);
					ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, dir5Vector, sqrt2, checkIfAttacked));
					Vector3 dir6Vector = new Vector3(p.transform.position.x + checkSize * (float)i, p.transform.position.y, p.transform.position.z - checkSize * (float)i);
					ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, dir6Vector, sqrt2, checkIfAttacked));
					Vector3 dir7Vector = new Vector3(p.transform.position.x - checkSize * (float)i, p.transform.position.y, p.transform.position.z + checkSize * (float)i);
					ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, dir7Vector, sqrt2, checkIfAttacked));
					Vector3 dir8Vector = new Vector3(p.transform.position.x - checkSize * (float)i, p.transform.position.y, p.transform.position.z - checkSize * (float)i);
					ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, dir8Vector, sqrt2, checkIfAttacked));
				}
				break;
			case PieceType.King:
				ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, new Vector3(p.transform.position.x + checkSize, p.transform.position.y, p.transform.position.z), 1f, checkIfAttacked));
				ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, new Vector3(p.transform.position.x - checkSize, p.transform.position.y, p.transform.position.z), 1f, checkIfAttacked));
				ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, new Vector3(p.transform.position.x, p.transform.position.y, p.transform.position.z + checkSize), 1f, checkIfAttacked));
				ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, new Vector3(p.transform.position.x, p.transform.position.y, p.transform.position.z - checkSize), 1f, checkIfAttacked));
				ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, new Vector3(p.transform.position.x + checkSize, p.transform.position.y, p.transform.position.z + checkSize), 1f, checkIfAttacked));
				ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, new Vector3(p.transform.position.x + checkSize, p.transform.position.y, p.transform.position.z - checkSize), 1f, checkIfAttacked));
				ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, new Vector3(p.transform.position.x - checkSize, p.transform.position.y, p.transform.position.z + checkSize), 1f, checkIfAttacked));
				ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, new Vector3(p.transform.position.x - checkSize, p.transform.position.y, p.transform.position.z - checkSize), 1f, checkIfAttacked));
				if (!p.hasMoved) {
					if (isBlackTurn) {
						if (!blackPieces[0].hasMoved) ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, new Vector3(p.transform.position.x, p.transform.position.y, p.transform.position.z - checkSize * 2f), true, checkIfAttacked));
						if (!blackPieces[7].hasMoved) ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, new Vector3(p.transform.position.x, p.transform.position.y, p.transform.position.z + checkSize * 2f), true, checkIfAttacked));
					} else {
						if (!whitePieces[0].hasMoved) ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, new Vector3(p.transform.position.x, p.transform.position.y, p.transform.position.z - checkSize * 2f), true, checkIfAttacked));
						if (!whitePieces[7].hasMoved) ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, new Vector3(p.transform.position.x, p.transform.position.y, p.transform.position.z + checkSize * 2f), true, checkIfAttacked));
					}
				}
				break;
			}
			return ct;
		}
		public void ChangePlayerTurn(){
			pieceSelectMark.GetComponent<Renderer>().enabled = false;
			for (int i = 0; i < checkTriggers.Length; i++) {
				checkTriggers[i].GetComponent<Collider>().enabled = false;
				checkTriggers[i].GetComponent<Renderer>().enabled = false;
			}
			isBlackTurn = !isBlackTurn;
			for (int i = 0; i < whitePieces.Length; i++) {
				if (whitePieces[i] != null) whitePieces[i].GetComponent<Collider>().enabled = !isBlackTurn;
			}
			for (int i = 0; i < blackPieces.Length; i++) {
				if (blackPieces[i] != null) blackPieces[i].GetComponent<Collider>().enabled = isBlackTurn;
			}
		}
		public bool IsKingUnderAttack(bool checkIfAttacked){
			CheckTrigger[] ct = new CheckTrigger[0];
			if (isBlackTurn) {
				for (int i = 0; i < whitePieces.Length; i++) {
					if (whitePieces[i] != null) {
						ArrayUtility.IncreaseArray(ref ct, PossibleTargets(whitePieces[i], checkIfAttacked));
					}
				}
				//ct1 = ct;
				for (int i = 0; i < ct.Length; i++) {
					if (ct[i].piece != null && ct[i].piece.pieceType == PieceType.King && ct[i].piece.color == PieceColor.Black) {
						return true;
					}
				}
			} else {
				for (int i = 0; i < blackPieces.Length; i++) {
					if (blackPieces[i] != null) {
						ArrayUtility.IncreaseArray(ref ct, PossibleTargets(blackPieces[i], checkIfAttacked));
					}
				}
				//ct1 = ct;
				for (int i = 0; i < ct.Length; i++) {
					if (ct[i].piece != null && ct[i].piece.pieceType == PieceType.King && ct[i].piece.color == PieceColor.White) {
						return true;
					}
				}
			}
			return false;
		}
		public void CheckIfKingUnderAttack(){
			kingCheckMark.transform.position = (IsKingUnderAttack(false)) ? ((isBlackTurn) ? blackPieces[3].transform.position : whitePieces[3].transform.position) : new Vector3(0f, -100f, 0f);
		}
		public void CheckIfWin(){
			if (PlayerPrefs.GetInt("ChessPossibleMoves") == 1) {
				CheckTrigger[] ct = new CheckTrigger[0];
				if (!isBlackTurn) {
					for (int i = 0; i < whitePieces.Length; i++) {
						if (whitePieces[i] != null) ArrayUtility.IncreaseArray(ref ct, PossibleTargets(whitePieces[i], true));
					}
					if (ct.Length == 0) {
						winGameWindow.SetActive(true);
						winnerText.text = (kingCheckMark.transform.position == new Vector3(0f, -100f, 0f)) ? "Draw" : "Black wins!";
					}
				} else {
					for (int i = 0; i < blackPieces.Length; i++) {
						if (blackPieces[i] != null) ArrayUtility.IncreaseArray(ref ct, PossibleTargets(blackPieces[i], true));
					}
					if (ct.Length == 0) {
						winGameWindow.SetActive(true);
						winnerText.text = (kingCheckMark.transform.position == new Vector3(0f, -100f, 0f)) ? "Draw" : "White wins!";
					}
				}
			} else {
				if (whitePieces[3] == null) {
					winGameWindow.SetActive(true);
					winnerText.text = "Black wins!";
				}
				if (blackPieces[3] == null) {
					winGameWindow.SetActive(true);
					winnerText.text = "White wins!";
				}
			}
		}
		public void BackToMenu(){
			UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
		}
		public CheckTrigger[] FindActiveTriggers(){
			CheckTrigger[] ct = new CheckTrigger[0];
			for (int i = 0; i < checkTriggers.Length; i++) {
				if (checkTriggers[i].GetComponent<Collider>().enabled)
					ArrayUtility.IncreaseArray(ref ct, checkTriggers[i]);
			}
			return ct;
		}
	}
}
