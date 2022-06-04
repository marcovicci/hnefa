using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameBoard : MonoBehaviour
{
    //Shout out to Andrew Connell for his resources on Unity board games.

    //Reference to our single cell prefab.
    public GameObject mCellPrefab;

    //How big is a cell?
    //How long is a piece of string?
    //Does Hungarian scope notation make sense?
    //Does any of this make sense?
    public int mnCellSize = 50;

    //Our actual setup of cells on the game board.
    //These can be hidden when not being debugged,
    //as they're not useful for inspector interaction

    //[HideInInspector]
    public SingleCell[,] mAllCells = new SingleCell[11, 11];

    public void Create()
    {
      //It's like SingleCell.Setup() but for the whole board!
      //You can see we're creating Y axis cells and X axis cells.
      //We go down each of our 11 rows and create 11 colums inside it.
      for (int y = 0; y < 11; y++)
      { // I LOVE A GOOD NESTED FOR LOOP
        for (int x = 0; x < 11; x++)
        {
          //Actually creating the cell from our prefab!
          GameObject newCell = Instantiate(mCellPrefab, transform);

          //Positioning it
          RectTransform newRectTransform = newCell.GetComponent<RectTransform>();
          //We want the pivot point centered in the cell.
          //So we're offsetting each cell by mnCellSize and then also adding half of mCellSize to accomplish that.
          newRectTransform.anchoredPosition = new Vector2((x * mnCellSize) + (mnCellSize / 2), (y * mnCellSize) + (mnCellSize / 2));

          //Properly calling that Setup() function in each cell
          mAllCells[x, y] = newCell.GetComponent<SingleCell>();
          mAllCells[x, y].Setup(new Vector2Int(x,y), this, mnCellSize);
        }
      }
    }
}
