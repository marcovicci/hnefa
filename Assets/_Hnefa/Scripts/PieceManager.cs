using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PieceManager : MonoBehaviour
{
    //here be references
    public GameObject mPiecePrefab;

    //Lists for pieces
    public BasePiece[] mWhitePieces = new BasePiece[121];
    public BasePiece[] mBlackPieces = new BasePiece[121];

    //Our whole starting board layout in an array. 121 cells! Good God.
    //Subject to change when Matthew decides where characters start.
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

    }

    private BasePiece[] CreatePieces(Color teamColor, Color32 spriteColor, GameBoard mBoard)
    {
      BasePiece[] newPieces = new BasePiece[121];

      for (int i = 0; i < mBoardLayout.Length; i++)
      {
          //Get the piece in this cell of the board
          string key = mBoardLayout[i];
          //We need to handle both black and white pieces.
          //If we're doing white pieces, we need everything but blank and enemy squares.
          //If we're doing black pieces we need to do the exact same stuff but with only enemy pieces.
          if (
          //I LOVE A GOOD NESTED PARENTHESIS
          (teamColor == Color.white && key != "NONE" && key != "ENEM")
          || //this is a load bearing "or" operator
          (teamColor == Color.black && key == "ENEM")
          )
          {
            //Make a new object for this piece
            //Parenting it to the Director will help with making move decisions later
            GameObject newPieceObject = Instantiate(mPiecePrefab);
            newPieceObject.transform.SetParent(transform);

            //scale and positioning nonsense
            newPieceObject.transform.localScale = new Vector3(1,1,1);
            newPieceObject.transform.localRotation = Quaternion.identity;

            //Figure out the piece type based on that silly dictionary above
            //and correctly add a component script for each type of piece
            Type pieceType = mPieceLibrary[key];
            BasePiece newPiece = (BasePiece)newPieceObject.AddComponent(pieceType);

            //Pop that piece in this list and set it up
            newPieces[i] = newPiece;
            newPiece.Setup(teamColor, spriteColor, this);
          } else {
            newPieces[i] = null;
          }
      }
      //Give those new lists back to Setup()
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
          }
          i++;
        }
      }
    }
}
