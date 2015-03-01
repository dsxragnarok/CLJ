using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharController : Entity {

	public float maxSpeed;

	// Jump Variables
	private int jumpPhase;			// 0=no jump 1=1st jump 2=2nd jump
	private int jumpForceIndex;		
	private float jumpTimer;
	public List<float> jumpForces;

	// Ground usage variables
	private bool grounded;
	public Transform groundCheck;
	private float groundRadius;
	public LayerMask whatIsGround;

	// X rest position
	private float xRest;

	private Collider2D[] colliders;

	// Status effects related to the player
	private float stunTimer;
	private int stunLevel;
	private bool dead;

	// Force applied to plant player on ground (useful for ramps and prevent bounce)
	private float groundForce = -250f;

	// A check around the player for nearby platforms. This is to prevent checking many
	// platforms whether the player is above or below each one individually.
	private float edgeColliderBoxCheckLen = 3.0f;

	private Animator animator;

	private float touchElapsedTime = 0.0f;

	// Apply a stun timer. Based on the level (i.e. strength),
	// Apply a knockback force to the character
	public void StunIt(float length, int level)
	{
		stunTimer = length;
		stunLevel = level;
		rigidbody2D.AddForce (Vector2.right * -5000.0f * level);
		if (stunLevel >= 4)
			rigidbody2D.AddForce (Vector2.up * 2000.0f * level);
		animator.SetBool ("Stunned", true);
	}

	// Returns whether the player is stunned,
	public bool IsStunned {
		get { return stunTimer > 0.0f && stunLevel > 0; }
	}

	// Returns home position: the x location the player drifts to
	public float XRest {
		get { return xRest; }
	}

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

		// flip the animation horizontally to face east
		animator = GetComponent<Animator> ();
		//Vector3 theScale = transform.localScale;
		//theScale.x *= -1;
		//transform.localScale = theScale;

#if UNITY_ANDROID
		// set up different jump forces for touch controls
		/*for (var i = 0; i < jumpForces.Count; i += 1)
		{
			jumpForces[i] *= 0.75f;
		}*/
#endif
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update ();

#if UNITY_STANDALONE || UNITY_WEBPLAYER

		if (!gameMaster.isGameStarted) {
			if (Input.GetKeyUp (KeyCode.Space)) {
				gameMaster.closeInstructions();
				gameMaster.isGameStarted = true;
			} else {
				return; 
			}
		}
#elif UNITY_ANDROID
		if (Input.touchCount > 0) {
			Touch touch = Input.touches[0];
			if (!gameMaster.isGameStarted) {
				if (touch.phase == TouchPhase.Began) {
					gameMaster.closeInstructions();
					gameMaster.isGameStarted = true;
				} else {
					return; 
				}
			}
		}
#endif
		stunTimer -= Time.deltaTime;

		// Perform jump if we are on ground or this is our double jump
		//if ((grounded || jumpPhase < 2) && Input.GetKeyDown (KeyCode.Space))
		// Perform jump if we are on ground
#if UNITY_STANDALONE || UNITY_WEBPLAYER
		if (!IsDead() && grounded && Input.GetKeyDown (KeyCode.Space))
		{
			gameMaster.SoundEffects.PlaySoundClip("jump");
			//StopCoroutine(performJump());
			//StartCoroutine(performJump());
			initJump();
			animator.SetBool("Grounded", false);
		}
		updateJump();
#elif UNITY_ANDROID
		if (Input.touchCount > 0) {
			Touch touch = Input.touches[0];

			//Debug.Log(touch.phase);
			/*	FOR FLAPPY BIRD STYLE CONTROL
			if (!IsDead() && touch.phase == TouchPhase.Began)
			{
				gameMaster.SoundEffects.PlaySoundClip("jump");
				rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0.0f);
				rigidbody2D.AddForce(new Vector2(0.0f, jumpForces[0]) * rigidbody2D.mass);
				animator.SetBool("Grounded", false);
			}
			*/

			// NOTE: this seems to work for touch -- however, there's an issue
			// where the player has infinite jump in midair.
			if (!IsDead ())
			{
				if (grounded && touch.phase == TouchPhase.Began)
				{
					gameMaster.SoundEffects.PlaySoundClip("jump");
					rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0.0f);
					jumpForceIndex = 0;
					touchElapsedTime = 0.0f;
					animator.SetBool("Grounded", false);
					rigidbody2D.AddForce(new Vector2(0.0f, jumpForces[jumpForceIndex] * rigidbody2D.mass));
					jumpForceIndex++;
				}
				else if (touch.phase == TouchPhase.Stationary)
				{
					touchElapsedTime += Time.deltaTime;
					if (touchElapsedTime >= 0.1f && jumpForceIndex < jumpForces.Count)
					{
						rigidbody2D.AddForce(new Vector2(0.0f, jumpForces[jumpForceIndex] * rigidbody2D.mass));
						touchElapsedTime = 0.0f;
						jumpForceIndex++;
					}

				}
				else if (touch.phase == TouchPhase.Ended)
				{
					touchElapsedTime = 0.0f;
					jumpForceIndex = 0;
				}
			}
		}
