using UnityEngine;
using System.Collections;

// Settings for the game such as volume
public class GameSettings : MonoBehaviour {

    private float masterVolume;

    public float MasterVolume 
    {
        get { return masterVolume; }
        set { masterVolume = value; }
    }

    private SoundEffectsManager sfm;

	// Use this for initialization
	void Start () {
        GameObject.DontDestroyOnLoad(this.gameObject);

        // initialize volume
        MasterVolume = 1.0f;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void AdjustMasterVolume (float n)
    {
        MasterVolume = n;
    }
}
