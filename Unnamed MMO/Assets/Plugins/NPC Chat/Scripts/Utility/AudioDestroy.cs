using UnityEngine;
using System.Collections;

public class AudioDestroy : MonoBehaviour {

	private AudioSource audioRef;

	void Start(){
		audioRef = GetComponent<AudioSource> ();
	}

	void Update () {
		if (audioRef.isPlaying) {			
		} else {
			Destroy(gameObject);
		}
	}
}