using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BasePiece : EventTrigger
{
    // Shout out to Andrew Connell for his resources on Unity board games.
    // First of all, we're inheriting from EventTrigger here so that when it gets
    // to being able to drag your bird around the board, we can use
    // all of EventTrigger's prebuilt stuff around onDrag, onClick etc
    // without having to write any of it ourself.

    // Variables happen now.
    public Color mColor = Color.clear;
    public bool mIsKing = false;

    // Starting position and current position, also our own RectTransform again
    public SingleCell mOriginalCell = null;
    public SingleCell mCurrentCell = null;
    protected RectTransform mRectTransform = null;
    protected PieceManager mPieceManager;

    // Here's where it might get weird. For prototyping I want to be able to manually
    // move the pieces, even though this will never be done in-game.
    // So I will be starting with Andrew Connell's system to drag pieces around the board.
    protected Vector3Int mMovement = new Vector3Int(10, 10, 0);
    protected List<SingleCell> mHighlightedCells = new List<SingleCell>();

    // Later this will lead into a system for understanding possible moves per-piece.
    // It'll all make sense then. I promise.
    // On that note here's a variable for the cell we're "moving to" right now.
    protected SingleCell mTargetCell = null;

    public virtual void Setup(Color newTeamColor, Color32 newSpriteColor, PieceManager newPieceManager)
    {
      mPieceManager = newPieceManager;
      mColor = newTeamColor;
      GetComponent<Image>().color = newSpriteColor;
      mRectTransform = GetComponent<RectTransform>();
    }

    public void Place(SingleCell newCell)
    {
      // Taking values from the cell and telling it which piece is where
      mCurrentCell = newCell;
      mOriginalCell = newCell;
      mCurrentCell.mCurrentPiece = this;

      // Positioning this piece
      transform.position = newCell.transform.position;
      gameObject.SetActive(true);
    }

    public virtual void Kill()
    {
      // Handles removing this piece from play.
      // Clears data for the current cell (so it knows it's empty) and removes this gameObject.

      mCurrentCell.mCurrentPiece = null;
      gameObject.SetActive(false);
    }

    public void Reset()
    {

      // By destroying this piece and then re-placing it where it belongs, we can reset it.
      Kill();
      Place(mOriginalCell);
    }

    public void IsKing()
    {
      // A simple function allowing the jarl to be differentiated on setup.
      mIsKing = true;
    }

    private void CreateCellPath(int xDirection, int yDirection, int movement)
    {
      // Again, starting with Andrew Connell's system to drag pieces around the board.
      // Called by the function directly below.

      // Find position of current cell...
      int currentX = mCurrentCell.mBoardPosition.x;
      int currentY = mCurrentCell.mBoardPosition.y;

      // Given an amount of spaces we can move and a direction, check those cells and add them
      // to our list of possible movement cells. You may notice this is a setup for flexibility -
      // of course, in hnefatafl, we will always be able to move like a rook.
      // That will be handled by the subclasses for our types of pieces though.
      for (int i = 1; i <= movement; i++)
      {
        currentX += xDirection;
        currentY += yDirection;

        // Let's see if the cell is occupied or out of bounds here.
        // You can see the actual code for these calculations in the GameBoard class.
        CellState mCellState = CellState.None;
        mCellState = mCurrentCell.mBoard.ValidateCell(currentX,currentY);

        // We can stop here if this is an unmoveable square.
        // Using 'break;' means we don't try to move past unmovable pieces, either.
        if (mCellState == CellState.Enemy || mCellState == CellState.Friendly || mCellState == CellState.OutOfBounds)
        {
          break;
        }

        // If this piece will only be moveable to our jarl we should do a bit of additional checking.
        if (mCellState == CellState.Win && mIsKing == false)
        {
          break;
        }

        if (mCellState == CellState.Throne)
        {
          // Non-jarl pieces can move over the throne but not stop on it, so we can't add it to our list
          // but we shouldn't break either.
          if (mIsKing)
          {
            // Add this cell to our list.
            mHighlightedCells.Add(mCurrentCell.mBoard.mAllCells[currentX,currentY]);
          }
        }

        if (mCellState == CellState.Free || mCellState == CellState.Blood)
        {
          // Add this cell to our list.
          mHighlightedCells.Add(mCurrentCell.mBoard.mAllCells[currentX,currentY]);
        }

      }
    }

    protected void CheckPathing()
    {
      // This is actually what is calling that function above.
      // It's a little ugly but works fine.

      // Horizontal movement
      CreateCellPath(1, 0, mMovement.x);
      CreateCellPath(-1, 0, mMovement.x);

      // Vertical movement
      CreateCellPath(0, 1, mMovement.y);
      CreateCellPath(0, -1, mMovement.y);

      // Using a Vector3Int instead of a Vector2Int you can also handle
      // diagonal movement, which would be necessary if this were chess.
      // I will preserve the code for that in these comments because the bird character
      // might be able to move diagonally, in which case this might come in handy.

      // Diagonal movement code follows...
      // CreateCellPath(1, 1, mMovement.z);
      // CreateCellPath(-1, 1, mMovement.z);
      // CreateCellPath(-1, -1, mMovement.z);
      // CreateCellPath(1, -1, mMovement.z);
    }

    protected void ShowCells()
    {
      // Changing color of the possible cells we can move to so you can see the code working.

      foreach (SingleCell cell in mHighlightedCells)
          cell.GetComponent<Image>().color = new Color32(255, 0, 0, 255);
      // I can't remember the last time I used a foreach like this
      // and the lack of curly brackets makes me so uncomfortable.
    }

    protected void ClearCells()
    {
      // Changing color of cells back to white.

      foreach (SingleCell cell in mHighlightedCells)
          cell.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
      // I can't remember the last time I used a foreach like this
      // and the lack of curly brackets makes me so uncomfortable.
      mHighlightedCells.Clear();
    }

    protected void Move()
    {
      // Handles moving a piece to a new cell on the board.
      // Updates the current cell to be empty, then swaps to the target cell.
      // Finally, updates our position visually to snap to the new cell and clears the target.
      // Currently, targeting is done via dragging, but later it will be via AI.
      mCurrentCell.mCurrentPiece = null;
      mCurrentCell = mTargetCell;
      mCurrentCell.mCurrentPiece = this;

      transform.position = mCurrentCell.transform.position;
      mTargetCell = null;
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
      // Here come those drag events!
      // What a drag!
      // Haha.
      // I've been dizzy and nauseous for four days straight so this is my sense of humor now.
      // I also need you to know my t key popped off the keyboard so I've been copying and pasting it as I code.
      // Anyway, this is why we inherit from the EventTrigger class allll the way up there.

      base.OnBeginDrag(eventData);

      // Checking for all those possible cells
      CheckPathing();

      //Making them red so we can see them
      ShowCells();
    }

    public override void OnDrag(PointerEventData eventData)
    {
      // Meanwhile this function makes the piece follow your mouse cursor.
      // As you can imagine, this won't be useful in real Hnefa. Bah.
      // But it will later have bits repurposed below - for retargeting.
      base.OnDrag(eventData);
      transform.position += (Vector3)eventData.delta;

      // Here's that retargeting code.
      // It checks our valid moves and sees if our mouse is in any of them.
      // It's such a cute little solution that I'm gonna be sad when I have to gut it
      // and make the AI figure this stuff out instead.
      foreach (SingleCell cell in mHighlightedCells)
      {
        if (RectTransformUtility.RectangleContainsScreenPoint(cell.mRectTransform, Input.mousePosition))
        {
          mTargetCell = cell;
          break;
        }

        // We break up there to avoid repeated calls so we don't even
        // need an 'else' statement here. But this is basically our 'else'.
        mTargetCell = null;
      }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
      base.OnEndDrag(eventData);

      // Clears those red cells from earlier.
      ClearCells();

      // Here's our code for snapping to the grid.
      // If we haven't got a new target cell from this dragging event,
      // just snap back to the original cell.
      // Otherwise we can call that move event we made earlier.
      // Instead of an else statement here you can also 'return;' after setting
      // the transform position, but I can't figure out if it's preferable
      // so I'm using if / else for readability.

      if (!mTargetCell)
      {
        transform.position = mCurrentCell.gameObject.transform.position;
      }
      else
      {
        Move();

        //If we move successfully, we also want to switch sides.
        mPieceManager.SwitchSides(mColor);
      }

    }
}
