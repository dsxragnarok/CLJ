using UnityEngine;
using System.Collections;

using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

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
#if UNITY_ANDROID
		PlayGamesPlatform.Activate ();
#endif
		// Authenticate and register a ProcessAuthentication callback
		// This call needs to be made before we can proceed to other calls in the Social API
		Social.localUser.Authenticate (ProcessAuthentication);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// This function gets called when Authenticate completes
	// Note that if the operation is successful, Social.localUser will contain data from the server. 
	public void ProcessAuthentication (bool success) {
		if (success) {
			Debug.Log ("Authenticated, checking achievements");
			// Request loaded achievements, and register a callback for processing them
			Social.LoadAchievements (ProcessLoadedAchievements);
		} else {
			Debug.Log ("Failed to authenticate");
		}
	}
	
	// This function gets called when the LoadAchievement call completes
	public void ProcessLoadedAchievements (IAchievement[] achievements) {
		if (achievements.Length == 0)
			Debug.Log ("Error: no achievements found");
		else
			Debug.Log ("Got " + achievements.Length + " achievements");
	}

	public void ReportHighScore ()
	{
#if UNITY_IOS
		// cloudislandscores is the leaderboard set up in iTunes Connect, might not exist on android version
		Social.ReportScore (highScore, "cloudislandscores", (bool result) => {
			if (result)
				Debug.Log ("Successfully reported achievement progress");
			else
				Debug.Log ("Failed to report achievement");
		});
#elif UNITY_ANDROID
		// CgkI68X_t_kNEAIQAQ is the leaderboard set up in Google Play Games Services, for android version
		Social.ReportScore (highScore, "CgkI68X_t_kNEAIQAQ", (bool success) => {
			if (success) {
				Debug.Log ("Successfully reported leaderboard score");
			} else {
				Debug.Log ("Failed to report leaderboard score");
			}
			//Social.ShowLeaderboardUI();
			//PlayGamesPlatform.Instance.ShowLeaderboardUI("CgkI68X_t_kNEAIQAQ");
		});
#endif
	}

	public void DisplayLeaderboard () {
		Social.ShowLeaderboardUI ();
	}
}
