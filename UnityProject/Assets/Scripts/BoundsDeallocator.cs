﻿using UnityEngine;
using System.Collections;

// With described top, left, bottom, and right boundaries, it provides functions to check if an object
// goes out of bounds. 
// NOTE: it does not deallocate any objects itself anymore, but still provides the useful functions so
// objects can check themselves whether they are out of bounds or not.
public class BoundsDeallocator : Entity {
	float width = 60.0f;	// it needs to encompass the spawn point or spawned scenes will get cut off
	float height = 20.0f;

	// Boundary rectangle description
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

		if (debugMode) {
			Vector3 tl = new Vector3 (Left, Top, 9);
			Vector3 br = new Vector3 (Right, Bottom, 9);
			Vector3 tr = new Vector3 (Right, Top, 9);
			Vector3 bl = new Vector3 (Left, Bottom, 9);
			
			Debug.DrawLine (tl, tr, Color.red);
			Debug.DrawLine (tl, bl, Color.red);
			Debug.DrawLine (bl, br, Color.red);
			Debug.DrawLine (tr, br, Color.red);
		}
	}

	/*
	// This function is obsolete, the bounds deallocator does not deallocate objects anymore.
	// Secondly, it is inefficient using colliders.
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
	// Use this function if only specific sides of the bounds should be checked.
	public bool IsOutOfBounds(GameObject obj, bool checkTop, bool checkBottom, bool checkLeft, bool checkRight)
	{
		return (obj.transform.position.x < topLeft.x && checkLeft) ||
			(obj.transform.position.x > bottomRight.x && checkRight) ||
				(obj.transform.position.y > topLeft.y && checkTop) ||
				(obj.transform.position.y < bottomRight.y && checkBottom);
	}
}
