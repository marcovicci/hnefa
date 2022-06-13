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
      base.Kill();

      mPieceManager.mIsKingAlive = false;
    }
}
