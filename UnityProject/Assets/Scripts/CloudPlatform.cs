using UnityEngine;
using System.Collections;

public class CloudPlatform : Entity {
	private float moveSpeed = 4.0f;
	public bool isSentinal = false;
	// Use this for initialization
	public override void Start () {
		base.Start ();
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
			gameMaster.PlatformSpawner.GeneratePlatform();
		}
	}

	void FixedUpdate () {

		Vector3 pos = transform.position;
		pos.x = pos.x - moveSpeed * Time.fixedDeltaTime;
		transform.position = pos;

		if (gameMaster.GameBounds.IsOutOfBounds(this.gameObject))
		{
			GameObject.Destroy(this.gameObject);
			
			GameObject[] cloudGroups = GameObject.FindGameObjectsWithTag ("CloudGroup");
			foreach (GameObject gobj in cloudGroups) {
				if (gobj.transform.childCount == 0)
					GameObject.Destroy(gobj);
			}
		}
	}
}
