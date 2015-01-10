using UnityEngine;
using System.Collections;

public class BoundsDeallocator : Entity {
	float width = 30.0f;
	float height = 20.0f;

	Vector2 pos;
	Vector2 topLeft;
	Vector2 bottomRight;

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

	// Use this for initialization
	public override void Start () {
		base.Start ();

		pos = new Vector2(transform.position.x, transform.position.y);
		topLeft = pos + new Vector2(0f, height / 2f);
		bottomRight = pos + new Vector2(width, -height / 2f);
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update ();
	}

	/*
	void FixedUpdate () {
		Collider2D[] collided = Physics2D.OverlapAreaAll(topLeft, bottomRight);

		foreach (Collider2D collider in collided) {
			if (collider.tag == "Platform" || collider.tag == "Birdie")
				GameObject.Destroy (collider.gameObject);
			else if (collider.tag == "Player")
			{
				CharController player = collider.GetComponent<CharController>();
				player.Die ();
			}
		}

		GameObject[] cloudGroups = GameObject.FindGameObjectsWithTag ("CloudGroup");
		foreach (GameObject gobj in cloudGroups) {
			if (gobj.transform.childCount == 0)
				GameObject.Destroy(gobj);
		}
	}
	*/

	// Returns true if the object parameter is out of the rectangle
	public bool IsOutOfBounds(GameObject obj)
	{
		return obj.transform.position.x < topLeft.x ||
			obj.transform.position.x > bottomRight.x ||
			obj.transform.position.y > topLeft.y ||
			obj.transform.position.y < bottomRight.y;
	}
}