#endif
	}

	void FixedUpdate () {
		if (!gameMaster.isGameStarted) {
			return;
		}

		// Check all nearby lines
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
				move.x = maxSpeed;
			else if (rigidbody2D.position.x > xRest + 0.1f)
				move.x = -maxSpeed;
			else
				move.x = 0f;	// zero out velocity at rest so ground force doesn't take its toll
		} 
		else {
			if (move.x > 0f)
				move.x = 0f;
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

			Vector2 delta = p2 - p1;
			bool side = false;
			if (System.Math.Abs (delta.y) >= 4 * System.Math.Abs (delta.x)) 
				side = true;	// Near vertical lines are auto pathable
			else
			{
				Vector2 m = p2 - p1;
				if (p2.x < p1.x || (p2.x == p1.x && p2.y < p1.y))
					m = p1 - p2;
				Vector2 n = new Vector2(m.y, -m.x);
				Vector2 pc = new Vector2(groundCheck.position.x, groundCheck.position.y);
				pc = pc - p1;
					
				side = Vector2.Dot (pc, n) > 0.0f; 
				// Comment this for efficiency, draws nice lines to tell you
				// what's is collidable and what's not
				if (side)
					Debug.DrawLine (p1, p2, Color.red, 0.0f, false);
				else
					Debug.DrawLine (p1, p2, Color.blue, 0.0f, false);
					
			}
			foreach (Collider2D col in colliders)
				Physics2D.IgnoreCollision(col, platform, side || IsDead());
		}

		if (gameMaster.GameBounds.IsOutOfBounds(this.gameObject))
		{
			Die();
		}
	}
	
	// Initialization of the coroutine part of performJump()
	private void initJump()
	{
		// Reset our y-velocity to 0 to perform our jump.
		rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0.0f);
		jumpPhase++;
		if (!grounded && jumpPhase <= 1)
			jumpPhase++;
		jumpForceIndex = 0;

		// Initial Jump
		rigidbody2D.AddForce(new Vector2(0.0f, jumpForces[jumpForceIndex]) * rigidbody2D.mass);
		++jumpForceIndex;

		jumpTimer = 0.0f;
	}

	// While loop of the coroutine part of performJump()
	private void updateJump()
	{
		// If we are not jumping, do not continue
		if (jumpForceIndex <= 0)
			return;
		
		// Update our jumpPhase.
		// As long as the space key is held down, apply jump forces specified in the jumpForces vector.
		jumpTimer = jumpTimer + Time.deltaTime;
		if (jumpTimer >= 0.1f)
		{
			jumpTimer = jumpTimer - 0.1f;

			++jumpForceIndex;
			if (jumpForceIndex < jumpForces.Count && Input.GetKey (KeyCode.Space))
			{
				Debug.Log (jumpForceIndex);
				rigidbody2D.AddForce(new Vector2(0.0f, jumpForces[jumpForceIndex]) * rigidbody2D.mass);
			}
			else
			{
				jumpForceIndex = 0;
			}
		}
	}

	// Performs a jump first resetting the player's y-velocity to zero.
	// It then goes through multiple phases as long as the Jump key is held down.
	// Starting at phase 0, it applies a force specified at phase 0,
	// It then continues to phase 1, 2, 3, ect. until there are no more phases.
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
		animator.SetBool ("Die", true);
		gameMaster.showGameOver ();
	}
}
