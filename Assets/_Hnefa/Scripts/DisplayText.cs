
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayText : MonoBehaviour
{
    // Creating a list of text to display that also serves as the conversation history
    List<string> mTextHistory = new List<string>();

    // Text itself
    public TMP_Text mText;

    public void AddText(string newText, string speakingCharacter)
    {
        // Passing this the arguments 'hello', 'Jarl' would result in... 
        // 'The Jarl: hello'
        TextUpdate("The " + speakingCharacter + " : " + newText);
    }

    public void AddRavenSpeak(string newText, string speakingCharacter)
    {
        // Passing this the arguments 'hello', 'Jarl' would result in...
        // 'The Raven, in the voice of the Jarl: hello'
        TextUpdate("The Raven, in the voice of the " + speakingCharacter + ": " + newText);
    }

    public void ConversationStarter(string speakingCharacter)
    {
        // Similar to the above but just adds a line to indicate that a conversation with the character has begun.
        // "The raven begins a conversation with the Jarl."
        TextUpdate("The raven begins a conversation with the " + speakingCharacter + ".");
    }

    public void ConversationEnder(string speakingCharacter)
    {
        // Similar to the above but for ending conversations so the text history makes sense.
        // "The conversation with the Jarl ends."
        // "***"
        // ...presumably followed by the next conversation beginning
        TextUpdate("The conversation with the " + speakingCharacter + " ends.");
        TextUpdate("***");
    }

    public void TextUpdate(string line)
    {
        mTextHistory.Add(line);
        mText.text += line + "\n";
    }

    public void EmotionChange(string speakingCharacter, string newEmotion)
    {
        // You may or may not want to use this one but it could be neat.
        // Since we're working with programmer art right now, why not have some text indication of changing emotions?
        // "The Jarl looks guilty."
        TextUpdate("The " + speakingCharacter + " looks" + newEmotion + ".");
    }


    public void ClearConversationHistory()
    {
        List<string> mTextHistory = new List<string>();
        mText.text = "";
    }
}
