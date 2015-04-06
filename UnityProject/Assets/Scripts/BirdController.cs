using UnityEngine;
using System.Collections;

// Base class for a bird game object. It contains
// all basic values such as bird rigidbody information and movement logic.
public abstract class BirdController : Entity {

	protected Rigidbody2D unitRigidbody = null;

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

	public override void Awake () {
		base.Awake ();

		unitRigidbody = this.GetComponent<Rigidbody2D> ();
	}

	// Use this for initialization
	public override void Start () {
		base.Start ();

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
		// Don't check bottom and right side bounds until bird is collected
		// This is to potentially prevent spawned birds to immediately be destroyed
		if (gameMaster.GameBounds.IsOutOfBounds(this.gameObject, true, collected, collected, true))
		{
			gameMaster.InstancingManager.RecycleObject(this);
		}
	}

	public virtual void OnTriggerEnter2D(Collider2D collider)
	{
	}
	
	public override void SetToEntity(Entity entPrefab)
	{
		base.SetToEntity (entPrefab);
		BirdController birdControllerPrefab = entPrefab.GetComponent<BirdController>();
		this.score = birdControllerPrefab.score;
		this.collected = birdControllerPrefab.collected;

		this.unitRigidbody.velocity = initialVelocity;
	}
}
