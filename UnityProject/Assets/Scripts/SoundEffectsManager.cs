using UnityEngine;
using System.Collections;

public class SoundEffectsManager : Entity {

	// TODO: create dictionary of soundclips
	public AudioClip jumpClip;

	private AudioSource source;

	public override void Awake () {
		base.Awake ();

		source = GetComponent<AudioSource> ();
	}

	// Use this for initialization
	public override void Start () {
		base.Start ();
	}

	public void PlaySoundClip(string key) {
		if (key.Equals ("jump"))
			source.PlayOneShot (jumpClip, 1.0f);
	}
}
