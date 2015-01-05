using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnBirds : MonoBehaviour {

	public float minY;
	public float maxY;
	public float initialDelay = 0.3f;	// the initial delay in seconds
	public float spawnInterval = 2.0f;	// number of seconds between spawns
	public List<GameObject> spawnList;

	// Use this for initialization
	void Start () {
		InvokeRepeating("GenerateBird", initialDelay, spawnInterval);
	}
	
	// Update is called once per frame
	void Update () {

	}

	void GenerateBird () {
		int rindex = Random.Range(0, spawnList.Count);
		Vector3 pos = this.transform.position;
		pos.y = pos.y + Random.Range (minY, maxY);
		//Debug.Log ("spawn cloud at y position = " + pos.y);
		Quaternion rot = this.transform.rotation;
		GameObject.Instantiate(spawnList[rindex], pos, rot);
	}
}
