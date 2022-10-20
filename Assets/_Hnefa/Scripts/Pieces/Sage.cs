using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sage : BasePiece
{
    public override void Setup(Color newTeamColor, Color32 newSpriteColor, PieceManager newPieceManager)
    {
      // Call this same function in the base piece class
      base.Setup(newTeamColor, newSpriteColor, newPieceManager);

      // Stuff specific to this piece.
      // By putting a file in the Resources folder we could load it here as a sprite.
      GetComponent<Image>().sprite = Resources.Load<Sprite>("Piece/sage");
    }
}
