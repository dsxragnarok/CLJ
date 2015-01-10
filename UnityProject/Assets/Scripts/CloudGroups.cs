using UnityEngine;
using System.Collections;

public class CloudGroups : Entity {
	public float moveSpeed = 3.0f;
	// Use this for initialization
	public override void Start () {
		base.Start ();
	
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update ();
	}

	void FixedUpdate() {
		Vector3 pos = transform.position;
		pos.x = pos.x - moveSpeed * Time.fixedDeltaTime;
		transform.position = pos;
	}
}
