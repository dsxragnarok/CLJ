using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnPlatforms : MonoBehaviour {
	public float minY = -2.0f;
	public float maxY = 2.0f;
	public List<GameObject> spawnList;
	// Use this for initialization
	void Start () {
		InvokeRepeating("GeneratePlatform", 1.0f, 1.0f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void GeneratePlatform()
	{
		int rindex = Random.Range(0, spawnList.Count);
		Vector3 pos = this.transform.position;
		pos.y = pos.y + Random.Range (minY, maxY);
		Quaternion rot = this.transform.rotation;
		GameObject.Instantiate(spawnList[rindex], pos, rot);
	}
}
