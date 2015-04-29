using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GoogleMobileAds.Api;

// This important object is the central manager across all necessary objects in the scene.
// It holds all managers and important entities.
// Any game object with the Entity attribute can refer to any of these managers and objects
// using the Game Master
public class GameMaster : MonoBehaviour {
	public GoogleAnalyticsV3 googleAnalyticsPrefab;
    private GoogleAnalyticsV3 googleAnalytics;   // Google Analytics for stats tracking
	private PlayerStats playerData;				// Player data and statistics
    private GameSettings settings;
	private CharController player;				// Player Avatar in the scene
	private BoundsDeallocator gameBounds;		// Bounds of the game usually to check when an entity is outside
	private SoundEffectsManager sfxManager;			// Sound Effects Manager
	private SpawnPlatforms platformSpawner;			// Cloud Spawner Manager
	private SpawnBirds birdSpawner;					// Bird Spawning Manager
	private DifficultyProgress difficultyManager;	// Difficulty Manager
	private ScoreDisplay scoreDisplayManager;		// Score Display in Game
	private InstanceManager instancingManager;		// Instancing Manager of Common Entity Objects
	private Canvas hudCanvas;						
	private Canvas worldCanvas;
	private Camera mainCamera;
    private AdHandler adManager;
	public int collectedStars = 0;				// Current Game collected stars
	public int collectedBalloons = 0;				// Current Game collected balloons
    public int collectedBlueBirds = 0;          // Current Game collected blue birds
	public int collectedCheckpoints = 0;		// Current Game collected checkpoints
	public int checkpointsPassed = 0;			
	public int score = 0;						// Current Score
	public int scoreMultiplier = 1;				// Score Multiplier Bonus
	public bool isSaved = false;
	public bool isHighScore = false;
	public bool isPaused = false;

    public GameObject resultDialog;
	public GameObject gameOverDialog;
	public GameObject instructionsDialog;
	public GameObject creditsDialog;
	public GameObject floatingTextPrefab;
    public GameObject volumeControl;
    public GameObject tgGameOverButton; // for testing purposes - this allows to hide the Game Over panel to take screenshot on death
    public GameObject pauseButton;
    public GameObject soundButton;

    public Sprite pauseImage;
    public Sprite playImage;
    public Sprite muteImage;
    public Sprite soundImage;

	public bool isGameStarted = false;
    public float startTime;             // The time in seconds after the player exits instruction screen
    public float endTime;               // The time in seconds when the player dies

    //private string versionString = "v 0.9";
    private string weburl = "http://mkcpgames.dsxragnarok.com/";

    private int tipIndex = 0;
    private string[] gameplayTips = new string[]
    {
        "You can vary the height of your\njump by how long you touch to jump.",
        "You can double jump by tapping twice.",
        "Each BALLOON permanently increases the\n worth of each STAR by 10.",
        "Each RAINBOW is worth 50 more\n than the previous RAINBOW."
    };

	public PlayerStats PlayerData
	{
		get { return playerData; }
	}

    public GameSettings Settings
    {
        get { return settings; }
    }

	public CharController Player
	{
		get { return player; }
	}
	
	public BoundsDeallocator GameBounds
	{
		get { return gameBounds; }
	}

	public SoundEffectsManager SoundEffects
	{
		get { return sfxManager; }
	}

	public SpawnPlatforms PlatformSpawner
	{
		get { return platformSpawner; }
	}
	
	public SpawnBirds BirdSpawner
	{
		get { return birdSpawner; }
	}

	public DifficultyProgress DifficultyManager
	{
		get { return difficultyManager; }
	}

	public ScoreDisplay ScoreDisplayManager
	{
		get { return scoreDisplayManager; }
	}
	
	public InstanceManager InstancingManager
	{
		get { return instancingManager; }
	}

	public Camera MainCamera
	{
		get { return mainCamera; }
	}
	
	public Canvas HudCanvas
	{
		get { return hudCanvas; }
	}
	
	public Canvas WorldCanvas
	{
		get { return worldCanvas; }
	}

    public AdHandler AdManager
    {
        get { return adManager; }
    }

