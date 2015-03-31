﻿using UnityEngine;
using System.Collections;

public class CloudPlatform : Entity {
	private float moveSpeed = 4.5f;
	public bool isSentinal = false;
	public bool activeTarget = true;

	public bool isCheckPoint = false;
	public bool collected = false;

	private int checkPointBonus = 200;

	// Use this for initialization
	public override void Start () {
		base.Start ();
			
		EdgeCollider2D platform = GetComponent<EdgeCollider2D>();
		
		// Assumes only two vertices per platform
		// If the cloud platform happens to be very vertical, change its layer name to "NotPlatforms"
		Vector2 p1 = (Vector2)platform.transform.position + (Vector2)(platform.transform.rotation * (Vector3)platform.points[0]);
		Vector2 p2 = (Vector2)platform.transform.position + (Vector2)(platform.transform.rotation * (Vector3)platform.points[1]);
		Vector2 delta = p2 - p1;
		if (System.Math.Abs (delta.y) >= 4 * System.Math.Abs (delta.x)) 
		{
			gameObject.layer = LayerMask.NameToLayer ("NotPlatforms");
		}
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update ();

		if (gameMaster.Player.IsDead ())
			moveSpeed = 0f;

		// A sentinal cloud determines when to spawn the next
		// group of clouds. When a sentinal crosses the left
		// edge of the spawner left edge, we call spawn
		if (isSentinal && transform.position.x < gameMaster.PlatformSpawner.Left + gameMaster.PlatformSpawner.sentinalOffset) {
			isSentinal = false;	// once spawned, deactivate this as a sentinal 
			CloudGroups cg = this.GetComponentInParent<CloudGroups>();
			gameMaster.PlatformSpawner.GeneratePlatform(cg.sceneList);
		}
	}

	void FixedUpdate () {
		if (gameMaster.isGameStarted) {
			Vector3 pos = transform.position;
			pos.x = pos.x - moveSpeed * Time.fixedDeltaTime;
			transform.position = pos;

			if (gameMaster.GameBounds.IsOutOfBounds (this.gameObject)) {
				gameMaster.InstancingManager.RecycleObject(this); // This removes itself as child but is 2 slow
			}

			if (activeTarget && gameMaster.BirdSpawner != null)
				gameMaster.BirdSpawner.CheckInsert(this);
		}
	}

	void OnCollisionEnter2D (Collision2D collider) {
		if (isCheckPoint && !collected && collider.gameObject.tag == "Player") {
			ActivateCheckPoint();
		}
	}

	void ActivateCheckPoint () {
		SpriteRenderer rend = this.GetComponent<SpriteRenderer>();
		rend.color = new Color(255,255,255);
		collected = true;
		gameMaster.updateScore (checkPointBonus);
		gameMaster.generateFloatingTextAt(gameMaster.Player.transform.position, checkPointBonus.ToString());
		//GameObject.Destroy (this.gameObject, 2.0f);
	}
	
	public override void SetToEntity(Entity entPrefab)
	{
		base.SetToEntity (entPrefab);

		// Set a lot of attributes to be like the prefab
		CloudPlatform cloudPlatformPrefab = entPrefab.GetComponent<CloudPlatform>();
		this.isSentinal = cloudPlatformPrefab.isSentinal;
		this.activeTarget = cloudPlatformPrefab.activeTarget;
		this.isCheckPoint = cloudPlatformPrefab.isCheckPoint;
		this.collected = cloudPlatformPrefab.collected;
		this.gameObject.layer = cloudPlatformPrefab.gameObject.layer;
		this.GetComponent<SpriteRenderer>().color = cloudPlatformPrefab.GetComponent<SpriteRenderer>().color;

		// Match the edge collider in case that is different
		EdgeCollider2D platformPrefab = cloudPlatformPrefab.GetComponent<EdgeCollider2D>();
		EdgeCollider2D platform = GetComponent<EdgeCollider2D>();
		platform.points = (Vector2[])platformPrefab.points.Clone ();

		// Check to see if it is a vertical cloud as it is done in Start()
		Vector2 p1 = (Vector2)platform.transform.position + (Vector2)(platform.transform.rotation * (Vector3)platform.points[0]);
		Vector2 p2 = (Vector2)platform.transform.position + (Vector2)(platform.transform.rotation * (Vector3)platform.points[1]);
		Vector2 delta = p2 - p1;
		if (System.Math.Abs (delta.y) >= 4 * System.Math.Abs (delta.x)) 
		{
			gameObject.layer = LayerMask.NameToLayer ("NotPlatforms");
		}
	}
}
