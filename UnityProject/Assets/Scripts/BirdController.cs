using UnityEngine;
using System.Collections;

public class BirdController : MonoBehaviour {

	public float minSpeed;
	public float maxSpeed;

	float moveSpeed;	// determined by a random number between minSpeed and maxSpeed

	// Use this for initialization
	void Start () {
		moveSpeed = Random.Range (minSpeed, maxSpeed);
		rigidbody2D.velocity = new Vector2 (-1.0f * moveSpeed, rigidbody2D.velocity.y);
		Physics2D.IgnoreLayerCollision (gameObject.layer, LayerMask.NameToLayer ("Platforms"), true);
		Physics2D.IgnoreLayerCollision (gameObject.layer, LayerMask.NameToLayer ("Enemies"), true);
	}
	
	// Update is called once per frame
	void Update () {

	}

	void FixedUpdate () {

	}
}
