using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class GameMaster : MonoBehaviour {
	private CharController player;
	private BoundsDeallocator gameBounds;
	private SoundEffectsManager sfxManager;
	private SpawnPlatforms platformSpawner;
	private SpawnBirds birdSpawner;
	public int playerScore = 0;

	public GameObject gameOverDialog;
	public GameObject instructionsDialog;

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
		}

		GameObject _sfxManager = GameObject.FindGameObjectWithTag ("SoundEffect");
		if (_sfxManager != null)
			sfxManager = _sfxManager.GetComponent<SoundEffectsManager> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void updateScore (int delta) {
		playerScore += delta;
		GameObject scoreObj = GameObject.FindGameObjectWithTag ("Score");
		Text t = scoreObj.GetComponent<Text> ();
		t.text = "Score: " + playerScore;
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
