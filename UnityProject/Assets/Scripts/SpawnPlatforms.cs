using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnPlatforms : Entity {
	GameObject _platformContainer;

	public float minY = -15.0f;
	public float maxY = 15.0f;
	public float initialDelay = 0.3f;	// the initial delay in seconds
	public float spawnInterval = 2.0f;	// number of seconds between spawns
	public List<GameObject> spawnList;	

	public override void Awake () {
		base.Awake ();

		_platformContainer = GameObject.FindGameObjectWithTag("PlatformContainer");
	}
	
	// Use this for initialization
	public override void Start () {
		base.Start ();

		InvokeRepeating("GeneratePlatform", initialDelay, spawnInterval);
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update ();
	}

	public void GeneratePlatform()
	{
		if (gameMaster.Player.IsDead ())
			return;
		int rindex = Random.Range(0, spawnList.Count);
		Vector3 pos = this.transform.position;
		pos.y = pos.y + Random.Range (minY, maxY);
		//Debug.Log ("spawn cloud at y position = " + pos.y);
		Quaternion rot = this.transform.rotation;
		GameObject obj = (GameObject)GameObject.Instantiate(spawnList[rindex], pos, rot);

		//if (Random.Range (0, 100) > 50) {
	//		Vector3 localScale = obj.transform.localScale;
	//		obj.transform.localScale = new Vector3(localScale.x * -1, localScale.y, localScale.z);
	//	}
			

		if (_platformContainer)
			obj.transform.parent = _platformContainer.transform;
	}
}
