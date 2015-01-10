using UnityEngine;
using System.Collections;

public class BoundsDeallocator : Entity {
	float width = 1.0f;
	float height = 20.0f;

	Vector2 pos;
	Vector2 topLeft;
	Vector2 bottomRight;
	// Use this for initialization
	public override void Start () {
		base.Start ();

		pos = new Vector2(transform.position.x, transform.position.y);
		topLeft = pos + new Vector2(-width, height) / 2.0f;
		bottomRight = pos + new Vector2(width, -height) / 2.0f;
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update ();
	}
	
	void FixedUpdate () {
		Collider2D[] collided = Physics2D.OverlapAreaAll(topLeft, bottomRight);

		foreach (Collider2D collider in collided) {
			if (collider.tag == "Platform")
				GameObject.Destroy (collider.gameObject);
		}

		GameObject[] cloudGroups = GameObject.FindGameObjectsWithTag ("CloudGroup");
		foreach (GameObject gobj in cloudGroups) {
			if (gobj.transform.childCount == 0)
				GameObject.Destroy(gobj);
		}
	}
}
