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
}
