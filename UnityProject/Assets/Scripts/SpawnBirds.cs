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

	[Serializable]
	public struct SpawnCriteria
	{
		public GameObject obj;
		public int amount;
	}
	public List<SpawnCriteria> spawnListCriterias;
	private int totalAmount;
	
	private float edgeColliderBoxCheckWidth = 6f;
	private float edgeColliderBoxCheckHeight = 20.0f;

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
		
		float targetDest;

		Vector3 pos = this.transform.position;
		if (lineQualifiers.Length > 0)
		{
			int idx = UnityEngine.Random.Range (0, lineQualifiers.Length);
			targetDest = lineQualifiers[idx].transform.position.x;
			pos.y = lineQualifiers[idx].transform.position.y;
		}
		else
		{
			targetDest = gameMaster.Player.transform.position.x;
			pos.y = gameMaster.Player.transform.position.y;
		}
		pos.y = pos.y + UnityEngine.Random.Range (0.5f, 2.0f);

		float targetPos = pos.x;
		float cloudSpeed = 4f;
		float t = 1.0f;
		float targetAcc = -5f;
		float targetVel = (targetDest - targetPos) / t + cloudSpeed - 0.5f * targetAcc * t;

		Quaternion rot = this.transform.rotation;
		GameObject obj = (GameObject)GameObject.Instantiate(getRandomSpawnType (), pos, rot);
		if (_birdContainer)
			obj.transform.parent = _birdContainer.transform;
		BirdController birdie = obj.GetComponent<BirdController>();
		birdie.initialPosition = birdie.transform.position.x;
		birdie.initialAcceleration = -5f;
		birdie.initialVelocity = targetVel;
		birdie.predictXmeet(gameMaster.Player.XRest, cloudSpeed);

		/*
		float spawnY;
		//if (Random.Range (0, 100) > 50) {
			GameObject[] clouds = GameObject.FindGameObjectsWithTag ("Platform");
			GameObject cloud = clouds[UnityEngine.Random.Range(0,clouds.Length)];

			spawnY = cloud.transform.position.y;
		//} else {
		//	spawnY = Random.Range (minY, maxY);
		//}

		Vector3 pos = this.transform.position;
		pos.y = spawnY + 0.5f;
		//pos.y = pos.y + Random.Range (minY, maxY);
		*/

	}

	public void PositionBird(BirdController birdie)
	{
		Vector3 pos = this.transform.position;
		pos.x = birdie.PredictedPosition;
		lineQualifiers = Physics2D.OverlapAreaAll ((Vector2)pos + new Vector2 (-edgeColliderBoxCheckWidth, -edgeColliderBoxCheckHeight),
		                                                        (Vector2)pos + new Vector2 (edgeColliderBoxCheckWidth, edgeColliderBoxCheckHeight),
		                                                        gameMaster.Player.whatIsGround);
		pos.x = this.transform.position.x;
		if (lineQualifiers.Length > 0)
		{
			int idx = UnityEngine.Random.Range (0, lineQualifiers.Length);
			pos.y = lineQualifiers[idx].transform.position.y;
		}
		else
		{
			pos.y = gameMaster.Player.transform.position.y;
		}
		pos.y = pos.y + UnityEngine.Random.Range (0.5f, 2.0f);
		birdie.transform.position = pos;
	}

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
