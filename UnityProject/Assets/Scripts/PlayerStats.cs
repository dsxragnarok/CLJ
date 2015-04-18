﻿using UnityEngine;
using System.Collections;

using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

// Contains information about the player and persists between plays.
// Reads and write these to hard disk as well so opening the game again
// will preserve these stats.
public class PlayerStats : MonoBehaviour {

	// The variables below are the COLLECTIVE values across all games the player has played.
	public int highScore = 0;
	public int totalStarsCollected = 0;
	public int totalRedBirdsCollected = 0;
	public int totalBlueBirdsCollected = 0;
	public int totalBlackBirdsCollected = 0;
	public int totalCheckpointsCollected = 0;

	void Awake () {
		GameObject.DontDestroyOnLoad (this.gameObject);
		//PlayerPrefs.DeleteAll (); // Resets Player Stats, for debugging, you can uncomment it
	}

	// Use this for initialization
	void Start () {
		LoadStatistics ();

#if UNITY_ANDROID
		PlayGamesPlatform.Activate ();
#endif
		// Authenticate and register a ProcessAuthentication callback
		// This call needs to be made before we can proceed to other calls in the Social API
        AttemptAuthentication();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Save and Load statistics using Player Prefs
	public void SaveStatistics()
	{
		PlayerPrefs.SetInt ("highScore", highScore);
		PlayerPrefs.SetInt ("totalStarsCollected", totalStarsCollected);
		PlayerPrefs.SetInt ("totalRedBirdsCollected", totalRedBirdsCollected);
		PlayerPrefs.SetInt ("totalBlueBirdsCollected", totalBlueBirdsCollected);
		PlayerPrefs.SetInt ("totalBlackBirdsCollected", totalBlackBirdsCollected);
		PlayerPrefs.SetInt ("totalCheckpointsCollected", totalCheckpointsCollected);
	}
	public void LoadStatistics()
	{
		highScore = PlayerPrefs.GetInt ("highScore", 0);
		totalStarsCollected = PlayerPrefs.GetInt ("totalStarsCollected", 0);
		totalRedBirdsCollected = PlayerPrefs.GetInt ("totalRedBirdsCollected", 0);
		totalBlueBirdsCollected = PlayerPrefs.GetInt ("totalBlueBirdsCollected", 0);
		totalBlackBirdsCollected = PlayerPrefs.GetInt ("totalBlackBirdsCollected", 0);
		totalCheckpointsCollected = PlayerPrefs.GetInt ("totalCheckpointsCollected", 0);
	}

    public void AttemptAuthentication ()
    {
        Social.localUser.Authenticate(ProcessAuthentication);
    }

	// This function gets called when Authenticate completes
	// Note that if the operation is successful, Social.localUser will contain data from the server. 
	public void ProcessAuthentication (bool success) {
		if (success) {
			Debug.Log ("Authenticated, checking achievements");
			// Request loaded achievements, and register a callback for processing them
			Social.LoadAchievements (ProcessLoadedAchievements);

            // Deactivate the Login Button and activate the Leaderboard Button
            GameObject hud = GameObject.FindGameObjectWithTag("HUD");
            UnityEngine.UI.Button[] buttons = hud.GetComponentsInChildren<UnityEngine.UI.Button>(true);
            foreach (UnityEngine.UI.Button btn in buttons)
            {
                if (btn.tag == "LoginButton")
                {
                    btn.gameObject.SetActive(false);
                }
                if (btn.tag == "LeaderboardButton")
                {
                    btn.gameObject.SetActive(true);
                }
            }
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

	public void DisplayLeaderboard () 
    {
        //if (success)
        //{
            Social.ShowLeaderboardUI();
        //}
        //else
        //{
            // we should probably put up some error message
         //   Debug.Log("Authentication failed.");
        //}
	}
    /*
    public void AttemptDisplayLeaderboard ()
    {
        if (Social.localUser.authenticated)
        {
            Social.ShowLeaderboardUI();
        }
        else
        {
            Social.localUser.Authenticate(DisplayLeaderboard);
        }
    }*/
}
