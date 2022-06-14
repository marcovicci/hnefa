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
      // By putting a file in the Resources folder we could load it here as a sprite.
      // GetComponent<Image>().sprite = Resources.Load<Sprite>("File_Name")
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

        mPieceManager.mIsKingAlive = false;
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


}
