using UnityEngine;
using System.Collections;

public class BirdController : Entity {

	public float minSpeed;
	public float maxSpeed;
	public float initialSpeed;
	public float speedVariance;

	public float initialAcceleration;
	public float maxAcceleration;
	public float accelerationIncrement;

	float acceleration;
	float moveSpeed;	// determined by a random number between minSpeed and maxSpeed

	bool dead;

	// Use this for initialization
	public override void Start () {
		base.Start ();

		//moveSpeed = Random.Range (minSpeed, maxSpeed);
		moveSpeed = initialSpeed + Random.Range (0, speedVariance);
		//moveSpeed = minSpeed;
		acceleration = initialAcceleration;
		rigidbody2D.velocity = new Vector2 (-1.0f * moveSpeed, rigidbody2D.velocity.y);
		dead = false;

		/*
		Physics2D.IgnoreLayerCollision (gameObject.layer, LayerMask.NameToLayer ("Background"), true);
		Physics2D.IgnoreLayerCollision (gameObject.layer, LayerMask.NameToLayer ("Platforms"), true);
		Physics2D.IgnoreLayerCollision (gameObject.layer, LayerMask.NameToLayer ("Enemies"), true);
		Physics2D.IgnoreLayerCollision (gameObject.layer, LayerMask.NameToLayer ("Player"), true);
		*/
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update ();
	}

	void FixedUpdate () {
		acceleration = Mathf.Clamp (acceleration + accelerationIncrement, initialAcceleration, maxAcceleration);
		//moveSpeed = Mathf.Clamp(moveSpeed + acceleration, minSpeed, maxSpeed);
		//Debug.Log ("moveSpeed: " + moveSpeed + " _ DeltaTime: " + Time.deltaTime);
		//rigidbody2D.velocity = new Vector2 (-1.0f * moveSpeed, rigidbody2D.velocity.y);
		//rigidbody2D.AddForce(new Vector2(acceleration * rigidbody2D.mass, 0.0f)); 
		rigidbody2D.AddRelativeForce (new Vector2 (acceleration * rigidbody2D.mass, 0.0f));
	}

	public void OnTriggerEnter2D(Collider2D collider)
	{
		if (!dead)
		{
			gameMaster.playerScore++;
			Debug.Log (gameMaster.playerScore.ToString());
		}
		dead = true;
	}
}
