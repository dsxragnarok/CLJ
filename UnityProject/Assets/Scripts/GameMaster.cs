using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class GameMaster : MonoBehaviour {
	private PlayerStats playerData;
	private CharController player;
	private BoundsDeallocator gameBounds;
	private SoundEffectsManager sfxManager;
	private SpawnPlatforms platformSpawner;
	private SpawnBirds birdSpawner;
	private DifficultyProgress difficultyManager;
	private ScoreDisplay scoreDisplayManager;
	private InstanceManager instancingManager;
	private Canvas hudCanvas;
	private Canvas worldCanvas;
	private Camera mainCamera;
	public int collectedStars = 0;
	public int collectedBirds = 0;
	public int collectedCheckpoints = 0;
	public int checkpointsPassed = 0;
	public int score = 0;
	public int scoreMultiplier = 1;
	public bool isHighScore = false;

	public GameObject gameOverDialog;
	public GameObject instructionsDialog;
	public GameObject creditsDialog;
	public GameObject floatingTextPrefab;

	public bool isGameStarted = false;

	public PlayerStats PlayerData
	{
		get { return playerData; }
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

	public void showCredits () {
		creditsDialog.SetActive (true);
	}

	public void closeCredits () {
		creditsDialog.SetActive (false);
	}

	public void closeInstructions () {
		instructionsDialog.SetActive (false);
	}

	public void showGameOver () {
		gameOverDialog.SetActive (true);
		if (isHighScore) {
			playerData.ReportHighScore ();
			isHighScore = false;		// prevent dat spam
		}
	}

	public void startGame() {
		Application.LoadLevel ("Play");
		linkObjects (); // Doesn't matter, just call it to feel safe
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
