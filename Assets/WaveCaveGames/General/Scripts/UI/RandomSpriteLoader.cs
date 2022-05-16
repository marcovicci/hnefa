using UnityEngine;
using UnityEngine.UI;

namespace WaveCaveGames.UI{

	public class RandomSpriteLoader : MonoBehaviour {

		public Sprite[] sprites;

		void Start(){
			if (GetComponent<Image>() != null && sprites != null && sprites.Length > 0)
				GetComponent<Image>().sprite = sprites[Random.Range(0,sprites.Length)];
		}
	}
}
