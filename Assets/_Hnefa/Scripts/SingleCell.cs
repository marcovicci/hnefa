using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleCell : MonoBehaviour
{
    //Shout out to Andrew Connell for his resources on Unity board games.

    //Setting up all our cute little variables

    //References to board position, gameboard object, cell size and our own RectTransforms
    //These can be hidden when not being debugged,
    //as they're not useful for inspector interaction

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

    public void Setup(Vector2Int newBoardPosition, GameBoard newBoard, int newCellSize)
    {
      //Sets all those cute variables when the object is first created.
      mBoardPosition = newBoardPosition;
      mBoard = newBoard;
      mnCellSize = newCellSize;

      mRectTransform = GetComponent<RectTransform>();
      mRectTransform.sizeDelta = new Vector2(mnCellSize, mnCellSize);
    }
}
