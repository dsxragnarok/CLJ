using UnityEngine;
using System.Collections;

// Settings for the game such as volume
public class GameSettings : MonoBehaviour {

    private float masterVolume;
    private float lastVolume;
    private bool muted = false;

    public float MasterVolume 
    {
        get { return masterVolume; }
        set 
        {
            lastVolume = masterVolume;
            masterVolume = value; 
        }
    }

    public float LastVolume 
    { 
        get { return lastVolume; }
        set { lastVolume = value; }
    }

    public bool Muted { get { return muted; } }

    private SoundEffectsManager sfm;

	// Use this for initialization
	void Start () {
        GameObject.DontDestroyOnLoad(this.gameObject);

        // initialize volume
        lastVolume = 0f;
        masterVolume = 1.0f;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void AdjustMasterVolume (float n)
    {
        MasterVolume = n;
    }

    public bool ToggleMute (bool alterVolume)
    {
        muted = !muted;
        if (alterVolume)
        {
            if (muted)
                MasterVolume = 0;
            else
                MasterVolume = LastVolume;        
        }
        return muted;
    }
}
