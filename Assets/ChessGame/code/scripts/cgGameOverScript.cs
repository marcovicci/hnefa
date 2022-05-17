using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
/// <summary>
/// This is the game over script attached to the game over prefab, it shows a simple text and a few buttons to take the player back to a new game or the menu.
/// </summary>
public class cgGameOverScript : MonoBehaviour {
    /// <summary>
    /// The text to display win/lose/draw message.
    /// </summary>
    public Text displayText;

    /// <summary>
    /// Reset button.
    /// </summary>
    public Button ResetButton;

    /// <summary>
    /// Main menu button
    /// </summary>
    public Button MainMenuButton;

    private Action _resetBoard;
    private Action _mainMenu;
	
    /// <summary>
    /// initialize the prefab, provide callback functions and display a win/lose/draw message.
    /// </summary>
    /// <param name="text">win/lose/draw message</param>
    /// <param name="resetBoard">Reset board callback</param>
    /// <param name="mainMenu">Go to main menu callback</param>
    public void initialize(string text,Action resetBoard,Action mainMenu)
    {
        displayText.text = text;
        _resetBoard = resetBoard;
        _mainMenu = mainMenu;
    }
	// Update is called once per frame
	void Update () {
	    
	}

    /// <summary>
    /// Reset board, if a callback has been provided then call it.
    /// </summary>
    public void ResetBoard()
    {
        if (_resetBoard != null) _resetBoard();
    }

    /// <summary>
    /// Go to main menu, if a callback has been provided then call it.
    /// </summary>
    public void MainMenu()
    {
        if (_mainMenu != null) _mainMenu();
    }
}
