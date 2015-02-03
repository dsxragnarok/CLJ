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
	private float dragSave;

	private Collider2D[] colliders;

	private float stunTimer;
	private int stunLevel;
	private bool dead;

	private float groundForce = -250f;
	private float edgeColliderBoxCheckLen = 3.0f;

	private Animator animator;

	public void StunIt(float length, int level)
	{
		stunTimer = length;
		stunLevel = level;
		rigidbody2D.AddForce (Vector2.right * -5000.0f * level);
		animator.SetBool ("Stunned", true);
	}

	public bool IsStunned
	{
		get { return stunTimer > 0.0f && stunLevel > 0; }
	}

	// Use this for initialization
	public override void Start () {
		base.Start ();

		jumpPhase = 0;
		jumpForceIndex = 0;
		grounded = false;
		xRest = rigidbody2D.position.x;
		dragSave = rigidbody2D.drag;
		colliders = GetComponents<Collider2D>();
		groundRadius = GetComponent<CircleCollider2D>().radius;
		dead = false;

		// flip the animation horizontally to face east
		animator = GetComponent<Animator> ();
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update ();

		if (!gameMaster.isGameStarted) {
			if (Input.GetKeyUp (KeyCode.Space)) {
				gameMaster.closeInstructions();
				gameMaster.isGameStarted = true;
			} else {
				return; 
			}
		}

		stunTimer -= Time.deltaTime;

		// Perform jump if we are on ground or this is our double jump
		//if ((grounded || jumpPhase < 2) && Input.GetKeyDown (KeyCode.Space))
		// Perform jump if we are on ground
		if (!IsDead() && grounded && Input.GetKeyDown (KeyCode.Space))
		{
			gameMaster.SoundEffects.PlaySoundClip("jump");
			StopCoroutine(performJump());
			StartCoroutine(performJump());
			animator.SetBool("Grounded", false);
		}
	}

	void FixedUpdate () {
		if (!gameMaster.isGameStarted) {
			return;
		}
		Collider2D[] lineQualifiers = Physics2D.OverlapAreaAll ((Vector2)transform.position + new Vector2 (-edgeColliderBoxCheckLen, -edgeColliderBoxCheckLen),
		                                                        (Vector2)transform.position + new Vector2 (edgeColliderBoxCheckLen, edgeColliderBoxCheckLen),
		                                                 whatIsGround);
		grounded = Physics2D.OverlapCircle (groundCheck.position, groundRadius * 3f, whatIsGround);
		animator.SetBool ("Grounded", grounded);

		// Reset jump phase if we are grounded so the person can jump again
		if (grounded) 
		{
			animator.SetBool ("Stunned", rigidbody2D.velocity.y <= 0.0f);
			if (jumpForceIndex > 0)
				jumpPhase = 0;
			else
				// Apply a further downward force to stick player onto ground.
				// Helps avoid walking right off ramps and bouncing off.
				rigidbody2D.AddForce (Vector2.up * groundForce);
		}

		Vector2 move = rigidbody2D.velocity;
		// Return to home x-position at a constant velocity
		if (stunTimer <= 0.0f && !IsDead ()) {
			if (rigidbody2D.position.x < xRest - 0.1f)
				move.x = 1f;
			else if (rigidbody2D.position.x > xRest + 0.1f)
				move.x = -1f;
			else
				move.x = 0f;	// zero out velocity at rest so ground force doesn't take its toll
		} 
		else {
		}
		rigidbody2D.velocity = move; 	

		animator.SetFloat ("Speed", Mathf.Abs (move.magnitude));

		// Ignore platform collisions if we are airborne
		//GameObject[] objs = GameObject.FindGameObjectsWithTag("Platform");
		//foreach (GameObject obj in objs)
		foreach (Collider2D obj in lineQualifiers)
		{
			EdgeCollider2D platform = obj.GetComponent<EdgeCollider2D>();

			// Assumes only two vertices per platform
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

		if (gameMaster.GameBounds.IsOutOfBounds(this.gameObject))
		{
			Die();
		}

		//rigidbody2D.drag = rigidbody2D.velocity.y > 0f ? dragSave : 0f;
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
		while  (jumpForceIndex < jumpForces.Count && Input.GetKey (KeyCode.Space))
		{
			rigidbody2D.AddForce(new Vector2(0.0f, jumpForces[jumpForceIndex]) * rigidbody2D.mass);
			++jumpForceIndex;
			yield return new WaitForSeconds(0.1f);
		}
		jumpForceIndex = 0; // Reset to 0 again to flag jump is over
	}

	public bool IsDead()
	{
		return dead;
	}

	public void Die()
	{
		dead = true;
		gameMaster.showGameOver ();
	}
}
