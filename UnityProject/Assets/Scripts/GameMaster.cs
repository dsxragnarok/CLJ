using UnityEngine;
using System.Collections;

// TODO: maybe we'll use this.. not sure yet.

public class GameMaster : MonoBehaviour {
	private CharController player;
	private BoundsDeallocator gameBounds;
	public int playerScore = 0;

	public CharController Player
	{
		get { return player; }
	}
	
	public BoundsDeallocator GameBounds
	{
		get { return gameBounds; }
	}

	// Use this for initialization
	void Start () {
		GameObject _player = GameObject.FindGameObjectWithTag("Player");
		player = _player.GetComponent<CharController>();
		
		GameObject _gameBounds = GameObject.FindGameObjectWithTag("Bounds");
		gameBounds = _gameBounds.GetComponent<BoundsDeallocator>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
