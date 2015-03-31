using UnityEngine;
using System.Collections;

public class CloudPlatform : Entity {
	private float moveSpeed = 4.5f;
	public bool isSentinal = false;
	public bool activeTarget = true;

	public bool isCheckPoint = false;
	public bool collected = false;

	private int checkPointBonus = 100;

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
				GameObject.Destroy (this.gameObject);

				GameObject[] cloudGroups = GameObject.FindGameObjectsWithTag ("CloudGroup");
				foreach (GameObject gobj in cloudGroups) {
						if (gobj.transform.childCount == 0)
								GameObject.Destroy (gobj);
				}
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

		gameMaster.PlayerData.totalCheckpointsCollected += 1;

		//int totalCheckPoints = gameMaster.PlayerData.totalCheckpointsCollected;
		int totalCheckPointBonus = checkPointBonus * gameMaster.checkpointsPassed;
		gameMaster.updateScore (totalCheckPointBonus);
		gameMaster.generateFloatingTextAt(gameMaster.Player.transform.position, totalCheckPointBonus.ToString());
		//GameObject.Destroy (this.gameObject, 2.0f);
	}
}
