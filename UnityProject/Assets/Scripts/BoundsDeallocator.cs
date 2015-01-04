using UnityEngine;
using System.Collections;

public class BoundsDeallocator : MonoBehaviour {
	float width = 1.0f;
	float height = 20.0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void FixedUpdate () {
		Vector2 pos = new Vector2(transform.position.x, transform.position.y);
		Vector2 topLeft = pos + new Vector2(-width, height) / 2.0f;
		Vector2 bottomRight = pos + new Vector2(width, -height) / 2.0f;
		Collider2D[] collided = Physics2D.OverlapAreaAll(topLeft, bottomRight);

		foreach (Collider2D collider in collided)
			GameObject.Destroy (collider.gameObject);
	}
}
