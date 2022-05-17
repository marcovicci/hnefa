using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// This is the main menu script attached to the main menu prefab, it has a few buttons, check boxes and a slider to control game mode and difficulty.
/// </summary>
public class cgMenuScript : MonoBehaviour {

    /// <summary>
    /// The Difficulty Slider(min =1, max = 3)
    /// </summary>
    public Slider DifficultySlider;

    /// <summary>
    /// Is a human controlling the white pieces?
    /// </summary>
    public Toggle HumanAsWhite;

    /// <summary>
    /// Is a human controlling the black pieces?
    /// </summary>
    public Toggle HumanAsBlack;

    /// <summary>
    /// Is a human controlling the black pieces?
    /// </summary>
    public Toggle DisplayAs3D;

    /// <summary>
    /// The textfield to display the current mode, based on what colors are controlled by human.
    /// </summary>
    public Text ModeDisplayText;

    /// <summary>
    /// The textfield to display the current mode, based on what colors are controlled by human.
    /// </summary>
    public Dropdown BoardType;

    /// <summary>
    /// The chessboard prefab
    /// </summary>
    public GameObject ChessBoard;
    /// <summary>
    /// The chessboard prefab
    /// </summary>
    public GameObject ChessBoardTiny;

    /// <summary>
    /// The current mode.
    /// </summary>
    public cgChessBoardScript.BoardMode currentBoardMode;

    private byte[] _weakDepthDifficulties = {3,3,4,4};
    private byte[] _strongDepthDifficulties = {3,4,4,5};
	// Use this for initialization
	void Start () {
        ToggleMode();
	}

    /// <summary>
    /// Difficulty slider changed.
    /// </summary>
    public void DifficultyChanged()
    {
        DifficultySlider.gameObject.GetComponentInChildren<Text>().text = "Difficulty: " + DifficultySlider.value;
    }
    /// <summary>
    /// Start the game, initialize the chessboard prefab.
    /// </summary>
    public void Play()
    {
        cgChessBoardScript newboard = GameObject.Instantiate((this.BoardType.value == 0 ? ChessBoard : ChessBoardTiny)).GetComponent<cgChessBoardScript>();
        newboard.searchDepthStrong = _strongDepthDifficulties[(int)DifficultySlider.value - 1];
        newboard.searchDepthWeak = _weakDepthDifficulties[(int)DifficultySlider.value - 1];
        cgEngine newengine = newboard.getEngine;
        newboard.Mode = currentBoardMode;
        newboard.displayAs3d = DisplayAs3D.isOn;
        newboard.start();
        //newobj.GetComponent

        GameObject.DestroyImmediate(gameObject);
    }

    /// <summary>
    /// Toggle which colors human controls.
    /// </summary>
    public void ToggleMode()
    {
        if (HumanAsBlack.isOn && HumanAsWhite.isOn) currentBoardMode = cgChessBoardScript.BoardMode.PlayerVsPlayer;
        else if (HumanAsBlack.isOn && !HumanAsWhite.isOn) currentBoardMode = cgChessBoardScript.BoardMode.EngineVsPlayer;
        else if (!HumanAsBlack.isOn && HumanAsWhite.isOn) currentBoardMode = cgChessBoardScript.BoardMode.PlayerVsEngine;
        else if (!HumanAsBlack.isOn && !HumanAsWhite.isOn) currentBoardMode = cgChessBoardScript.BoardMode.EngineVsEngine;

        //Change the mode display text according to the current board mode.
        if (currentBoardMode == cgChessBoardScript.BoardMode.PlayerVsPlayer)
        {
            DifficultySlider.enabled = false;
            
            ModeDisplayText.text = "Player vs Player";
        }
        else
        {
            if (currentBoardMode == cgChessBoardScript.BoardMode.PlayerVsEngine) ModeDisplayText.text = "Player vs Engine";
            if (currentBoardMode == cgChessBoardScript.BoardMode.EngineVsPlayer) ModeDisplayText.text = "Engine vs Player";
            if (currentBoardMode == cgChessBoardScript.BoardMode.EngineVsEngine) ModeDisplayText.text = "Engine vs Engine";

            DifficultySlider.enabled = true;
        }
    }

    /// <summary>
    /// Quit the game.
    /// </summary>
    public void Quit()
    {
        Application.Quit();
        
    }
}
