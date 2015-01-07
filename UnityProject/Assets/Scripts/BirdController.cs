using UnityEngine;
using System.Collections;

public class BirdController : MonoBehaviour {

	public float minSpeed;
	public float maxSpeed;
	public float initialSpeed;
	public float speedVariance;

	public float initialAcceleration;
	public float maxAcceleration;
	public float accelerationIncrement;

	float acceleration;
	float moveSpeed;	// determined by a random number between minSpeed and maxSpeed

	// Use this for initialization
	void Start () {
		//moveSpeed = Random.Range (minSpeed, maxSpeed);
		moveSpeed = initialSpeed + Random.Range (0, speedVariance);
		//moveSpeed = minSpeed;
		acceleration = initialAcceleration;
		rigidbody2D.velocity = new Vector2 (-1.0f * moveSpeed, rigidbody2D.velocity.y);

		Physics2D.IgnoreLayerCollision (gameObject.layer, LayerMask.NameToLayer ("Platforms"), true);
		Physics2D.IgnoreLayerCollision (gameObject.layer, LayerMask.NameToLayer ("Enemies"), true);
	}
	
	// Update is called once per frame
	void Update () {

	}

	void FixedUpdate () {
		acceleration = Mathf.Clamp (acceleration + accelerationIncrement, initialAcceleration, maxAcceleration);
		//moveSpeed = Mathf.Clamp(moveSpeed + acceleration, minSpeed, maxSpeed);
		//Debug.Log ("moveSpeed: " + moveSpeed + " _ DeltaTime: " + Time.deltaTime);
		//rigidbody2D.velocity = new Vector2 (-1.0f * moveSpeed, rigidbody2D.velocity.y);
		//rigidbody2D.AddForce(new Vector2(acceleration * rigidbody2D.mass, 0.0f)); 
		rigidbody2D.AddRelativeForce (new Vector2 (acceleration * rigidbody2D.mass, 0.0f));
	}
}
