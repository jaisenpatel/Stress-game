using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent (typeof(AudioSource))]
public class PlayVideo : MonoBehaviour {

	// Use this for initializationd
	public MovieTexture movie;
	private AudioSource audio1 ;
	void Start () {
		GetComponent<RawImage> ().texture = movie as MovieTexture;
		audio1 = GetComponent<AudioSource> ();
		audio1.clip = movie.audioClip;
		movie.Play();
		audio1.Play();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space) && movie.isPlaying) {

			movie.Pause();

		}
		else if (Input.GetKeyDown (KeyCode.Space) && !movie.isPlaying) {

			movie.Play();

		}
			
	}
}
