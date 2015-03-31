using UnityEngine;
using System.Collections;

// Basic class which inherits MonoBehavior and has access
// to the Game Master.
public abstract class Entity : MonoBehaviour {
	public int entityID = -1;
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

	// This assigns attributes of the entity prefab to this instance.
	// NOTE: Since we are recycling these game objects, we must take all
	// attributes from a class and override this function that we want to re-initialize.
	// We also need to discover which prefab values are needed to be set in this function.
	public virtual void SetToEntity(Entity entPrefab)
	{
	}
}
