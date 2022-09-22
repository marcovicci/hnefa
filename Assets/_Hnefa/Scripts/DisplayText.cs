
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayText : MonoBehaviour
{
    // Creating a list of text to display
    List<string> mTextHistory = new List<string>();

    public void AddText(string newText, string speakingCharacter)
    {
        // Passing this the arguments 'hello', 'Jarl' would result in adding 'Jarl: hello' to the list
        mTextHistory.Add(speakingCharacter + ": " + newText);
    }
}
