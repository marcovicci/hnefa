using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Jarl : BasePiece
{
    public override void Setup(Color newTeamColor, Color32 newSpriteColor, PieceManager newPieceManager)
    {
      // Call this same function in the base piece class
      base.Setup(newTeamColor, newSpriteColor, newPieceManager);

      // Stuff specific to this piece.
      // First we're telling our base class we are the jarl.
      base.IsKing();

      // Sending our position to the piece manager to place the bird.

      // By putting a file in the Resources folder we could load it here as a sprite.
      GetComponent<Image>().sprite = Resources.Load<Sprite>("Piece/jarl");
    }

    // We have to override the Kill() function as well, just to make sure killing the Jarl is a loss
    public override void Kill()
    {

      if (CheckSurrounded(1, 0, mMovement.x)
      && CheckSurrounded(-1, 0, mMovement.x)
      && CheckSurrounded(0, 1, mMovement.y)
      && CheckSurrounded(0, -1, mMovement.y))
      {
        // King is surrounded so we run the base Kill here
        base.Kill();
        
        if (!mIsVirtual) 
        {
          mPieceManager.mIsKingAlive = false;
        }
        else 
        {
          mPieceManager.mDirector.GetComponent<Simulator>().mIsKingAlive = false;
        }
      }
    }

    public override void Place(SingleCell newCell)
    {
      base.Place(newCell);
      mPieceManager.MakeBird(this);
    }

    public override void Move()
    {

      // Because this is the jarl, in addition to finishing up his move
      // we need to check if it's time to win the game

      if (CheckWin())
      {

        // Hooray! We win!
        // Right now this just resets the game if the king isn't a simulated piece. 
        if (!mIsVirtual) 
        {
          mPieceManager.mIsKingFree = true;
          mPieceManager.ResetPieces();
        }
        else 
        {
          mPieceManager.mDirector.GetComponent<Simulator>().mIsKingFree = true;
        }
      }
      else
      {
        // Just continue as normal.
        // We should also send our position to the Simulator. 
        mPieceManager.mDirector.GetComponent<Simulator>().mKingPos = new Vector2Int(mCurrentCell.mBoardPosition.x, mCurrentCell.mBoardPosition.y);
        base.Move();
      }
    }

    public bool CheckSurrounded(int xDirection, int yDirection, int movement)
    {

      // While we're here, let's give the Jarl a special function
      // for determining if it's time for him to cark it or not
      // Since the jarl needs to be surrounded on all four sides!

      int currentX = mCurrentCell.mBoardPosition.x;
      int currentY = mCurrentCell.mBoardPosition.y;

      currentX += xDirection;
      currentY += yDirection;

      // the jarl can either be surrounded by 4 enemies, or 3 enemies and a throne
      if (MatchesState(currentX,currentY, CellState.Enemy) || MatchesState(currentX,currentY, CellState.Throne))
      {
        return true;
      }

      return false;

    }

    public bool CheckWin()
    {
      //Function to quickly check if we've just moved onto a winning square.

      if (MatchesState(mTargetCell.mBoardPosition.x, mTargetCell.mBoardPosition.y, CellState.Win))
      {
        return true;
      }

      return false;
    }


}