	// Use this for initialization
	void Start () {
		// iOS framerate is vsynced and will always be 15, 30, or 60 FPS. Without setting
		// the targetFrameRate, it'll be at 30. Setting this to 50 will make iOS FPS 60.
		// TODO: Might want to adjust forces and such to have it all be at 60 FPS later.
		Application.targetFrameRate = 50;

		linkObjects ();

        AudioSource musicSource = mainCamera.GetComponent<AudioSource>();
        if (musicSource != null)
        {
            musicSource.volume = Settings.MasterVolume;
        }

        // initialize tipIndex to a random position
        tipIndex = Random.Range(0, gameplayTips.Length - 1);

        GameObject volumeControl = GameObject.FindGameObjectWithTag("VolumeControl");
        if (volumeControl != null)
        {
            Slider volumeSlider = volumeControl.GetComponentInChildren<Slider>();
            volumeSlider.value = settings.MasterVolume;
        }

        // Deactivate the Login Button and activate the Leaderboard Button
        GameObject hud = GameObject.FindGameObjectWithTag("HUD");
        Button[] buttons = hud.GetComponentsInChildren<Button>(true);
        foreach (Button btn in buttons)
        {
            if (playerData.isAuthenticated() && btn.tag == "LoginButton")
            {
                btn.gameObject.SetActive(false);
            }
            if (playerData.isAuthenticated() && btn.tag == "LeaderboardButton")
            {
                btn.gameObject.SetActive(true);
            }
            if (btn.name == "VolumeButton")
            {
                Image btnImage = btn.GetComponent<Image>();
                if (Settings.Muted)
                {
                    btnImage.sprite = muteImage;
                }
                else
                {
                    btnImage.sprite = soundImage;
                }
            }


#if UNITY_IOS || UNITY_EDITOR
            // These platforms don't use a quit button
            if (btn.tag == "QuitButton")
            {
                btn.gameObject.SetActive(false);
            }
#endif
        }

        GameObject versionDisplay = GameObject.FindGameObjectWithTag("VersionString");
        if (versionDisplay != null)
        {
            versionDisplay.GetComponent<Text>().text = Application.version;//versionString;
        }
#if UNITY_ANDROID || UNITY_IOS
        googleAnalytics.DispatchHits();
        if (Application.loadedLevelName == "Main")
        {
            googleAnalytics.LogScreen("Main Menu Screen");
            //adManager.RequestBanner(AdPosition.BottomRight);  // no ads on Main Menu ~ too much clutter
        }
        if (Application.loadedLevelName == "Play")
        {
            googleAnalytics.LogScreen("Play Screen");
        }
#endif

    }
	
	// Update is called once per frame
	void Update () {
#if UNITY_WEBPLAYER || UNITY_STANDALONE
		if (Input.GetKeyDown (KeyCode.P)) {
			Debug.Log ("Screenshot Taken");
			Application.CaptureScreenshot ("Screenshot.png");
		}
#endif
        if (Input.GetKeyDown (KeyCode.Escape))
        {
            if (creditsDialog != null && creditsDialog.activeSelf)
            {
                closeCredits();
            }
            else if ((Application.loadedLevelName == "Main" && !creditsDialog.activeSelf) ||
                (Application.loadedLevelName == "Play" && (resultDialog.activeSelf || gameOverDialog.activeSelf)))
            {
                Application.Quit();
            }
        }
	}

	public void updateScore (int value) {
		score += value;

		scoreDisplayManager.TextDisplay.text = score.ToString();
		scoreDisplayManager.TriggerScale();
		if (score > playerData.highScore) {
			playerData.highScore = score;
			scoreDisplayManager.activeGlow = true;
			scoreDisplayManager.TextDisplay.color = Color.yellow;
			isHighScore = true;
		}
	}

	public void generateFloatingTextAt(Vector3 pos, string value)
	{
		GameObject ftInstance = (GameObject)GameObject.Instantiate(floatingTextPrefab);
		ftInstance.transform.SetParent(hudCanvas.transform);
		//Vector3 worldOffset = new Vector3(0.5f, 0.5f, 0f);
		//ftInstance.transform.position = mainCamera.WorldToScreenPoint(pos + worldOffset);
		ftInstance.GetComponent<FloatingText>().BindToTarget(Player.gameObject);
		ftInstance.GetComponent<Text>().text = value;
	}

