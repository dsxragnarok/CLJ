using UnityEngine;
using System.Collections;

public abstract class BirdController : Entity {
	public enum BirdType { NONE, RED, BLUE, BLACK };

	public int score;

	[HideInInspector]
	public float initialPosition;
	protected float predictedPosition;
	public float initialVelocity;
	public float initialAcceleration;
	protected float acceleration;

	protected bool collected;


	public abstract BirdType Type {
		get;
	}

	public float PredictedPosition {
		get { return predictedPosition; }
	}
	
	private float cloudSpeed;
	private float cnter = 0f;

	// Use this for initialization
	public override void Start () {
		base.Start ();

		//moveSpeed = minSpeed;
		rigidbody2D.velocity = new Vector2 (initialVelocity, rigidbody2D.velocity.y);
		collected = false;
	}


	// Update is called once per frame
	public override void Update () {
		base.Update ();
		Debug.DrawLine (new Vector3 (initialPosition - cnter, this.transform.position.y, this.transform.position.z),
		                new Vector3 (predictedPosition - cnter, this.transform.position.y, this.transform.position.z),
		                predictedPosition - cnter > 0f ? Color.blue : Color.red);
	}

	public virtual void FixedUpdate () {
		cnter += cloudSpeed * Time.fixedDeltaTime;
		if (gameMaster.GameBounds.IsOutOfBounds(this.gameObject))
		{
			GameObject.Destroy(this.gameObject);
		}
	}

	public virtual void OnTriggerEnter2D(Collider2D collider)
	{
	}

	public float predictXmeet(float objX, float objSpd)
	{
		float xi = initialPosition;
		float vi = initialVelocity + objSpd;
		float ai = initialAcceleration;

		//Debug.Log(xi);
		//Debug.Log(vi);
		//Debug.Log(ai);
		//Debug.Log(objSpd);
		//Debug.Log(objX);
		// Formula
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

		//Debug.Log (t);

		cloudSpeed = objSpd;
		predictedPosition = objX + objSpd * t;
		return predictedPosition;
	}
}
