using UnityEngine;
using System.Collections;

public abstract class BirdController : Entity {
	public enum BirdType { NONE, RED, BLUE, BLACK };

	public int score;

	public float minSpeed;
	public float maxSpeed;
	public float initialSpeed;
	public float speedVariance;

	public float initialAcceleration;
	public float maxAcceleration;
	public float accelerationIncrement;

	protected float acceleration;
	protected float moveSpeed;	// determined by a random number between minSpeed and maxSpeed

	protected bool collected;

	public abstract BirdType Type {
				get;
	}

	// Use this for initialization
	public override void Start () {
		base.Start ();

		//moveSpeed = Random.Range (minSpeed, maxSpeed);
		moveSpeed = initialSpeed + Random.Range (0, speedVariance);
		//moveSpeed = minSpeed;
		acceleration = initialAcceleration;
		rigidbody2D.velocity = new Vector2 (-1.0f * moveSpeed, rigidbody2D.velocity.y);
		collected = false;
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update ();
	}

	public virtual void FixedUpdate () {
		/*
		if (gameMaster.Player.IsDead())
		{
			rigidbody2D.velocity = Vector2.zero;
		}
		*/

		if (gameMaster.GameBounds.IsOutOfBounds(this.gameObject))
		{
			GameObject.Destroy(this.gameObject);
		}
	}

	public virtual void OnTriggerEnter2D(Collider2D collider)
	{
	}
}
