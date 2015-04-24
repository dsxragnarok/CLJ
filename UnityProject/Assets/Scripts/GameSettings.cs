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

    void Awake()
    {
        // initialize volume
        lastVolume = masterVolume = 1.0f;
    }

	// Use this for initialization
	void Start () {
        GameObject.DontDestroyOnLoad(this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void AdjustMasterVolume (float n)
    {
        MasterVolume = n;
        if (n == 0)
            muted = true;
        else
            muted = false;
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