    public void generateFloatingTextAt(Vector3 pos, string value, Color beginColor, Color endColor, GameObject bindTarget = null)
    {
        GameObject ftInstance = (GameObject)GameObject.Instantiate(floatingTextPrefab);
        FloatingText ft = ftInstance.GetComponent<FloatingText>();
        ft.startColor = beginColor;
        ft.endColor = endColor;
        if (bindTarget != null)
            ft.BindToTarget(bindTarget);
        ft.GetComponent<Text>().text = value;
    }

    public void ToggleMute()
    {
#if UNITY_ANDROID || UNITY_IOS
        googleAnalytics.LogEvent("Button Click", "Mute Toggle", "Player toggles volume via speaker icon", 1);
#endif
        GameObject volumeControl = GameObject.FindGameObjectWithTag("VolumeControl");
        bool muted;

        if (volumeControl == null)
        {
            muted = Settings.ToggleMute(true);
        }
        else
        {
            muted = Settings.ToggleMute(false);
            
            if (muted)
            {
                volumeControl.GetComponentInChildren<Slider>().value = 0f;
            }
            else
                volumeControl.GetComponentInChildren<Slider>().value = Settings.LastVolume;
        }

        GameObject hud = GameObject.FindGameObjectWithTag("HUD");
        Button[] buttons = hud.GetComponentsInChildren<Button>();

        foreach (Button btn in buttons)
        {
            if (btn.name == "VolumeButton")
            {
                Image sndImage = btn.GetComponent<Image>();
                if (muted)
                {
                    sndImage.sprite = muteImage;
                }
                else
                {
                    sndImage.sprite = soundImage;
                }
                break;
            }
        }

        if (SoundEffects != null)
        {
            SoundEffects.MasterVolume = Settings.MasterVolume;
        }
        AudioSource musicSource = mainCamera.GetComponent<AudioSource>();
        if (musicSource != null)
        {
            musicSource.volume = Settings.MasterVolume;
        }
    }
    public void adjustVolume (float v)
    {
        //Slider volume = volumeControl.GetComponent<Slider>();
        //Debug.Log("Volume value: " + volume.value);
        Settings.AdjustMasterVolume(v);
        Button[] buttons = volumeControl.GetComponentsInChildren<Button>();
        foreach (Button btn in buttons)
        {
            if (btn.name == "VolumeButton")
            {
                Image btnImage = btn.GetComponent<Image>();
                if (v > 0)
                {
                    btnImage.sprite = soundImage;
                }
                else
                {
                    btnImage.sprite = muteImage;
                }
            }
        }
    }

	public void showLeaderboard ()
    {
#if UNITY_ANDROID || UNITY_IOS
        googleAnalytics.LogScreen("Leaderboard Screen");
        googleAnalytics.LogEvent("Button Click", "Leaderboard", "Player opens leaderboard", 1);
#endif
        playerData.DisplayLeaderboard();//playerData.AttemptDisplayLeaderboard ();
	}

	public void showCredits ()
    {
#if UNITY_ANDROID || UNITY_IOS
        googleAnalytics.LogScreen("Credits Screen");
        googleAnalytics.LogEvent("Button Click", "Credits", "Player opens the credits", 1);
#endif
        creditsDialog.SetActive (true);
	}

	public void closeCredits ()
    {
#if UNITY_ANDROID || UNITY_IOS
        googleAnalytics.LogEvent("Button Click", "Close Credits", "Player closes the credits", 1);
#endif
        creditsDialog.SetActive (false);
	}

	public void closeInstructions ()
    {
#if UNITY_ANDROID || UNITY_IOS
        googleAnalytics.LogEvent("Game Start", "Initiate Game", "Player tapped to close instructions and start game", 1);
#endif
        instructionsDialog.SetActive (false);
        // start game music
        mainCamera.GetComponent<AudioSource>().Play();
	}

