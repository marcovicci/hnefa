using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleCell : MonoBehaviour
{
    // Shout out to Andrew Connell for his resources on Unity board games.

    // Setting up all our cute little variables

    // References to board position, gameboard object, cell size and our own RectTransforms
    // These can be hidden when not being debugged,
    // as they're not useful for inspector interaction

    //[HideInInspector]
    public int mnCellSize;
    //[HideInInspector]
    public Vector2Int mBoardPosition = Vector2Int.zero;
    //[HideInInspector]
    public GameBoard mBoard = null;
    //[HideInInspector]
    public RectTransform mRectTransform = null;
    //[HideInInspector]
    public BasePiece mCurrentPiece = null;

    //Couple ridiculous booleans for special types of tiles.
    // Only the first one can be changed during the game, the other two happen on board creation.
    public bool mDeathHappenedHere = false;
    public bool mIsThrone = false;
    public bool mIsCorner = false;

    public void Setup(Vector2Int newBoardPosition, GameBoard newBoard, int newCellSize)
    {
      // Sets all those cute variables when the object is first created.
      mBoardPosition = newBoardPosition;
      mBoard = newBoard;
      mnCellSize = newCellSize;

      mRectTransform = GetComponent<RectTransform>();
      mRectTransform.sizeDelta = new Vector2(mnCellSize, mnCellSize);

      //Sets some extra variables - is this a throne or corner cell?
      if (mBoardPosition == new Vector2Int(0,0)
      || mBoardPosition == new Vector2Int(10,0)
      || mBoardPosition == new Vector2Int(0,10)
      || mBoardPosition == new Vector2Int(10,10))
      {
        mIsCorner = true;
      }

      if (mBoardPosition == new Vector2Int(5, 5))
      {
        mIsThrone = true;
      }
    }

    public void RemovePiece()
    {
      // Very simple function to destroy the piece in this cell.
      // Using this we can allow pieces to be taken and also lead into more complex death events.

      if (mCurrentPiece != null)
      {
        mCurrentPiece.Kill();
      }
    }
}
