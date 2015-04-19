using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// This important object is the central manager across all necessary objects in the scene.
// It holds all managers and important entities.
// Any game object with the Entity attribute can refer to any of these managers and objects
// using the Game Master
public class GameMaster : MonoBehaviour {
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
	public int collectedStars = 0;				// Current Game collected stars
	public int collectedBirds = 0;				// Current Game collected birds
	public int collectedCheckpoints = 0;		// Current Game collected checkpoints
	public int checkpointsPassed = 0;			
	public int score = 0;						// Current Score
	public int scoreMultiplier = 1;				// Score Multiplier Bonus
	public bool isSaved = false;
	public bool isHighScore = false;
	public bool isPaused = false;

	public GameObject gameOverDialog;
	public GameObject instructionsDialog;
	public GameObject creditsDialog;
	public GameObject floatingTextPrefab;
    public GameObject volumeControl;
    public GameObject tgGameOverButton; // for testing purposes - this allows to hide the Game Over panel to take screenshot on death
    public GameObject pauseButton;

    public Sprite pauseImage;
    public Sprite playImage;

	public bool isGameStarted = false;

    private string versionString = "v 0.7";

    private int tipIndex = 0;
    private string[] gameplayTips = new string[]
    {
        "You can vary the height of your\njump by how long you touch to jump.",
        "You can double jump by tapping twice.",
        "Each RED BIRD permanently increases the\n worth of each STAR by 10.",
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

        if (playerData.isAuthenticated())
        {
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
        }

#if UNITY_IOS || UNITY_EDITOR
        // In ios, we don't use the Quit button
        GameObject quitButton = GameObject.FindGameObjectWithTag("QuitButton");
        quitButton.SetActive(false);
#endif

        GameObject versionDisplay = GameObject.FindGameObjectWithTag("VersionString");
        if (versionDisplay != null)
        {
            versionDisplay.GetComponent<Text>().text = versionString;
        }
	}
	
	// Update is called once per frame
	void Update () {
#if UNITY_WEBPLAYER || UNITY_STANDALONE
		if (Input.GetKeyDown (KeyCode.P)) {
			Debug.Log ("Screenshot Taken");
			Application.CaptureScreenshot ("Screenshot.png");
		}
#endif
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

    public void adjustVolume (float v)
    {
        //Slider volume = volumeControl.GetComponent<Slider>();
        //Debug.Log("Volume value: " + volume.value);
        Settings.AdjustMasterVolume(v);
    }

	public void showLeaderboard () {
        playerData.DisplayLeaderboard();//playerData.AttemptDisplayLeaderboard ();
	}

	public void showCredits () {
		creditsDialog.SetActive (true);
	}

	public void closeCredits () {
		creditsDialog.SetActive (false);
	}

	public void closeInstructions () {
		instructionsDialog.SetActive (false);
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

	public void showGameOver () {

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
			}
			isSaved = true; // Prevent Dat Spam
		}
	}

	public void togglePause () {
		isPaused = !isPaused;
        Image pbImage = pauseButton.GetComponent<Image>();
        AudioSource musicSource = mainCamera.GetComponent<AudioSource>();
		if (isPaused) {
            pbImage.sprite = playImage;
            Time.timeScale = 0f;
            musicSource.Pause();
            //musicSource.volume = 0;
		} else {
            pbImage.sprite = pauseImage;
            Time.timeScale = 1.0f;
            musicSource.UnPause();
            //musicSource.volume = Settings.MasterVolume;
		}
	}

	public void startGame() {
		Application.LoadLevel ("Play");
		linkObjects (); // Doesn't matter, just call it to feel safe
	}

    public void mainMenu()
    {
        Application.LoadLevel ("Main");
    }

	public void restartGame () {
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
	public void quitGame () {
		Application.Quit ();
	}

    public bool isHitPauseButton (Vector3 position)
    {
        Vector3 pb = pauseButton.transform.position;

        float halfw = pauseButton.GetComponent<UnityEngine.UI.Image>().rectTransform.rect.width / 2;
        float halfh = pauseButton.GetComponent<UnityEngine.UI.Image>().rectTransform.rect.height / 2;

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
        PlayerData.AttemptAuthentication();
    }

	public void linkObjects() {
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