    public void showToggleGameOver()
    {
        tgGameOverButton.SetActive(true);
    }

    public void hideToggleGameOver()
    {
        tgGameOverButton.SetActive(false);
    }

    public void hideGameOver()
    {
        gameOverDialog.SetActive(false);
    }

    public void toggleGameOverScreen()
    {
        gameOverDialog.SetActive(!gameOverDialog.activeSelf);
    }

	public void showGameOver ()
    {
#if UNITY_ANDROID || UNITY_IOS
        //adManager.ShowInterstitial();
        googleAnalytics.LogScreen("Game Over");
#endif
        resultDialog.SetActive(false);
        gameOverDialog.SetActive(true);
        // Set the Comment Text object to be active if the user earned a high score
        // Otherwise, set the Tip Text object to be active and display a random gameplay tip
        Text[] texts = gameOverDialog.GetComponentsInChildren<Text>();
        foreach (Text text in texts)
        {
            if (text.name == "CommentText")
                text.gameObject.SetActive(isHighScore);

            if (text.name == "TipText" && !isHighScore)
            {
                int idx = (tipIndex + 1) % gameplayTips.Length;
                text.text = gameplayTips[idx];
                text.gameObject.SetActive(!isHighScore);
            }
        }
        /*
        if (!isSaved)
		{
			gameOverDialog.SetActive (true);
			// Set the Comment Text object to be active if the user earned a high score
            // Otherwise, set the Tip Text object to be active and display a random gameplay tip
			Text[] texts = gameOverDialog.GetComponentsInChildren<Text> ();
			foreach (Text text in texts) {
				if (text.name == "CommentText")
					text.gameObject.SetActive (isHighScore);
                
                if (text.name == "TipText" && !isHighScore) {
                    int idx = (tipIndex + 1) % gameplayTips.Length;
                    text.text = gameplayTips[idx];
                    text.gameObject.SetActive (!isHighScore);
                }
			}
			// Save player data and if there is a high score, report the high score to
			// a social platform.
			playerData.SaveStatistics ();
			if (isHighScore) {
				playerData.ReportHighScore ();
                googleAnalytics.LogEvent("HighScore", "Surpassed", "Beat own high score", playerData.highScore);
			}

            // Collect some stats for Google Analytics
            googleAnalytics.LogEvent("StarsCollected", "StarsThisSession", "Stars", this.collectedStars);
            googleAnalytics.LogEvent("BalloonsCollected", "BalloonsThisSession", "Balloons", this.collectedBalloons);
            googleAnalytics.LogEvent("RainbowsCollected", "RainbowsThisSession", "Rainbows", this.collectedCheckpoints);
            googleAnalytics.LogEvent("CheckpointsPassed", "CheckpointsPassedThisSession", "Checkpoints", this.checkpointsPassed);
            googleAnalytics.LogEvent("BlueBirdsCollected", "BlueBirdsThisSession", "Bluebirds", this.collectedBlueBirds);
            googleAnalytics.LogEvent("SessionDuration", "ThisSessionDuration", "Duration", (long)((endTime - startTime) * 1000L));
#if UNITY_ANDROID
            // These are specific to Google Play leaderboards.
            playerData.ReportLeaderboard(this.collectedStars, "CgkI68X_t_kNEAIQCA");
            playerData.ReportLeaderboard(this.collectedBalloons, "CgkI68X_t_kNEAIQCQ");
            playerData.ReportLeaderboard(this.collectedCheckpoints, "CgkI68X_t_kNEAIQCg");
            playerData.ReportLeaderboard(this.collectedBlueBirds, "CgkI68X_t_kNEAIQDw");

            playerData.ReportLeaderboard(playerData.totalDeath, "CgkI68X_t_kNEAIQCw");
            playerData.ReportLeaderboard(playerData.totalStarsCollected, "CgkI68X_t_kNEAIQDA");
            playerData.ReportLeaderboard(playerData.totalRedBirdsCollected, "CgkI68X_t_kNEAIQDQ");
            playerData.ReportLeaderboard(playerData.totalCheckpointsCollected, "CgkI68X_t_kNEAIQDg");
            playerData.ReportLeaderboard(playerData.totalBlueBirdsCollected, "CgkI68X_t_kNEAIQEQ");
            playerData.ReportLeaderboard(playerData.totalBlackBirdsCollected, "CgkI68X_t_kNEAIQEA");
            playerData.ReportLeaderboard((long)((endTime - startTime) * 1000L), "CgkI68X_t_kNEAIQEg");  // Google Play accepts time in milliseconds
#elif UNITY_IOS
            playerData.ReportLeaderboard(this.collectedStars, "starspersession");
			playerData.ReportLeaderboard(this.collectedBalloons, "balloonspersession");
            playerData.ReportLeaderboard(this.collectedCheckpoints, "rainbowspersession");
            playerData.ReportLeaderboard(this.collectedBlueBirds, "bluebirdspersession");

            playerData.ReportLeaderboard(playerData.totalDeath, "mostdeaths");
            playerData.ReportLeaderboard(playerData.totalStarsCollected, "moststarsalltime");
            playerData.ReportLeaderboard(playerData.totalRedBirdsCollected, "mostballoonsalltime");
            playerData.ReportLeaderboard(playerData.totalCheckpointsCollected, "mostrainbowsalltime");
            playerData.ReportLeaderboard(playerData.totalBlueBirdsCollected, "bluebirdsalltime");
            playerData.ReportLeaderboard(playerData.totalBlackBirdsCollected, "yellowbirdsalltime");
            playerData.ReportLeaderboard((long)(endTime - startTime), "longestsession");           // Game Center set to the second
#endif

            isSaved = true; // Prevent Dat Spam
		}*/
	}

