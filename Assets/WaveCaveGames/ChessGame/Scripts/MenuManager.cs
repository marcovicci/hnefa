using UnityEngine;

namespace WaveCaveGames.ChessGame{
	
	public class MenuManager : MonoBehaviour
	{
		void Awake(){
			if (!PlayerPrefs.HasKey("ChessPossibleMoves"))
				PlayerPrefs.SetInt("ChessPossibleMoves", 1);
		}
		public void LoadScene(string sceneName){
			UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
		}
		public void Quit(){
			#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
			#else
			Application.Quit();
			#endif
		}
		public void OpenURL(string url){
			Application.OpenURL(url);
		}
	}
}
