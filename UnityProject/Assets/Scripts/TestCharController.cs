using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestCharController : MonoBehaviour {

	public float maxSpeed = 1f;

	// Jump Variables
	private int jumpPhase;			// 0=no jump 1=1st jump 2=2nd jump
	private int jumpForceIndex;		
	public List<float> jumpForces;

	private bool grounded;
	public Transform groundCheck;
	private float groundRadius = 0.1f;
	public LayerMask whatIsGround;

	// X rest position
	private float xRest;

	// Use this for initialization
	void Start () {
		jumpPhase = 0;
		jumpForceIndex = 0;
		grounded = false;
		xRest = rigidbody2D.position.x;
	}
	
	// Update is called once per frame
	void Update () {
		// Perform jump if we are on ground or this is our double jump
		if ((grounded || jumpPhase <= 1) && Input.GetKeyDown (KeyCode.Space))
		{
			StopCoroutine(performJump());
			StartCoroutine(performJump());
		}
	}

	void FixedUpdate () {
		grounded = Physics2D.OverlapCircle (groundCheck.position, groundRadius, whatIsGround);
		// Reset jump phase if we are grounded so the person can jump again
		if (grounded && jumpForceIndex > 0)
			jumpPhase = 0;

		float move = 0.0f;
		// Return to home x-position at a constant velocity
		if (rigidbody2D.position.x < xRest - 0.1f)
			move = maxSpeed;
		if (rigidbody2D.position.x > xRest + 0.1f)
			move = -maxSpeed;
		rigidbody2D.velocity = new Vector2(move, rigidbody2D.velocity.y); 	

		// Ignore platform collisions if we are airborne
		GameObject[] objs = GameObject.FindGameObjectsWithTag("Platform");
		foreach (GameObject obj in objs)
		{
			EdgeCollider2D platform = obj.GetComponent<EdgeCollider2D>();
			// Assumes only two vertices per platform
			Vector2 p1 = (Vector2)platform.transform.position + platform.points[0];
			Vector2 p2 = (Vector2)platform.transform.position + platform.points[1];
			Vector2 m = p2 - p1;
			Vector2 n = new Vector2(-m.y, m.x);
			if (Vector2.Dot (Vector2.up, n) < 0.0f)
				n = new Vector2(m.y, -m.x);
			Vector2 pc = (Vector2)groundCheck.position - p1;
			if (Vector2.Dot (pc, n) < 0.0f)
				Debug.DrawLine (p1, p2, Color.red, 0.0f, false);
			else
				Debug.DrawLine (p1, p2, Color.blue, 0.0f, false);
			Debug.Log (p1.ToString () + " " + p2.ToString());
			Physics2D.IgnoreCollision(this.collider2D, platform, Vector2.Dot (pc, n) < 0.0f);
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
}