    public void showResult()
    {
        if (!isSaved)
        {
            // Set the Comment Text object to be active if the user earned a high score
            // Otherwise, set the Tip Text object to be active and display a random gameplay tip
            Text[] texts = resultDialog.GetComponentsInChildren<Text>(true);
            foreach (Text text in texts)
            {
                if (text.name == "ScoreField")
                {
                    text.text = score.ToString();
                }
                if (text.name == "StarsField")
                {
                    text.text = collectedStars.ToString();
                }

                if (text.name == "BalloonsField")
                {
                    text.text = collectedBalloons.ToString();
                }

                if (text.name == "RainbowsField")
                {
                    text.text = collectedCheckpoints.ToString();
                }

                if (text.name == "BluebirdsField")
                {
                    text.text = collectedBlueBirds.ToString();
                }

                if (text.name == "DurationField")
                {
                    long duration_seconds = (long)(endTime - startTime);
                    long minutes = duration_seconds / 60;
                    long seconds = duration_seconds % 60;
                    text.text = string.Format("{0:00}:{1:00}", minutes, seconds);
                }                    
            }
            // Save player data and if there is a high score, report the high score to
            // a social platform.
            playerData.SaveStatistics();
            if (isHighScore)
            {
                playerData.ReportHighScore();
#if UNITY_ANDROID || UNITY_IOS
                googleAnalytics.LogEvent("High Score", "Personal High Score", "Beat own high score", playerData.highScore);
#endif
            }
#if UNITY_ANDROID || UNITY_IOS
            // android admob call -- NOTE: Interstitial ads are too annoying, only use banners - KP
            // we figure out if we should request a banner ad or interstitial based on how many times the player has been dead
            /*if (playerData.totalDeath % AdHandler.INTERSTITIAL_FREQUENCY == 0)
            {
                adManager.RequestInterstitual();
            }
            else
            {
                adManager.InterstitialCountDown += 1;
                adManager.ShowBanner();
                //adManager.RequestBanner(AdPosition.Top);
            }*/
            adManager.ShowBanner();

            googleAnalytics.LogScreen("Results Screen");
            // Collect some stats for Google Analytics
            googleAnalytics.LogEvent("Stars Collected", "Stars Per Session", "Stars collected in one session", this.collectedStars);
            googleAnalytics.LogEvent("Balloons Collected", "Balloons Per Session", "Balloons collected in one session", this.collectedBalloons);
            googleAnalytics.LogEvent("Rainbows Collected", "Rainbows Per Session", "Rainbows collected in one session", this.collectedCheckpoints);
            googleAnalytics.LogEvent("Checkpoints Passed", "Checkpoints Passed Per Session", "Checkpoints passed in one session", this.checkpointsPassed);
            googleAnalytics.LogEvent("Blue Birds Collected", "Blue Birds Per Session", "Blue Birds collided in one session", this.collectedBlueBirds);
            googleAnalytics.LogEvent("Duration", "Session Duration", "Duration of one game run", (long)((endTime - startTime) * 1000L));
#endif
#if UNITY_ANDROID
            // These are specific to Google Play leaderboards.
            playerData.ReportLeaderboard(this.collectedStars, "CgkI68X_t_kNEAIQCA");
            playerData.ReportLeaderboard(this.collectedBalloons, "CgkI68X_t_kNEAIQCQ");
            playerData.ReportLeaderboard(this.collectedCheckpoints, "CgkI68X_t_kNEAIQCg");
            playerData.ReportLeaderboard(this.collectedBlueBirds, "CgkI68X_t_kNEAIQDw");

            playerData.ReportLeaderboard(playerData.totalDeath, "CgkI68X_t_kNEAIQCw");
            playerData.ReportLeaderboard(playerData.totalStarsCollected, "CgkI68X_t_kNEAIQDA");
            playerData.ReportLeaderboard(playerData.totalRedBirdsCollected, "CgkI68X_t_kNEAIQDQ");
            playerData.ReportLeaderboard(playerData.totalCheckpointsCollected, "CgkI68X_t_kNEAIQDg");
            playerData.ReportLeaderboard(playerData.totalBlueBirdsCollected, "CgkI68X_t_kNEAIQEQ");
            playerData.ReportLeaderboard(playerData.totalBlackBirdsCollected, "CgkI68X_t_kNEAIQEA");
            playerData.ReportLeaderboard((long)((endTime - startTime) * 1000L), "CgkI68X_t_kNEAIQEg");  // Google Play accepts time in milliseconds
#elif UNITY_IOS
            playerData.ReportLeaderboard(this.collectedStars, "starspersession");
			playerData.ReportLeaderboard(this.collectedBalloons, "balloonspersession");
            playerData.ReportLeaderboard(this.collectedCheckpoints, "rainbowspersession");
            playerData.ReportLeaderboard(this.collectedBlueBirds, "bluebirdspersession");

            playerData.ReportLeaderboard(playerData.totalDeath, "mostdeaths");
            playerData.ReportLeaderboard(playerData.totalStarsCollected, "moststarsalltime");
            playerData.ReportLeaderboard(playerData.totalRedBirdsCollected, "mostballoonsalltime");
            playerData.ReportLeaderboard(playerData.totalCheckpointsCollected, "mostrainbowsalltime");
            playerData.ReportLeaderboard(playerData.totalBlueBirdsCollected, "bluebirdsalltime");
            playerData.ReportLeaderboard(playerData.totalBlackBirdsCollected, "yellowbirdsalltime");
            playerData.ReportLeaderboard((long)(endTime - startTime), "longestsession");           // Game Center set to the second
#endif

            isSaved = true; // Prevent Dat Spam
            resultDialog.SetActive(true);
        }
    }

