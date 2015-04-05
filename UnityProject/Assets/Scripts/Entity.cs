using UnityEngine;
using System.Collections;

// Basic class which inherits MonoBehavior and has access
// to the Game Master.
//
// Note: It is important cloud scene 01 are overriden with -1 IDs to
// prevent users from overloading the instance manager.
public abstract class Entity : MonoBehaviour {
	public int entityID = -1;	// Keys less than 0 will be ignored by the instancing manager
	protected GameMaster gameMaster;

	public virtual void Awake () {
		Link();
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

	// Link to the Game Master
	public void Link()
	{
		GameObject _gameMaster = GameObject.FindGameObjectWithTag("GameEngine");
		gameMaster = _gameMaster.GetComponent<GameMaster>();
	}
}
