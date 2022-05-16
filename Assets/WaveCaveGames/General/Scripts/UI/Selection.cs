using UnityEngine;

namespace WaveCaveGames.UI{

	public class Selection : MonoBehaviour {

		public GameObject[] buttons;
		public string key;
		private int backup;

		void Start () {
			SetKeyValue (PlayerPrefs.GetInt (key));
		}
		public void SetKeyValue (int value) {
			for (int i = 0; i < buttons.Length; i++) {
				if (i != value) {
					buttons [i].SetActive (false);
				} else {
					buttons [i].SetActive (true);
				}
			}
			PlayerPrefs.SetInt (key, value);
		}
		public void BackupValue(){
			backup = PlayerPrefs.GetInt(key);
		}
		public void SaveBackupValue(){
			PlayerPrefs.SetInt(key, backup);
		}
	}
}