	public void togglePause () {
        // Don't allow pause during game over
        if (resultDialog.activeSelf || gameOverDialog.activeSelf)
            return;

		isPaused = !isPaused;
        Image pbImage = pauseButton.GetComponent<Image>();
        AudioSource musicSource = mainCamera.GetComponent<AudioSource>();
		if (isPaused) {
#if UNITY_ANDROID || UNITY_IOS
            adManager.ShowBanner();
            //adManager.RequestBanner(AdPosition.Top);
            googleAnalytics.LogEvent("Button Click", "Pause Button", "Player paused the game", 1);
#endif

            pbImage.sprite = playImage;
            Time.timeScale = 0f;
            musicSource.Pause();
		} 
        else
        {
#if UNITY_ANDROID || UNITY_IOS
            adManager.HideBanner();
            //adManager.DestroyBanner();
            googleAnalytics.LogEvent("Button Click", "Unpause Button", "Player unpaused the game", 1);
#endif
            pbImage.sprite = pauseImage;
            Time.timeScale = 1.0f;
            musicSource.UnPause();
		}
	}

	public void startGame()
    {
#if UNITY_ANDROID || UNITY_IOS
        adManager.HideBanner();
        //adManager.DestroyBanner();
        googleAnalytics.LogEvent("Button Click", "Play Button", "Player clicked play button", 1);
#endif
        Application.LoadLevel ("Play");
		linkObjects (); // Doesn't matter, just call it to feel safe
	}

