using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BasePiece : EventTrigger
{
    //Shout out to Andrew Connell for his resources on Unity board games.
    //First of all, we're inheriting from EventTrigger here so that when it gets
    //to being able to drag your bird around the board, we can use
    //all of EventTrigger's prebuilt stuff around onDrag, onClick etc
    //without having to write any of it ourself.

    //Variables happen now.
    public Color mColor = Color.clear;

    //Starting position and current position, also our own RectTransform again
    public SingleCell mOriginalCell = null;
    public SingleCell mCurrentCell = null;
    protected RectTransform mRectTransform = null;

    protected PieceManager mPieceManager;

    public virtual void Setup(Color newTeamColor, Color32 newSpriteColor, PieceManager newPieceManager)
    {
      mPieceManager = newPieceManager;
      mColor = newTeamColor;
      GetComponent<Image>().color = newSpriteColor;
      mRectTransform = GetComponent<RectTransform>();
    }

    public void Place(SingleCell newCell)
    {
      //Taking values from the cell and telling it which piece is where
      mCurrentCell = newCell;
      mOriginalCell = newCell;
      mCurrentCell.mCurrentPiece = this;

      //Positioning this piece
      transform.position = newCell.transform.position;
      gameObject.SetActive(true);
    }
}
