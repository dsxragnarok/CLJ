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

	public enum BirdType { NONE, RED, BLUE, BLACK };
	public BirdType type;

	public Sprite liveSprite;
	public Sprite deadSprite;

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
		if (gameMaster.Player.IsDead())
		{
			rigidbody2D.velocity = Vector2.zero;
		}
		if (!dead)
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
		
		if (gameMaster.GameBounds.IsOutOfBounds(this.gameObject))
		{
			GameObject.Destroy(this.gameObject);
		}
	}

	public void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.tag == "Player")
		{
			CharController charController = collider.GetComponent<CharController>();
			if (!dead)
			{
				if (type == BirdType.RED)
				{
					gameMaster.playerScore++;
					Debug.Log ("Score: " + gameMaster.playerScore.ToString());
				}
				else if (type == BirdType.BLACK)
				{
					charController.Die ();
				}
			}
			dead = true;

			GetComponent<SpriteRenderer>().sprite = deadSprite;
		}
	}
}
