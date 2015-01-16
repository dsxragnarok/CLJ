using UnityEngine;
using System.Collections;

public class BirdBlue : BirdController {
	public Sprite deadSprite;

	public override BirdType Type
	{
		get { return BirdType.BLUE; }
	}

	// Use this for initialization
	public override void Start () {
		base.Start ();

		score = -5;
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update ();
	}

	public override void FixedUpdate () {
		base.FixedUpdate ();

		if (!collected)
		{
			acceleration = Mathf.Clamp (acceleration + accelerationIncrement, initialAcceleration, maxAcceleration);
			//moveSpeed = Mathf.Clamp(moveSpeed + acceleration, minSpeed, maxSpeed);
			//Debug.Log ("moveSpeed: " + moveSpeed + " _ DeltaTime: " + Time.deltaTime);
			//rigidbody2D.velocity = new Vector2 (-1.0f * moveSpeed, rigidbody2D.velocity.y);
			//rigidbody2D.AddForce(new Vector2(acceleration * rigidbody2D.mass, 0.0f)); 
			rigidbody2D.AddRelativeForce (new Vector2 (acceleration * rigidbody2D.mass, 0.0f));
		}
		else
		{
			rigidbody2D.velocity = Vector2.up * -10f;
			transform.rigidbody2D.transform.Rotate(Vector3.forward * -90f * Time.fixedDeltaTime);
		}
	}

	public override void OnTriggerEnter2D(Collider2D collider) {
		base.OnTriggerEnter2D (collider);
		if (collider.tag == "Player")
		{
			if (!collected) 
			{
				GetComponent<SpriteRenderer>().sprite = deadSprite;
				collected = true;
			}
		}
	}
}