    public void mainMenu()
    {
#if UNITY_ANDROID || UNITY_IOS
        adManager.HideBanner();
        //adManager.DestroyBanner();
        googleAnalytics.LogEvent("Button Click", "Main Menu Button", "Player clicked the Main Menu button", 1);
#endif
        Application.LoadLevel ("Main");
    }

	public void restartGame ()
    {
#if UNITY_ANDROID || UNITY_IOS
        adManager.HideBanner();
        //adManager.DestroyBanner();
        googleAnalytics.LogEvent("Button Click", "Retry Button", "Player clicked the Retry button", 1);
#endif
        // For restarts, grab all Entities in the scene and put them in the instancing manager for re-use
		// The containers below are all existing recyclable entities.
		GameObject _platformContainer = GameObject.FindGameObjectWithTag("PlatformContainer");
		if (_platformContainer != null) {
			Entity[] entities = _platformContainer.GetComponentsInChildren<Entity>();
			foreach (Entity ent in entities)
				instancingManager.RecycleObject(ent);
		}
		GameObject _birdContainer = GameObject.FindGameObjectWithTag("BirdContainer");
		if (_birdContainer != null) {
			Entity[] entities = _birdContainer.GetComponentsInChildren<Entity>();
			foreach (Entity ent in entities)
				instancingManager.RecycleObject(ent);
		}

		Application.LoadLevel (Application.loadedLevel);
		linkObjects (); // Doesn't matter, just call it to feel safe
	}

	// Note To Self - this is ignored in the editor and webplayer
	public void quitGame ()
    {
#if UNITY_ANDROID || UNITY_IOS
        googleAnalytics.LogEvent("Button Click", "Quit Button", "Player clicked the Quit button", 1);
#endif
        Application.Quit ();
	}

    public void visitWebsite ()
    {
#if UNITY_ANDROID || UNITY_IOS
        googleAnalytics.LogEvent("Button Click", "Visit Site", "Player clicked on Visit Site button", 1);
#endif
        Application.OpenURL(weburl);
    }

    public bool isHitPauseButton (Vector3 position)
    {
        Vector3 pb = pauseButton.transform.position;

        float halfw = pauseButton.GetComponent<Image>().rectTransform.rect.width / 2;
        float halfh = pauseButton.GetComponent<Image>().rectTransform.rect.height / 2;

        float left = pb.x - halfw;
        float right = pb.x + halfw;
        float top = pb.y - halfh;
        float bottom = pb.y + halfh;

        if (position.x >= left && position.x <= right && position.y >= top && position.y <= bottom)
            return true;

        return false;
    }

    public bool isHitSoundButton(Vector3 position)
    {
        Vector3 pb = soundButton.transform.position;

        float halfw = soundButton.GetComponent<Image>().rectTransform.rect.width / 2;
        float halfh = soundButton.GetComponent<Image>().rectTransform.rect.height / 2;

        float left = pb.x - halfw;
        float right = pb.x + halfw;
        float top = pb.y - halfh;
        float bottom = pb.y + halfh;

        if (position.x >= left && position.x <= right && position.y >= top && position.y <= bottom)
            return true;

        return false;
    }

    public void SocialLogin ()
    {
#if UNITY_ANDROID || UNITY_IOS
        googleAnalytics.LogEvent("Button Click", "Login Button", "Player clicked the Login button", 1);
#endif
        PlayerData.AttemptAuthentication();
    }

