using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Bird : EventTrigger
{
    // Shout out to Andrew Connell for his resources on Unity board games.
    // First of all, we're inheriting from EventTrigger here so that when it gets
    // to being able to drag your bird around the board, we can use
    // all of EventTrigger's prebuilt stuff around onDrag, onClick etc
    // without having to write any of it ourself.

    public SingleCell mOriginalCell = null;
    public SingleCell mCurrentCell = null;
    public BasePiece mJarl = null;
    public bool isMoving;
    protected RectTransform mRectTransform = null;
    protected PieceManager mPieceManager;
    public GameObject mDirector;

    // Here's where it might get weird. For prototyping I want to be able to manually
    // move the pieces, even though this will never be done in-game.
    // So I will be starting with Andrew Connell's system to drag pieces around the board.
    protected Vector3Int mMovement = new Vector3Int(10, 10, 0);
    public List<SingleCell> mHighlightedCells = new List<SingleCell>();
    public List<SingleCell> mFlightHistory = new List<SingleCell>();
    public List<int> mMovementCost = new List<int>();
    protected SingleCell mTargetCell = null;

    public int MaxEnergy = 10;
    public int mBirdEnergy;
    public int mCurrentCost = 0;

    // reference to energy script from Sarah Bridge
    public BirdEnergy energyBar;

    // line rendering nonsense
    public LineRenderer lr; 

    public List<BasePiece> mConversationQueue = new List<BasePiece>();

    public virtual void Setup(PieceManager newPieceManager, BasePiece newJarl)
    {
      mBirdEnergy = MaxEnergy;
      mJarl = newJarl;
      mPieceManager = newPieceManager;
      isMoving = false;
      lr = GetComponent<LineRenderer>();
      energyBar = mPieceManager.GetComponent<BirdEnergy>();
      mRectTransform = GetComponent<RectTransform>();
      mDirector = GameObject.Find("GameDirectorPrefab");

      
      GetComponent<Image>().sprite = Resources.Load<Sprite>("Piece/raven");

      Place(mJarl.mCurrentCell);

      EndTurn();
      
    }

    public void Place(SingleCell newCell)
    {
      // Taking values from the cell the Jarl is in
      mCurrentCell = newCell;
      mOriginalCell = newCell;

      // Positioning this piece
      transform.position = newCell.transform.position;
      gameObject.SetActive(true);
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
        // Using 'break;' means we don't try to move past unmovable positions, either.
        if (mCellState == CellState.OutOfBounds)
        {
          break;
        }

        // The bird can only land on a friendly piece.
        if (mCellState == CellState.Friendly)
        {
          mHighlightedCells.Add(mCurrentCell.mBoard.mAllCells[currentX,currentY]);
          mMovementCost.Add(i);
        }

      }
    }

    public void CheckPathing()
    {
      // This is actually what is calling that function above.
      // It's a little ugly but works fine.

      // Horizontal movement
      CreateCellPath(1, 0, mBirdEnergy);
      CreateCellPath(-1, 0, mBirdEnergy);

      // Vertical movement
      CreateCellPath(0, 1, mBirdEnergy);
      CreateCellPath(0, -1, mBirdEnergy);

      // Using a Vector3Int instead of a Vector2Int you can also handle
      // diagonal movement, which is necessary for the bird!

      // Diagonal movement code follows...
      CreateCellPath(1, 1, mBirdEnergy);
      CreateCellPath(-1, 1, mBirdEnergy);
      CreateCellPath(-1, -1, mBirdEnergy);
      CreateCellPath(1, -1, mBirdEnergy);
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
          cell.GetComponent<Image>().color = new Color32(188, 168, 151, 255);
      // I can't remember the last time I used a foreach like this
      // and the lack of curly brackets makes me so uncomfortable.
      mHighlightedCells.Clear();
    }

    public virtual void Move()
    {
      // Handles moving bird to a new cell on the board.
      // Swaps to the target cell, updates our position visually to snap to the new cell and clears the target.
      mCurrentCell = mTargetCell;

      transform.position = mCurrentCell.transform.position;
      mTargetCell = null;
    }

    public void NewTurn()
    {
      // Resetting some values
      mBirdEnergy = MaxEnergy;
      mFlightHistory = new List<SingleCell>();
      Place(mJarl.mCurrentCell);
    }

    public void EndTurn()
    {
      gameObject.SetActive(false);
    }

    public void DrawFlightPath()
    {

      if (mBirdEnergy <= 0)
      {
        // Turn over, let's get out of here. 
        mDirector.GetComponent<Director>().StartConversationScene(mConversationQueue);
        EndTurn();

      }
      else
        {
        //lr.positionCount = mConversationQueue.Count;
        //lr.sortingLayerName = "Line";
        //lr.sortingOrder = 50;
        //foreach (BasePiece piece in mConversationQueue)
        //{
        //  lr.SetPosition(mConversationQueue.IndexOf(piece), piece.gameObject.transform.position);
        //}
      }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
      if (isMoving)
      {
        Debug.Log("is moving");
        foreach (SingleCell cell in mHighlightedCells)
        {
          if (RectTransformUtility.RectangleContainsScreenPoint(cell.mRectTransform, Input.mousePosition))
          {
            mTargetCell = cell;
            // Grabs the 'cost' of moving to this cell, sets it to current cell
            mCurrentCost = mMovementCost[mHighlightedCells.IndexOf(cell)];
            Debug.Log("breaking");
            break;
          }
          Debug.Log("did not break");
          // We break up there to avoid repeated calls so we don't even
          // need an 'else' statement here. But this is basically our 'else'.
          mTargetCell = null;
        }

        // Clears those red cells from earlier.
        ClearCells();

        // Here's our code for snapping to the grid.
        // If we haven't got a new target cell from this event,
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

          //If we move successfully, we also want to do resource stuff here...
          mBirdEnergy -= mCurrentCost;
          Debug.Log("Movement cost " + mCurrentCost + " and energy is now " + mBirdEnergy);
          energyBar.SetEnergy(mBirdEnergy);

          // Add the piece on this square to the conversation queue
          mConversationQueue.Add(mCurrentCell.mCurrentPiece);

          isMoving = false;
          DrawFlightPath();
        }
      }
      else
      {

     
      
      // Checking for all those possible cells
      CheckPathing();

      //Making them red so we can see them
      ShowCells();

      isMoving = true;

      }
      
    }

}
