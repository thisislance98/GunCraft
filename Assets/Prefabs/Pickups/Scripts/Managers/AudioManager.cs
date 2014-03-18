using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

	public static AudioManager Instance;

	// Use this for initialization
	void Start () {
	
		Instance = this;
	}

	public void Play(AudioClip clip)
	{
		audio.PlayOneShot(clip);

	}

}