	public void linkObjects() {
		GameObject _GAv3 = GameObject.FindGameObjectWithTag("GAv3");
		if (_GAv3 != null) {
			googleAnalytics = _GAv3.GetComponent<GoogleAnalyticsV3> ();
		} else {
			// Create an instance from a provided prefab
			googleAnalytics = GameObject.Instantiate(googleAnalyticsPrefab);
		}

        GameObject _adManager = GameObject.FindGameObjectWithTag("AdManager");
        if (_adManager != null)
        {
            adManager = _adManager.GetComponent<AdHandler>();
        }
        else
        {
            GameObject instance = new GameObject();
            instance.name = "AdManager";
            instance.tag = "AdManager";
            adManager = instance.AddComponent<AdHandler>();
        }

		GameObject _playerData = GameObject.FindGameObjectWithTag("PlayerData");
		if (_playerData != null) {
			playerData = _playerData.GetComponent<PlayerStats> ();
		} else {
			// Create a Player Data instance if it does not exist yet
			GameObject instance = new GameObject();
			instance.name = "PlayerData";
			instance.tag = "PlayerData";
			playerData = instance.AddComponent<PlayerStats>();
		}

        GameObject _gameSettings = GameObject.FindGameObjectWithTag("GameSettings");
        if (_gameSettings != null)
        {
            settings = _gameSettings.GetComponent<GameSettings>();
        }
        else
        {
            // Create a Game Settings instance if it does not yet exist
            GameObject instance = new GameObject();
            instance.name = "GameSettings";
            instance.tag = "GameSettings";
            settings = instance.AddComponent<GameSettings>();
        }

		GameObject _player = GameObject.FindGameObjectWithTag("Player");
		if (_player != null)
			player = _player.GetComponent<CharController>();
		
		GameObject _gameBounds = GameObject.FindGameObjectWithTag("Bounds");
		if (_gameBounds != null)
			gameBounds = _gameBounds.GetComponent<BoundsDeallocator>();
		
		GameObject _platformSpawner = GameObject.FindGameObjectWithTag ("CloudSceneSpawner");
		if (_platformSpawner != null)
			platformSpawner = _platformSpawner.GetComponent<SpawnPlatforms> ();
		
		GameObject _birdSpawner = GameObject.FindGameObjectWithTag ("BirdSpawner");
		if (_birdSpawner != null)
			birdSpawner = _birdSpawner.GetComponent<SpawnBirds> ();
		
		GameObject _difficultyManager = GameObject.FindGameObjectWithTag ("DifficultyManager");
		if (_difficultyManager != null)
			difficultyManager = _difficultyManager.GetComponent<DifficultyProgress> ();
		
		GameObject _instancingManager = GameObject.FindGameObjectWithTag ("InstancingManager");
		if (_instancingManager != null)
		{
			instancingManager = _instancingManager.GetComponent<InstanceManager> ();
			instancingManager.UpdateCachedObjectLinks();
		}
		else {
			GameObject instance = new GameObject();
			instance.name = "InstancingManager";
			instance.tag = "InstancingManager";
			instancingManager = instance.AddComponent<InstanceManager>();
		}

		GameObject _scoreDisplayManager = GameObject.FindGameObjectWithTag ("Score");
		if (_scoreDisplayManager != null)
			scoreDisplayManager = _scoreDisplayManager.GetComponent<ScoreDisplay>();
		
		GameObject _sfxManager = GameObject.FindGameObjectWithTag ("SoundEffect");
		if (_sfxManager != null)
			sfxManager = _sfxManager.GetComponent<SoundEffectsManager> ();
		
		GameObject _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
		if (_mainCamera != null)
			mainCamera = _mainCamera.GetComponent<Camera>();
		
		GameObject _hudCanvas = GameObject.FindGameObjectWithTag("HUD");
		if (_hudCanvas != null)
			hudCanvas = _hudCanvas.GetComponent<Canvas>();
		
		GameObject _worldCanvas = GameObject.FindGameObjectWithTag("WorldCanvas");
		if (_worldCanvas != null)
			worldCanvas = _worldCanvas.GetComponent<Canvas>();
	}
}
