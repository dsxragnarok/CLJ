using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Manages how clouds are generated. It takes a list of scenes and waits for a trigger by a cloud flagged as a
// sentinel to generate the next cloud scene.
public class SpawnPlatforms : Entity {
	GameObject _platformContainer;

	public float minY = -15.0f;
	public float maxY = 15.0f;
	public float xOffset = 5;
	public float sentinalOffset = -14;
	public float initialDelay = 0.3f;	// the initial delay in seconds
	public float spawnInterval = 2.0f;	// number of seconds between spawns
	public List<GameObject> spawnList;	

	/* ** */
	float width = 30.0f;
	float height = 20.0f;
	
	Vector2 pos;
	Vector2 topLeft;
	Vector2 bottomRight;
	
	bool debugMode = true;
	
	public float Left
	{
		get { return topLeft.x; }
	}
	
	public float Right
	{
		get { return bottomRight.x; }
	}
	
	public float Top
	{
		get { return topLeft.y; }
	}
	
	public float Bottom
	{
		get { return bottomRight.y; }
	}
	/* ** */


	public override void Awake () {
		base.Awake ();

		_platformContainer = GameObject.FindGameObjectWithTag("PlatformContainer");
	}
	
	// Use this for initialization
	public override void Start () {
		base.Start ();

		pos = new Vector2(transform.position.x, transform.position.y);
		topLeft = pos + new Vector2(0f, height / 2f);
		bottomRight = pos + new Vector2(width, -height / 2f);

		//GeneratePlatform ();
		//InvokeRepeating("GeneratePlatform", initialDelay, spawnInterval);
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update ();

		if (debugMode) {
			Vector3 tl = new Vector3 (Left, Top, 9);
			Vector3 br = new Vector3 (Right, Bottom, 9);
			Vector3 tr = new Vector3 (Right, Top, 9);
			Vector3 bl = new Vector3 (Left, Bottom, 9);
			
			Debug.DrawLine (tl, tr, Color.yellow);
			Debug.DrawLine (tl, bl, Color.yellow);
			Debug.DrawLine (bl, br, Color.yellow);
			Debug.DrawLine (tr, br, Color.yellow);
		}
	}

	public void GeneratePlatform(List<GameObject> scenes)
	{
		if (gameMaster.Player.IsDead () || !gameMaster.isGameStarted)
			return;
		int rindex = Random.Range (0, scenes.Count);//Random.Range(0, spawnList.Count);
		Vector3 pos = this.transform.position;
		//pos.x = pos.x + xOffset;
		pos.y = pos.y + Random.Range (minY, maxY);
		pos.x = pos.x + xOffset;
		//Debug.Log ("spawn cloud at y position = " + pos.y);
		Quaternion rot = this.transform.rotation;

		//GameObject obj = GameObject.Instantiate(scenes[rindex], pos, rot) as GameObject;

		// Construct a new cloud scene container
		GameObject obj = new GameObject(scenes[rindex].name);
		CloudGroups cloudGroupsPrefab = scenes[rindex].GetComponent<CloudGroups>();
		CloudGroups cloudGroupsInstance = obj.AddComponent<CloudGroups>();

		// Using the prefab we want to build, go through each children and grab it from
		// the instance manager.
		cloudGroupsInstance.sceneList = new List<GameObject>(cloudGroupsPrefab.sceneList);
		Entity[] entities = cloudGroupsPrefab.GetComponentsInChildren<Entity>(true);
		foreach (Entity ent in entities)
		{
			if (ent.gameObject != cloudGroupsPrefab.gameObject)
			{
				Entity entInstance = gameMaster.InstancingManager.RetrieveObject(ent);
				if (entInstance != null)
				{
					entInstance.transform.parent = obj.transform;
					entInstance.SetToEntity(ent);
				}
			}
		}
		obj.transform.position = pos;
		obj.transform.rotation = rot;
		if (_platformContainer)
			obj.transform.parent = _platformContainer.transform;

		if (gameMaster.DifficultyManager)
			gameMaster.DifficultyManager.LevelUp();
	}

}
