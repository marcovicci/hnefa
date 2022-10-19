using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Director : MonoBehaviour
{

    //Here be references
    public GameBoard mBoard;
    public PieceManager mPieceManager;

    void Start()
    {
      //Create the board!
      mBoard.Create();

      //Setup pieces
      mPieceManager.Setup(mBoard);
    }

    public void TakeAlliedMove(string mode="random")
    {
      if (mode == "random")
      {
        // code for random allied piece selection
        mPieceManager.PickRandomAlliedPiece();
      }

      else
      {
        // ... code for non-random piece selection based on variance
        mPieceManager.PickAlliedPiece();
      }
    }

    public void TakeEnemyMove()
    {
      mPieceManager.PickEnemyPiece();
    }
}
