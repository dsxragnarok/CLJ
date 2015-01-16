using UnityEngine;
using System.Collections;

public class SoundEffectsManager : Entity {

	public AudioClip jumpClip;

	private AudioSource source;

	void Awake () {
		base.Awake ();

		source = GetComponent<AudioSource> ();
	}

	// Use this for initialization
	void Start () {
		base.Start ();
	}

	public void PlaySoundClip(string key) {
		if (key.Equals ("jump"))
			source.PlayOneShot (jumpClip, 1.0f);
	}
}
