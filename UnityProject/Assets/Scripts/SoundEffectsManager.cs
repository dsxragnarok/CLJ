using UnityEngine;
using System.Collections.Generic;

public class SoundEffectsManager : Entity {

	public List<SoundClip> soundClips;
	private Dictionary<string, AudioClip> soundMap = new Dictionary<string, AudioClip>(10);
	private AudioSource source;

    private float masterVolume;

    public float MasterVolume { set { masterVolume = value; } get { return masterVolume; } }

	[System.Serializable]
	public struct SoundClip
	{
		public string name;
		public AudioClip clip;
	}

	public override void Awake () {
		base.Awake ();
		source = GetComponent<AudioSource> ();

		// populate the dictionary from the serialized soundclip list
		for (int i = 0; i < soundClips.Count; i += 1) {
			soundMap.Add(soundClips[i].name, soundClips[i].clip);
		}

        
        //masterVolume = 1.0f;
	}

	// Use this for initialization
	public override void Start () {
		base.Start ();
        //masterVolume = 1.0f;
        masterVolume = gameMaster.Settings.MasterVolume;
	}

	public void PlaySoundClip(string key, float volume) {
		if (soundMap.ContainsKey (key)) {
			source.PlayOneShot(soundMap[key], volume * masterVolume);

		}
	}

	public void PlaySoundClip(string key) {
		PlaySoundClip (key, 1.0f);
	}

	public void PlaySoundClip(string key, float volume, float offset) {
		if (soundMap.ContainsKey (key)) {
			GetComponent<AudioSource>().time = offset;
			GetComponent<AudioSource>().PlayOneShot(soundMap[key], volume);
		}
	}
}
