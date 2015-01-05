using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestCharController : MonoBehaviour {

	public float maxSpeed = 3f;

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

		// Return to home x-position, needs rework
		if (rigidbody2D.position.x < xRest - 0.1f)
			rigidbody2D.AddForce (new Vector2(maxSpeed, 0.0f) * rigidbody2D.mass);
		if (rigidbody2D.position.x > xRest + 0.1f)
			rigidbody2D.AddForce (new Vector2(-maxSpeed, 0.0f) * rigidbody2D.mass);

		// Ignore platform collisions if we are airborne
		Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Platforms"), !grounded);
	}

	private IEnumerator performJump()
	{
		// Update our jumpPhase.
		// Reset our y-velocity to 0 to perform our jump.
		// As long as the space key is held down, apply jump forces specified in the jumpForces vector.
		rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0.0f);
		jumpPhase++;
		jumpForceIndex = 0;
		for (; jumpForceIndex < jumpForces.Count && Input.GetKey (KeyCode.Space); ++jumpForceIndex)
		{
			rigidbody2D.AddForce(new Vector2(0.0f, jumpForces[jumpForceIndex]) * rigidbody2D.mass);
			yield return new WaitForSeconds(0.1f);
		}
	}
}
