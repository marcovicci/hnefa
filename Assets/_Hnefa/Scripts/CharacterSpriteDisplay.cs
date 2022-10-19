using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSpriteDisplay : MonoBehaviour
{
    public void ShowCorrectEmotion(string emotion)
    {
        // Hunt for a face file for this emotion
        var newFace = Resources.Load<Sprite>("Faces/" + emotion);
        if (newFace != null)
        {
            GetComponent<Image>().sprite = Resources.Load<Sprite>("Faces/" + emotion);
        }
        else
        {
            // If we don't have one we'll default to Neutral to avoid things going pear-shaped
            GetComponent<Image>().sprite = Resources.Load<Sprite>("Faces/Neutral");
        }
    }
}
