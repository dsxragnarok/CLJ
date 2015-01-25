using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class GameMaster : MonoBehaviour {
	private CharController player;
	private BoundsDeallocator gameBounds;
	private SoundEffectsManager sfxManager;
	private SpawnPlatforms platformSpawner;
	public int playerScore = 0;

	public GameObject gameOverDialog;

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

	// Use this for initialization
	void Start () {
		GameObject _player = GameObject.FindGameObjectWithTag("Player");
		player = _player.GetComponent<CharController>();
		
		GameObject _gameBounds = GameObject.FindGameObjectWithTag("Bounds");
		gameBounds = _gameBounds.GetComponent<BoundsDeallocator>();

		GameObject _spawner = GameObject.FindGameObjectWithTag ("Spawner");
		platformSpawner = _spawner.GetComponent<SpawnPlatforms> ();

		GameObject _sfxManager = GameObject.FindGameObjectWithTag ("SoundEffect");
		sfxManager = _sfxManager.GetComponent<SoundEffectsManager> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void updateScore (int delta) {
		playerScore += delta;
		GameObject scoreObj = GameObject.FindGameObjectWithTag ("Score");
		Debug.Log (scoreObj);
		Text t = scoreObj.GetComponent<Text> ();
		t.text = "Score: " + playerScore;
	}

	public void showGameOver () {
		gameOverDialog.SetActive (true);
	}

	public void restartGame () {
		Application.LoadLevel (Application.loadedLevel);
	}

	// Note To Self - this is ignored in the editor and webplayer
	public void quitGame () {
		Application.Quit ();
	}
}
