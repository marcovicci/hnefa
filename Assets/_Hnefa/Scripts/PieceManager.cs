using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PieceManager : MonoBehaviour
{
    // here be references
    public GameObject mPiecePrefab;
    public GameObject mDirector;
    public GameObject mGameBoard;

    // How's the jarl doing? Is he OK? Nothing else matters if he isn't.
    public bool mIsKingAlive = true;
    
    // On the other hand, is he in a corner already?
    public bool mIsKingFree = false;

    // Lists for pieces
    public BasePiece[] mWhitePieces = new BasePiece[121];
    public BasePiece[] mBlackPieces = new BasePiece[121];

    // This next one is weird - it's storing the white pieces, black pieces and cells.
    // In theory we can use this to roll back to previous states and build the board from there.
    public List<(BasePiece[], BasePiece[], SingleCell[,])> mBoardHistory;

    // Our whole starting board layout in an array. 121 cells! Good God.
    // Subject to change when Matthew decides where characters start.
    private string[] mBoardLayout = new string[121]
    {
      "NONE", "NONE", "NONE", "ENEM", "ENEM", "ENEM", "ENEM", "ENEM", "NONE", "NONE", "NONE",
      "NONE", "NONE", "NONE", "NONE", "NONE", "ENEM", "NONE", "NONE", "NONE", "NONE", "NONE",
      "NONE", "NONE", "NONE", "NONE", "NONE", "NONE", "NONE", "NONE", "NONE", "NONE", "NONE",
      "ENEM", "NONE", "NONE", "NONE", "NONE", "ARTI", "NONE", "NONE", "NONE", "NONE", "ENEM",
      "ENEM", "NONE", "NONE", "NONE", "SEEK", "TRIC", "COWA", "NONE", "NONE", "NONE", "ENEM",
      "ENEM", "ENEM", "NONE", "SAGE", "HERO", "JARL", "OUTL", "ORPH", "NONE", "ENEM", "ENEM",
      "ENEM", "NONE", "NONE", "NONE", "WITC", "MOTH", "LOVE", "NONE", "NONE", "NONE", "ENEM",
      "ENEM", "NONE", "NONE", "NONE", "NONE", "CHIL", "NONE", "NONE", "NONE", "NONE", "ENEM",
      "NONE", "NONE", "NONE", "NONE", "NONE", "NONE", "NONE", "NONE", "NONE", "NONE", "NONE",
      "NONE", "NONE", "NONE", "NONE", "NONE", "ENEM", "NONE", "NONE", "NONE", "NONE", "NONE",
      "NONE", "NONE", "NONE", "ENEM", "ENEM", "ENEM", "ENEM", "ENEM", "NONE", "NONE", "NONE"
    };

    //Dictionary linking the above to our library of piece types
    private Dictionary<string, Type> mPieceLibrary = new Dictionary<string, Type>()
    {
      {"ARTI", typeof(Artist)},
      {"TRIC", typeof(Trickster)},
      {"MOTH", typeof(Mother)},
      {"SEEK", typeof(Seeker)},
      {"CHIL", typeof(Child)},
      {"COWA", typeof(Coward)},
      {"SAGE", typeof(Sage)},
      {"HERO", typeof(Hero)},
      {"OUTL", typeof(Outlaw)},
      {"ORPH", typeof(Orphan)},
      {"WITC", typeof(Witch)},
      {"LOVE", typeof(Lover)},
      {"JARL", typeof(Jarl)},
      {"ENEM", typeof(Enemy)}
    };

    public void Setup(GameBoard mBoard)
    {
      mWhitePieces = CreatePieces(Color.white, new Color32(80,124,159,255), mBoard);
      mBlackPieces = CreatePieces(Color.black, new Color32(210,95,64,255), mBoard);

      PlacePieces(mWhitePieces, mBoard);
      PlacePieces(mBlackPieces, mBoard);

      // Saying who goes first - in the case of fetlar hnefatafl it's the attackers.
      // Due to black magic we are passing in the allied color though. Don't worry about it, just read on.
      SwitchSides(Color.white);

    }

    private BasePiece[] CreatePieces(Color teamColor, Color32 spriteColor, GameBoard mBoard)
    {
      BasePiece[] newPieces = new BasePiece[121];

      for (int i = 0; i < mBoardLayout.Length; i++)
      {
          // Get the piece in this cell of the board
          string key = mBoardLayout[i];
          // We need to handle both black and white pieces.
          // If we're doing white pieces, we need everything but blank and enemy squares.
          // If we're doing black pieces we need to do the exact same stuff but with only enemy pieces.
          if (
          // I LOVE A GOOD NESTED PARENTHESIS
          (teamColor == Color.white && key != "NONE" && key != "ENEM")
          || // this is a load bearing "or" operator
          (teamColor == Color.black && key == "ENEM")
          )
          {
            // Make a new object for this piece
            // Parenting it to the Director will help with making move decisions later
            GameObject newPieceObject = Instantiate(mPiecePrefab);
            newPieceObject.transform.SetParent(transform);

            // scale and positioning nonsense
            newPieceObject.transform.localScale = new Vector3(1,1,1);
            newPieceObject.transform.localRotation = Quaternion.identity;

            // Figure out the piece type based on that silly dictionary above
            // and correctly add a component script for each type of piece
            Type pieceType = mPieceLibrary[key];
            BasePiece newPiece = (BasePiece)newPieceObject.AddComponent(pieceType);

            // Pop that piece in this list and set it up
            newPieces[i] = newPiece;
            newPiece.Setup(teamColor, spriteColor, this);
          } else {
            newPieces[i] = null;
          }
      }
      // Give those new lists back to Setup()
      return newPieces;
    }

    private void PlacePieces(BasePiece[] thisTeam, GameBoard mBoard)
    {
      int i = 0;
      for (int y = 0; y < 11; y++)
      { // I LOVE A GOOD NESTED FOR LOOP
        for (int x = 0; x < 11; x++)
        {
          if (thisTeam[i] != null)
          {
            thisTeam[i].Place(mBoard.mAllCells[x, y]);
            thisTeam[i].mArrayPosition = i;
          }
          i++;
        }
      }
    }

    private void SetInteractive(BasePiece[] allPieces, bool value)
    {
      // Will probably be chucked out after prototyping.
      // Just lets the piece manager declare whether pieces are draggable or not.
      foreach (BasePiece piece in allPieces)
            if (piece != null)
            {
              piece.enabled = value;
            }
    }

    public void SwitchSides(Color color)
    {
      // Before we do anything else we have to check how the jarl is doing.
      // After all, if he dies, the game is over.
      if (!mIsKingAlive)
      {
        // RIP
        // At this point let's reset the pieces.
        ResetPieces();

        // Refresh that value. the king is risen! is that what christians say?
        mIsKingAlive = true;

        // We'll want to also re-initialize that white color value I mentioned way above
        // so that when the game resets it is the attacker's move again.
        color = Color.white;
      }

      // Hey, this line of code makes me feel insane.
      // But basically, we are determining whether it is the enemy turn
      // by checking color against Color.white and reversing whatever the result is.
      bool isEnemyTurn = color == Color.white ? true : false;

      // Next we're using that boolean variable to set interactivity on the pieces.
      SetInteractive(mWhitePieces, !isEnemyTurn);
      SetInteractive(mBlackPieces, isEnemyTurn);
    }

    public void ResetPieces()
    {
      // Re-initialize pieces in each team.
      mWhitePieces = CreatePieces(Color.white, new Color32(80,124,159,255), mGameBoard.GetComponent<GameBoard>());
      mBlackPieces = CreatePieces(Color.black, new Color32(210,95,64,255), mGameBoard.GetComponent<GameBoard>());

      // Goes through and moves all the pieces on each team back to their starting position.
      foreach (BasePiece piece in mWhitePieces)
            if (piece != null)
            {
              piece.Reset();
            }
      foreach (BasePiece piece in mBlackPieces)
            if (piece != null)
            {
              piece.Reset();
            }
    }
}
