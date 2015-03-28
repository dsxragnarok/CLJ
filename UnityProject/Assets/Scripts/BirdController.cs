using UnityEngine;
using System.Collections;

public abstract class BirdController : Entity {

	protected Rigidbody2D unitRigidbody;

	public enum BirdType { NONE, RED, BLUE, BLACK };

	public int score;

	[HideInInspector]
	public Vector2 initialPosition;
	public Vector2 initialVelocity;
	public Vector2 initialAcceleration;
	protected Vector2 acceleration;

	protected bool collected;


	public abstract BirdType Type {
		get;
	}

	// Use this for initialization
	public override void Start () {
		base.Start ();
		
		unitRigidbody = GetComponent<Rigidbody2D> ();

		//moveSpeed = minSpeed;
		//rigidbody2D.velocity = new Vector2 (initialVelocity, rigidbody2D.velocity.y);
		unitRigidbody.velocity = initialVelocity;
		collected = false;
	}


	// Update is called once per frame
	public override void Update () {
		base.Update ();
	}

	public virtual void FixedUpdate () {
		if (gameMaster.GameBounds.IsOutOfBounds(this.gameObject, true, false, false, true))
		{
			GameObject.Destroy(this.gameObject);
		}
	}

	public virtual void OnTriggerEnter2D(Collider2D collider)
	{
	}
}
