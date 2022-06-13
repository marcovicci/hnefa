using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CellState
{
  // In his videos on Unity Chess, Andrew Connell uses this crazy enum
  // to mark if cells are empty, occupied by a friendly or enemy piece,
  // or are out of bounds. For future utility I have a few more states.
  // For actual hnefatafl play it doesn't matter if a piece is allied or hostile,
  // but for emotionally driven decisions it might.
  None,
  Friendly,
  Enemy,
  Free,
  OutOfBounds,
  Throne, // only the jarl can target this space! but others can pass over it
  Win, // only the jarl can target this space!
  Blood // Pieces might feel some sort of way after passing over a spot where someone died.
}

public class GameBoard : MonoBehaviour
{
    // Shout out to Andrew Connell for his resources on Unity board games.

    // Reference to our single cell prefab.
    public GameObject mCellPrefab;

    // How big is a cell?
    // How long is a piece of string?
    // Does Hungarian scope notation make sense?
    // Does any of this make sense?
    public int mnCellSize = 50;

    // Our actual setup of cells on the game board.
    // These can be hidden when not being debugged,
    // as they're not useful for inspector interaction

    //[HideInInspector]
    public SingleCell[,] mAllCells = new SingleCell[11, 11];

    public void Create()
    {
      // It's like SingleCell.Setup() but for the whole board!
      // You can see we're creating Y axis cells and X axis cells.
      // We go down each of our 11 rows and create 11 colums inside it.
      for (int y = 0; y < 11; y++)
      { // I LOVE A GOOD NESTED FOR LOOP
        for (int x = 0; x < 11; x++)
        {
          // Actually creating the cell from our prefab!
          GameObject newCell = Instantiate(mCellPrefab, transform);

          // Positioning it
          RectTransform newRectTransform = newCell.GetComponent<RectTransform>();
          // We want the pivot point centered in the cell.
          // So we're offsetting each cell by mnCellSize and then also adding half of mCellSize to accomplish that.
          newRectTransform.anchoredPosition = new Vector2((x * mnCellSize) + (mnCellSize / 2), (y * mnCellSize) + (mnCellSize / 2));

          // Properly calling that Setup() function in each cell
          mAllCells[x, y] = newCell.GetComponent<SingleCell>();
          mAllCells[x, y].Setup(new Vector2Int(x,y), this, mnCellSize);
        }
      }
    }

    public CellState ValidateCell(int targetX, int targetY)
    {
      // Let's first check if the cell is out of bounds.
      // If it is we don't have to do anything else.
      // Andrew Connell had his OOB check separated into two if statements for X and Y
      // but I don't understand why and I think he's wrong. Sorry Andrew.
      // Surely if any of these parameters are OOB everything is OOB and we can just move on!

      if (targetX < 0 || targetX > 10 || targetY < 0 || targetY > 10)
      {
        return CellState.OutOfBounds;
      }

      // Okay anyway, now that that's over with we can do real checking.
      SingleCell targetCell = mAllCells[targetX, targetY];

      // Here let's handle an occupied cell.
      if (targetCell.mCurrentPiece != null)
      {
        // Pieces have internal data on color, so we can check here if they're enemy or friendly pieces.
        // For a two player game you'd also want to check the color data of the piece you're moving.
        // But we don't care and don't need to compare anything, we know who our friends are.

        if (targetCell.mCurrentPiece.mColor == Color.white)
        {
          return CellState.Friendly;
        }

        if (targetCell.mCurrentPiece.mColor == Color.black)
        {
          return CellState.Enemy;
        }
      }

      // If the cell is unoccupied we can check if maybe a death happened here,
      // or check if it's a throne, or check if it's a win tile.
      if (targetCell.mDeathHappenedHere == true)
      {
        return CellState.Blood;
      }
      if (targetCell.mIsThrone == true)
      {
        return CellState.Throne;
      }
      if (targetCell.mIsCorner == true)
      {
        return CellState.Win;
      }
        // Okay, having failed all those ridiculous previous checks we can now
        // confidently say the cell is empty/free to move to.

        return CellState.Free;
    }
}
