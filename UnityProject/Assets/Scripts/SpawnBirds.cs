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

	void GenerateBird () {
		if (gameMaster.Player.IsDead ())
			return;

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

		Quaternion rot = this.transform.rotation;
		GameObject obj = (GameObject)GameObject.Instantiate(getRandomSpawnType (), pos, rot);
		if (_birdContainer)
			obj.transform.parent = _birdContainer.transform;

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
