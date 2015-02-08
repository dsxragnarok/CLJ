using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SpawnBirds : Entity {
	GameObject _birdContainer;

	public float minY;
	public float maxY;
	public float initialDelay = 0.3f;	// the initial delay in seconds
	public float spawnInterval = 2.0f;	// number of seconds between spawns

	// A definition of a spawn type and the amount (more means more often)
	[Serializable]
	public struct SpawnCriteria
	{
		public GameObject obj;
		public int amount;
	}
	public List<SpawnCriteria> spawnListCriterias;
	private int totalAmount;

	// Moving window that looks for all cloud platforms for potential spawn destinations
	private float edgeColliderBoxCheckWidth = 6f;
	private float edgeColliderBoxCheckHeight = 20.0f;
	// Cloud platforms found and updated are here
	private Collider2D[] lineQualifiers;		

	public override void Awake () {
		base.Awake ();

		_birdContainer = GameObject.FindGameObjectWithTag("BirdContainer");

		totalAmount = 0;
		for (int i = 0; i < spawnListCriterias.Count; ++i)
			totalAmount += spawnListCriterias [i].amount;
	}

	// Use this for initialization
	public override void Start () {
		base.Start ();

		// TODO: change the spawnInterval based on score -- possibly will require using invoke instead of invokerepeating
		InvokeRepeating("GenerateBird", initialDelay, spawnInterval);
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update ();
	}

	public void FixedUpdate() {
		base.Update ();

		float midx = (gameMaster.Player.XRest + this.transform.position.x) / 2f;
		Vector3 pos = this.transform.position;
		pos.x = midx;
		lineQualifiers = Physics2D.OverlapAreaAll ((Vector2)pos + new Vector2 (-edgeColliderBoxCheckWidth, -edgeColliderBoxCheckHeight) / 2f,
		                                           (Vector2)pos + new Vector2 (edgeColliderBoxCheckWidth, edgeColliderBoxCheckHeight) / 2f,
		                                           gameMaster.Player.whatIsGround);
	}

	void GenerateBird () {
		if (gameMaster.Player.IsDead () || !gameMaster.isGameStarted)
			return;

		// The target meeting location between home position and bird
		float targetDest;

		Vector3 pos = this.transform.position;
		if (lineQualifiers.Length <= 0) {
			return;
		}

		// Obtain a random cloud
		int idxPlatform = UnityEngine.Random.Range (0, lineQualifiers.Length);
		SpawnOffset spawnOffsetter = lineQualifiers[idxPlatform].GetComponent<SpawnOffset> ();

		// Obtain locations where we can spawn birds fairly
		targetDest = lineQualifiers[idxPlatform].transform.position.x;		
		pos.y = lineQualifiers[idxPlatform].transform.position.y;
		if (spawnOffsetter) 
		{
			Vector2 offset = spawnOffsetter.getRandomOffset ();

			targetDest = targetDest + offset.x;
			pos.y = pos.y + offset.y;
		}

		// Predict when home position reaches the point we want bird and home to meet
		float targetPos = pos.x;
		float cloudSpeed = 4.5f;
		float t = (targetDest - gameMaster.Player.XRest) / cloudSpeed;
		// Obtain desired bird velocity to achieve the above prediction
		float targetAcc = -5f;
		float targetVel = (targetDest - targetPos) / t - cloudSpeed - 0.5f * targetAcc * t;

		// Instantiate bird with parameters
		Quaternion rot = this.transform.rotation;
		GameObject obj = (GameObject)GameObject.Instantiate(getRandomSpawnType (), pos, rot);
		if (_birdContainer)
			obj.transform.parent = _birdContainer.transform;
		BirdController birdie = obj.GetComponent<BirdController>();
		birdie.initialPosition = birdie.transform.position.x;
		birdie.initialAcceleration = -5f;
		birdie.initialVelocity = targetVel;
		birdie.predictXmeet(gameMaster.Player.XRest, cloudSpeed);
	}

	// Returns random bird type based on the spawn criterias
	private GameObject getRandomSpawnType()
	{
		int rvalue = UnityEngine.Random.Range (0, totalAmount);
		int check = 0;
		for (int i = 0; i < spawnListCriterias.Count; ++i)
		{
			check += spawnListCriterias[i].amount;
			if (rvalue < check) return spawnListCriterias[i].obj;
		}
		return null;
	}
}
