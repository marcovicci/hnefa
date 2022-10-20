using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public string mCurrentPiece;
    public int[] mCurrentEASA = new int[4];
    public GameObject CharacterPortrait;
    public GameObject TextBox;
    public GameObject mDirector;
    public int ConversationCount;
    public string[] AllEmotions = new string[26]
    {"Aggressive","Alarm","Anger","Anxiety","Aware",
    "Contempt","Curiosity","Cynicism","Delight","Despair",
    "Disappointed","Disbelief","Disgust","Distracted","Dominance",
    "Envy","Fear","Guilty","Happy","Love",
    "Morbidity","Neutral","Nostalgic","Optimism","Outrage",
    "Pessimistic"
    };

    void Start()
    {
        mDirector = GameObject.Find("GameDirectorPrefab");
        mDirector.GetComponent<Director>().ContinueConversationScene(this);
    }

    public void StartConversation(BasePiece currentPiece)
    {
        TextBox.GetComponent<DisplayText>().ClearConversationHistory();
        CharacterPortrait.GetComponent<CharacterSpriteDisplay>().ShowCorrectEmotion("Neutral");
        // Copy in important values from current piece at start of dialogue
        mCurrentPiece = currentPiece.name;
        mCurrentEASA = currentPiece.mCurrentEASA;

        // Start conversation
        TextBox.GetComponent<DisplayText>().ConversationStarter(mCurrentPiece);
        
    }

    public void PlayerTalking(string dialogue, int[] easa, string voice)
    {
        // Called when the player selects dialogue
        TextBox.GetComponent<DisplayText>().AddRavenSpeak(dialogue, voice);
    }

    public void PlayerTalkingTest()
    {
        TextBox.GetComponent<DisplayText>().AddRavenSpeak("Caw.", "raven itself");
        StartCoroutine(ShortDelayForTesting());    
    }

    IEnumerator ShortDelayForTesting()
    {
      yield return new WaitForSeconds(1);
      CharacterTalking("Hmm.");
      UpdateEmotion();
    }

    public void CharacterTalking(string dialogue)
    {
        TextBox.GetComponent<DisplayText>().AddText(dialogue, mCurrentPiece);

    }

    public void UpdateEmotion()
    {
        // emotional matrix calcs will go here
        // for now we just set it to neutral
        var EmotionalResult = AllEmotions[Random.Range(0, AllEmotions.Length)];

        // Adjust character EASA values here (in a separate function perhaps?)
        // To display any emotional change...
        //TextBox.GetComponent<DisplayText>().EmotionChange(mCurrentPiece, EmotionalResult);

        CharacterPortrait.GetComponent<CharacterSpriteDisplay>().ShowCorrectEmotion(EmotionalResult);

        // full list of emotional options follows according to the filenames of the faces
        //Aggressive
        //Alarm
        //Anger
        //Anxiety
        //Aware
        //Contempt
        //Curiosity
        //Cynicism
        //Delight
        //Despair
        //Disappointed
        //Disbelief
        //Disgust
        //Distracted
        //Dominance
        //Envy
        //Fear
        //Guilty
        //Happy
        //Love
        //Morbidity
        //Neutral
        //Nostalgic
        //Optimism
        //Outrage
        //Pessimistic
        //Pride
        //Remorse
        //Sad
        //Shame
        //Sorrow
        //Submission
        //Trust
    }

    public void EndConversation()
    {
        TextBox.GetComponent<DisplayText>().ConversationEnder(mCurrentPiece);

        // activate the close button here?
    }

    public void CloseConversation()
    {
        mDirector.GetComponent<Director>().AllConversationsEnded();
    }

    public void NextConversation()
    {
        mDirector.GetComponent<Director>().ContinueConversationScene(this);
    }

    
}
