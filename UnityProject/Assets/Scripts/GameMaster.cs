using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class GameMaster : MonoBehaviour {
	private CharController player;
	private BoundsDeallocator gameBounds;
	private SoundEffectsManager sfxManager;
	private SpawnPlatforms platformSpawner;
	private SpawnBirds birdSpawner;
	private DifficultyProgress difficultyManager;
	private Canvas hudCanvas;
	private Canvas worldCanvas;
	private Camera mainCamera;
	public int collectedStars = 0;
	public int collectedBirds = 0;
	public int score = 0;
	public int scoreMultiplier = 10;

	public GameObject gameOverDialog;
	public GameObject instructionsDialog;
	public GameObject floatingTextPrefab;

	public bool isGameStarted = false;

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
		GameObject _player = GameObject.FindGameObjectWithTag("Player");
		if (_player != null)
			player = _player.GetComponent<CharController>();

		GameObject _gameBounds = GameObject.FindGameObjectWithTag("Bounds");
		if (_gameBounds != null)
			gameBounds = _gameBounds.GetComponent<BoundsDeallocator>();

		GameObject _spawner = GameObject.FindGameObjectWithTag ("Spawner");
		if (_spawner != null)
		{
			platformSpawner = _spawner.GetComponent<SpawnPlatforms> ();
			birdSpawner = _spawner.GetComponent<SpawnBirds>(); 
			difficultyManager = _spawner.GetComponent<DifficultyProgress>();
		}

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
	
	// Update is called once per frame
	void Update () {
	
	}

	public void updateScore (int value) {
		score += value;

		GameObject scoreObj = GameObject.FindGameObjectWithTag ("Score");
		Text t = scoreObj.GetComponent<Text> ();
		t.text = score.ToString();
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

	public void closeInstructions () {
		instructionsDialog.SetActive (false);
	}

	public void showGameOver () {
		gameOverDialog.SetActive (true);
	}

	public void startGame() {
		Application.LoadLevel ("Play");
	}

	public void restartGame () {
		Application.LoadLevel (Application.loadedLevel);
	}

	// Note To Self - this is ignored in the editor and webplayer
	public void quitGame () {
		Application.Quit ();
	}
}
