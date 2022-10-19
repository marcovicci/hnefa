using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Director : MonoBehaviour
{

    //Here be references
    public GameBoard mBoard;
    public PieceManager mPieceManager;
    public DialogueManager DialogueWindow;
    public List<BasePiece> mConversationQueue;

    void Start()
    {
      //Create the board!
      mBoard.Create();

      //Setup pieces
      mPieceManager.Setup(mBoard);
    }

    public void TakeAlliedMove(string mode="random")
    {
      if (mode == "random")
      {
        // code for random allied piece selection
        mPieceManager.PickRandomAlliedPiece();
      }

      else
      {
        // ... code for non-random piece selection based on variance
        mPieceManager.PickAlliedPiece();
      }
    }

    public void TakeEnemyMove()
    {
      mPieceManager.PickEnemyPiece();
    }

    public void StartConversationScene(List<BasePiece> ConversationQueue)
    {
      
      // Copy in our list of characters
      mConversationQueue = ConversationQueue;
      
      // Load the dialogue scene
      SceneManager.LoadScene("DialogueWindow", LoadSceneMode.Additive);

    }

    public void ContinueConversationScene(DialogueManager window)
    {
      // Fetch dialogue window
      DialogueWindow = window;

      // Send next character to dialogue window
      DialogueWindow.StartConversation(mConversationQueue[0]);

      // Pop it off the list
      mConversationQueue.RemoveAt(0);
    }

    public void AllConversationsEnded()
    {
      // This will be called before the dialogue manager deletes itself, to continue the flow of the game.
      TakeAlliedMove();
    }
}
