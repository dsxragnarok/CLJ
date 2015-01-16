using UnityEngine;
using System.Collections;

public class GameMaster : MonoBehaviour {
	private CharController player;
	private BoundsDeallocator gameBounds;
	private SoundEffectsManager sfxManager;
	public int playerScore = 0;

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

	// Use this for initialization
	void Start () {
		GameObject _player = GameObject.FindGameObjectWithTag("Player");
		player = _player.GetComponent<CharController>();
		
		GameObject _gameBounds = GameObject.FindGameObjectWithTag("Bounds");
		gameBounds = _gameBounds.GetComponent<BoundsDeallocator>();

		GameObject _sfxManager = GameObject.FindGameObjectWithTag ("SoundEffect");
		sfxManager = _sfxManager.GetComponent<SoundEffectsManager> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
