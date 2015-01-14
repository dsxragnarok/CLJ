using UnityEngine;
using System.Collections;

// Basic class which inherits MonoBehavior and has access
// to the Game Master.
public abstract class Entity : MonoBehaviour {
	protected GameMaster gameMaster;

	public virtual void Awake () {
		GameObject _gameMaster = GameObject.FindGameObjectWithTag("GameEngine");
		gameMaster = _gameMaster.GetComponent<GameMaster>();
	}

	// Use this for initialization
	public virtual void Start () {
	}
	
	// Update is called once per frame
	public virtual void Update () {
	
	}
}
