using UnityEngine;
using System.Collections;

// Contains information about the player and persists between plays.
// Reads and write these to hard disk as well so opening the game again
// will preserve these stats.
public class PlayerStats : MonoBehaviour {

	public int highScore = 0;
	public int totalStarsCollected = 0;
	public int totalRedBirdsCollected = 0;
	public int totalBlueBirdsCollected = 0;
	public int totalBlackBirdsCollected = 0;
	public int totalCheckpointsCollected = 0;

	void Awake () {
		GameObject.DontDestroyOnLoad (this.gameObject);
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
