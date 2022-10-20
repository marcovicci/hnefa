using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuFunctions : MonoBehaviour
{
    public void Quit()
    {
      Application.Quit();
    }

    public void NewGame()
    {
      SceneManager.LoadScene("HnefaBoard");
    }

    public void Continue()
    {
      // Currently identical to NewGame lol
      NewGame();
    }

    public void ResumeGame()
    {
      Destroy(gameObject);
    }
}
