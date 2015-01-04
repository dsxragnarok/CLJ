using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnPlatforms : MonoBehaviour {
	public float minY = -15.0f;
	public float maxY = 15.0f;
	public List<GameObject> spawnList;
	// Use this for initialization
	void Start () {
		InvokeRepeating("GeneratePlatform", 0.30f, 2.0f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void GeneratePlatform()
	{
		int rindex = Random.Range(0, spawnList.Count);
		Vector3 pos = this.transform.position;
		pos.y = pos.y + Random.Range (minY, maxY);
		Debug.Log ("spawn cloud at y position = " + pos.y);
		Quaternion rot = this.transform.rotation;
		GameObject.Instantiate(spawnList[rindex], pos, rot);
	}
}
