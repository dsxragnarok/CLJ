using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnBirds : MonoBehaviour {
	GameObject _birdContainer;

	public float minY;
	public float maxY;
	public float initialDelay = 0.3f;	// the initial delay in seconds
	public float spawnInterval = 2.0f;	// number of seconds between spawns
	public List<GameObject> spawnList;
	
	void Awake () {
		_birdContainer = GameObject.FindGameObjectWithTag("BirdContainer");
	}

	// Use this for initialization
	void Start () {
		// TODO: change the spawnInterval based on score -- possibly will require using invoke instead of invokerepeating
		InvokeRepeating("GenerateBird", initialDelay, spawnInterval);
	}
	
	// Update is called once per frame
	void Update () {

	}

	void GenerateBird () {
		float spawnY;
		//if (Random.Range (0, 100) > 50) {
			GameObject[] clouds = GameObject.FindGameObjectsWithTag ("Platform");
			GameObject cloud = clouds[Random.Range(0,clouds.Length)];

			spawnY = cloud.transform.position.y;
		//} else {
		//	spawnY = Random.Range (minY, maxY);
		//}

		int rindex = Random.Range(0, spawnList.Count);
		Vector3 pos = this.transform.position;
		pos.y = spawnY + 0.5f;
		//pos.y = pos.y + Random.Range (minY, maxY);

		Quaternion rot = this.transform.rotation;
		GameObject obj = (GameObject)GameObject.Instantiate(spawnList[rindex], pos, rot);
		if (_birdContainer)
			obj.transform.parent = _birdContainer.transform;
	}
}
