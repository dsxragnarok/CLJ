using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// Manages how birds are spawned. It currently takes a list of active clouds (approaching the player)
// and measures a proper trajectory so that it'll be somewhere reachable by the time the cloud gets to the player.
public class SpawnBirds : Entity {
	GameObject _birdContainer;

	public float minY;
	public float maxY;
	public float initialDelay = 0.5f;	// the initial delay in seconds
	public float spawnInterval = 3.0f;	// number of seconds between spawns

	// A definition of a spawn type and the amount (more means more often)
	[Serializable]
	public class SpawnCriteria
	{
		public BirdController obj;
		public int amount;
	}
	public List<SpawnCriteria> spawnListCriterias;
	private int totalAmount;

	// Moving window that looks for all cloud platforms for potential spawn destinations
	// Cloud platforms found and updated are here
	private List<CloudPlatform> targetPlatforms;
	private float exitLine;		// x-location on the left side when the cloud leaves the candidate list
	private float enterLine;	// x-location on the right when the cloud enters the candidate list

	// Every fixed frame, we check to see which clouds in the list are still in the candidate region.
	// Any cloud that passes the exit line is not added into the list anymore (and its active flag is set to false).
	// Turning off the active flag ensures it won't be added again.
	public void CheckRemove()
	{
		List<CloudPlatform> ret = new List<CloudPlatform>();
		foreach (CloudPlatform platform in targetPlatforms)
		{
			if (platform.transform.position.x > exitLine)
				ret.Add (platform);
			else
				platform.activeTarget = false;
		}
		targetPlatforms = ret;
	}

	// CloudPlatforms may offer to register themselves into the candidate list, and if they exceed the enter line, 
	// they are added to the candidate list.
	public void CheckInsert(CloudPlatform platform)
	{
		if (platform.transform.position.x <= enterLine)
			targetPlatforms.Add (platform);
	}

	public override void Awake () {
		base.Awake ();

		_birdContainer = GameObject.FindGameObjectWithTag("BirdContainer");

		UpdateCriteriaTotalAmount();
	}

	// Use this for initialization
	public override void Start () {
		base.Start ();

		float offsetx = 2.5f;
		targetPlatforms = new List<CloudPlatform>();
		float midx = (gameMaster.Player.XRest + this.transform.position.x) / 2f;
		exitLine = midx - offsetx;
		enterLine = midx + offsetx;
		Invoke("PerformSpawn", initialDelay);
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update ();
	}

	public void FixedUpdate() {
		CheckRemove();
	}

	void PerformSpawn() {
		// Keep spawning birds using the spawn interval
		GenerateBird();
		Invoke("PerformSpawn", spawnInterval);
	}

	void GenerateBird () {
		if (gameMaster.Player.IsDead () || !gameMaster.isGameStarted)
			return;

		// The target meeting location between home position and bird
		Vector2 targetDest;

		if (targetPlatforms.Count <= 0) {
			return;
		}

		BirdController spawnPrefab = getRandomSpawnType ();

		// Obtain a random cloud
		int idxPlatform = UnityEngine.Random.Range (0, targetPlatforms.Count);

		// Obtain locations where we can spawn birds fairly
		targetDest = targetPlatforms[idxPlatform].transform.position;	
		// Spawn with a little bit of y offset
		targetDest = targetDest + new Vector2(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range (0.0f, 3.0f));

		// Predict when home position reaches the point we want bird and home to meet
		float cloudSpeed = 4.5f;

		// x1 + v1 * t = x2 + v2 * t => x2 = x1 + (v1 - v2) * t
		// Predict home position using desired velocity and acceleration and time
		Vector2 targetAcc = new Vector2(UnityEngine.Random.Range (-3.5f, -2.5f), UnityEngine.Random.Range (-2.5f, -1.5f));
		Vector2 targetVel = new Vector2(UnityEngine.Random.Range (-8.0f, -6.0f), UnityEngine.Random.Range (7.0f, 9.0f));
		Vector2 targetPos;

		float t = 2.0f;
		targetPos = targetDest + (new Vector2(-cloudSpeed, 0.0f) - targetVel) * t - targetAcc * t * t * 0.5f;
	
		// Instantiate bird with parameters
		Quaternion rot = this.transform.rotation;
		//BirdController obj = (BirdController)GameObject.Instantiate(spawnPrefab, targetPos, rot);
		BirdController obj = gameMaster.InstancingManager.RetrieveObject(spawnPrefab).GetComponent<BirdController>();
		obj.transform.position = targetPos;
		obj.transform.rotation = rot;
		obj.SetToEntity(spawnPrefab);
		if (obj != null)
		{
			if (_birdContainer)
				obj.transform.parent = _birdContainer.transform;

			BirdController birdie = obj.GetComponent<BirdController>();
			birdie.initialPosition = targetPos;
			birdie.initialAcceleration = targetAcc;
			birdie.initialVelocity = targetVel;
		}
	}

	// Returns random bird type based on the spawn criterias
	private BirdController getRandomSpawnType()
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
	
	public SpawnCriteria FindSpawnCriteria(BirdController.BirdType type)
	{
		foreach (SpawnCriteria criteria in spawnListCriterias)
		{
			if (criteria.obj.Type == type)
				return criteria;
		}
		return null;
	}

	public void UpdateCriteriaTotalAmount()
	{
		totalAmount = 0;
		for (int i = 0; i < spawnListCriterias.Count; ++i)
			totalAmount += spawnListCriterias [i].amount;
	}
}
