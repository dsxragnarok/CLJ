using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharController : Entity {

	public float maxSpeed;

	// Jump Variables
	private int jumpPhase;			// 0=no jump 1=1st jump 2=2nd jump
	private int jumpForceIndex;		
	public List<float> jumpForces;

	private bool grounded;
	public Transform groundCheck;
	private float groundRadius;
	public LayerMask whatIsGround;

	// X rest position
	private float xRest;

	private Collider2D[] colliders;

	private bool dead;

	// Use this for initialization
	public override void Start () {
		base.Start ();

		jumpPhase = 0;
		jumpForceIndex = 0;
		grounded = false;
		xRest = rigidbody2D.position.x;
		colliders = GetComponents<Collider2D>();
		groundRadius = GetComponent<CircleCollider2D>().radius;
		dead = false;
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update ();

		// Perform jump if we are on ground or this is our double jump
		//if ((grounded || jumpPhase < 2) && Input.GetKeyDown (KeyCode.Space))
		// Perform jump if we are on ground
		if (!IsDead() && grounded && Input.GetKeyDown (KeyCode.Space))
		{
			StopCoroutine(performJump());
			StartCoroutine(performJump());
		}
	}

	void FixedUpdate () {
		grounded = Physics2D.OverlapArea (groundCheck.position - Vector3.left * groundRadius - Vector3.up * groundRadius, 
		                                  groundCheck.position + Vector3.left * groundRadius + Vector3.up * groundRadius, 
		                                  whatIsGround);
		//grounded = Physics2D.OverlapCircle (groundCheck.position, groundRadius, whatIsGround);
		// Reset jump phase if we are grounded so the person can jump again
		if (grounded && jumpForceIndex > 0)
			jumpPhase = 0;

		float move = 0.0f;
		// Return to home x-position at a constant velocity
		if (IsDead())
		{
			if (rigidbody2D.position.x < xRest - 0.1f)
				move = maxSpeed;
			if (rigidbody2D.position.x > xRest + 0.1f)
				move = -maxSpeed;
		}
		rigidbody2D.velocity = new Vector2(move, rigidbody2D.velocity.y); 	

		// Ignore platform collisions if we are airborne
		GameObject[] objs = GameObject.FindGameObjectsWithTag("Platform");
		foreach (GameObject obj in objs)
		{
			EdgeCollider2D[] platforms = obj.GetComponents<EdgeCollider2D>();

			foreach (EdgeCollider2D platform in platforms)
			{
				// Assumes only two vertices per platform
				// TODO: this assumption is false with the new changes to the clouds
				Vector2 p1 = (Vector2)platform.transform.position + (Vector2)(platform.transform.rotation * (Vector3)platform.points[0]);
				Vector2 p2 = (Vector2)platform.transform.position + (Vector2)(platform.transform.rotation * (Vector3)platform.points[1]);
				Vector2 m = p2 - p1;
				if (p2.x < p1.x || (p2.x == p1.x && p2.y < p1.y))
					m = p1 - p2;
				Vector2 n = new Vector2(m.y, -m.x);
				Vector2 pc = new Vector2(groundCheck.position.x, groundCheck.position.y);
				pc = pc - p1;
				
				bool side = Vector2.Dot (pc, n) > 0.0f; 
				// Comment this for efficiency, draws nice lines to tell you
				// what's is collidable and what's not
				if (side)
					Debug.DrawLine (p1, p2, Color.red, 0.0f, false);
				else
					Debug.DrawLine (p1, p2, Color.blue, 0.0f, false);
				
				foreach (Collider2D col in colliders)
					Physics2D.IgnoreCollision(col, platform, side || IsDead());
			}
		}

		if (gameMaster.GameBounds.IsOutOfBounds(this.gameObject))
		{
			Die();
		}
	}

	private IEnumerator performJump()
	{
		// Update our jumpPhase.
		// Reset our y-velocity to 0 to perform our jump.
		// As long as the space key is held down, apply jump forces specified in the jumpForces vector.
		rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0.0f);
		jumpPhase++;
		if (!grounded && jumpPhase <= 1)
			jumpPhase++;
		jumpForceIndex = 0;
		for (; jumpForceIndex < jumpForces.Count && Input.GetKey (KeyCode.Space); ++jumpForceIndex)
		{
			rigidbody2D.AddForce(new Vector2(0.0f, jumpForces[jumpForceIndex]) * rigidbody2D.mass);
			yield return new WaitForSeconds(0.1f);
		}
	}

	public bool IsDead()
	{
		return dead;
	}

	public void Die()
	{
		dead = true;
	}
}
