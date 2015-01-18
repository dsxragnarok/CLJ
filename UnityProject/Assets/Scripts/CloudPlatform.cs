using UnityEngine;
using System.Collections;

public class CloudPlatform : Entity {
	private float moveSpeed = 4.0f;
	// Use this for initialization
	public override void Start () {
		base.Start ();
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update ();
		if (gameMaster.Player.IsDead ())
			moveSpeed = 0f;
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
