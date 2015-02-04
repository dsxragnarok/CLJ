using UnityEngine;
using System.Collections;

public abstract class BirdController : Entity {
	public enum BirdType { NONE, RED, BLUE, BLACK };

	public int score;

	protected float initialPosition;
	private float predictedPosition;

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

		// Remove later, for debugging predictions
		maxAcceleration = 0.0f;
		accelerationIncrement = 0.0f;
		maxSpeed = 1000000.0f;
		minSpeed = 0.0f;

		initialPosition = transform.position.x;

		//moveSpeed = Random.Range (minSpeed, maxSpeed);
		moveSpeed = initialSpeed + Random.Range (0, speedVariance);
		//moveSpeed = minSpeed;
		acceleration = initialAcceleration;
		rigidbody2D.velocity = new Vector2 (-1.0f * moveSpeed, rigidbody2D.velocity.y);
		collected = false;

		predictedPosition = predictXmeet (CLOUD_SPEED, gameMaster.Player.XRest);
	}

	const float CLOUD_SPEED = 4f;
	private float cnter = 0f;

	// Update is called once per frame
	public override void Update () {
		base.Update ();
		Debug.Log (predictedPosition - cnter);
		Debug.DrawLine (new Vector3 (initialPosition - cnter, this.transform.position.y, this.transform.position.z),
		                new Vector3 (predictedPosition - cnter, this.transform.position.y, this.transform.position.z),
		                predictedPosition - cnter > 0f ? Color.blue : Color.red);
	}

	public virtual void FixedUpdate () {
		/*
		if (gameMaster.Player.IsDead())
		{
			rigidbody2D.velocity = Vector2.zero;
		}
		*/
		
		cnter += CLOUD_SPEED * Time.fixedDeltaTime;
		if (gameMaster.GameBounds.IsOutOfBounds(this.gameObject))
		{
			GameObject.Destroy(this.gameObject);
		}
	}

	public virtual void OnTriggerEnter2D(Collider2D collider)
	{
	}

	public float predictXmeet(float objSpd, float objX)
	{
		float xi = initialPosition;
		float vi = -initialSpeed;
		float ai = initialAcceleration;

		//0.5 ai * t^2 + vi * t + xi = objX + objSpd * t;
		//0.5 ai * t^2 + vi * t - objSpd * t = objX - xi;
		//0.5 ai * t^2 + (vi - objSpd) * t + xi - objX = 0
		float a = 0.5f * ai;
		float b = vi - objSpd;
		float c = xi - objX;
		float det = b * b - 4 * a * c;
		float t = 0f;

		if (a == 0f) {
			t = -c / b;
		}
		else if (det >= 0f) {
			float sqrtdet = UnityEngine.Mathf.Sqrt (det);
			t = (-b + sqrtdet) / (2f * a); 

			if (t < 0f) t = (-b - sqrtdet) / (2f * a);
		}

		float xprime = objX + objSpd * t;

		return xprime;
	}
}
