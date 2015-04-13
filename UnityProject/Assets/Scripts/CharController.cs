using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This is the Player Avatar and contains all logic involved with how the user interfaces
// with the Player. It contains Jump, Physics, and Avatar behavior.
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
	public LayerMask activePlatforms;
	public LayerMask inactivePlatforms;

	// X rest position
	private float xRest;

	//private Collider2D[] colliders;

	// Status effects related to the player
	private float stunTimer;
	private int stunLevel;
	private bool dead;

	// Force applied to plant player on ground (useful for ramps and prevent bounce)
	private float groundForce = -300f;

	// A check around the player for nearby platforms. This is to prevent checking many
	// platforms whether the player is above or below each one individually.
	private float edgeColliderBoxCheckLen = 3.0f;

	private Animator animator;
	private Rigidbody2D unitRigidbody;

	// Apply a stun timer. Based on the level (i.e. strength),
	// Apply a knockback force to the character
	public void StunIt(float length, int level)
	{
		stunTimer = length;
		stunLevel = level;
		unitRigidbody.AddForce (Vector2.right * -3000.0f * level);
		if (stunLevel >= 4)
			unitRigidbody.AddForce (Vector2.up * 1000.0f * level);
		//jumpPhase = 2;
	}

	// Returns whether the player is stunned,
	public bool IsStunned {
		get { return stunTimer > 0.0f || stunLevel > 0; }
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
		xRest = this.transform.position.x;
		//colliders = GetComponents<Collider2D>();
		groundRadius = GetComponent<CircleCollider2D>().radius;
		dead = false;

		// flip the animation horizontally to face east
		animator = GetComponent<Animator> ();
		//Vector3 theScale = transform.localScale;
		//theScale.x *= -1;
		//transform.localScale = theScale;		
		unitRigidbody = GetComponent<Rigidbody2D> ();
		
		Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("InactivePlatforms"));
		Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("NotPlatforms"));
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update ();

#if UNITY_STANDALONE || UNITY_WEBPLAYER

		if (!gameMaster.isGameStarted) {
			if (Input.GetKeyUp (KeyCode.Space) || Input.GetMouseButtonUp(0)) {
				gameMaster.closeInstructions();
				gameMaster.isGameStarted = true;
			} else {
				return; 
			}
		}
#elif UNITY_ANDROID || UNITY_IOS
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

		// Perform jump if we are on ground
#if UNITY_STANDALONE || UNITY_WEBPLAYER
		if (!IsDead() && (grounded || jumpPhase < 2) && 
		    (Input.GetKeyDown (KeyCode.Space) || Input.GetMouseButtonDown(0))
		 )
		{
            if (Input.GetMouseButtonDown(0) && gameMaster.isHitPauseButton(Input.mousePosition))
                return;
            /*{
                Vector3 mp = Input.mousePosition;
                Vector3 pb = gameMaster.pauseButton.transform.position;
                
                float halfw = gameMaster.pauseButton.GetComponent<UnityEngine.UI.Image>().rectTransform.rect.width / 2;
                float halfh = gameMaster.pauseButton.GetComponent<UnityEngine.UI.Image>().rectTransform.rect.height / 2;

                float left = pb.x - halfw;
                float right = pb.x + halfw;
                float top = pb.y - halfh;
                float bottom = pb.y + halfh;

                if (mp.x >= left && mp.x <= right && mp.y >= top && mp.y <= bottom)
                    return;
            }*/

			gameMaster.SoundEffects.PlaySoundClip("jump");
			//StopCoroutine(performJump());
			//StartCoroutine(performJump());
			initJump();
		}
#elif UNITY_ANDROID || UNITY_IOS
		if (Input.touchCount > 0) {
			Touch touch = Input.touches[0];

			//Debug.Log(touch.phase);
			/*	FOR FLAPPY BIRD STYLE CONTROL
			if (!IsDead() && touch.phase == TouchPhase.Began)
			{
				gameMaster.SoundEffects.PlaySoundClip("jump");
				unitRigidbody.velocity = new Vector2(unitRigidbody.velocity.x, 0.0f);
				unitRigidbody.AddForce(new Vector2(0.0f, jumpForces[0]) * unitRigidbody.mass);
				animator.SetBool("Grounded", false);
			}
			*/

			if (!IsDead ())
			{
				if ((grounded || jumpPhase < 2) && touch.phase == TouchPhase.Began)
				{
                    if (gameMaster.isHitPauseButton(touch.position))
                        return;

					gameMaster.SoundEffects.PlaySoundClip("jump");
					initJump();
				}
			}
		}
#endif
	}

	void FixedUpdate () {
		if (!gameMaster.isGameStarted) {
			return;
		}

		updateJump();

		setActiveInactivePlatform(activePlatforms);
		setActiveInactivePlatform(inactivePlatforms);

		grounded = Physics2D.OverlapCircle (groundCheck.position, 1.5f * groundRadius, activePlatforms) && jumpForceIndex <= 0;
		animator.SetBool ("Grounded", grounded);

		// Reset jump phase if we are grounded so the person can jump again
		if (grounded) 
		{
			stunLevel = 0;
			jumpPhase = 0;
			// Apply a further downward force to stick player onto ground.
			// Helps avoid walking right off ramps and bouncing off.
			unitRigidbody.AddForce (Vector2.up * groundForce);
		}
		animator.SetBool ("Wailing", jumpPhase >= 2);
		
		Vector2 move = unitRigidbody.velocity;
		// Return to home x-position at a constant velocity
		if (!IsStunned && !IsDead ()) {
			if (unitRigidbody.position.x < xRest - 0.1f)
				move.x = maxSpeed;
			else if (unitRigidbody.position.x > xRest + 0.1f)
				move.x = -maxSpeed;
			else
				move.x = 0f;	// zero out velocity at rest so ground force doesn't take its toll
		} 
		else {
			if (move.x > 0f)
				move.x = 0f;
		}
		unitRigidbody.velocity = move; 	

		animator.SetFloat ("Speed", Mathf.Abs (move.magnitude));

		if (!dead && gameMaster.GameBounds.IsOutOfBounds(this.gameObject))
		{
			Die();
		}
	}

	// Sets which platforms should be active and inactive towards the player. It takes only the important ones which are
	// near the player. A platform is considered inactive if the player comes from the bottom (bottom-left) side. This
	// allows it so the player can jump up onto platforms.
	private void setActiveInactivePlatform(LayerMask whatIsGround)
	{		
		// Check all nearby lines
		Collider2D[] lineQualifiers = Physics2D.OverlapAreaAll ((Vector2)transform.position + new Vector2 (-edgeColliderBoxCheckLen, -edgeColliderBoxCheckLen),
		             			                                (Vector2)transform.position + new Vector2 (edgeColliderBoxCheckLen, edgeColliderBoxCheckLen),
		                                           				whatIsGround);
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
			//if (side)
			//	Debug.DrawLine (p1, p2, Color.red, 0.0f, false);
			//else
			//	Debug.DrawLine (p1, p2, Color.blue, 0.0f, false);

			platform.gameObject.layer = side || IsDead() ? LayerMask.NameToLayer("InactivePlatforms") : LayerMask.NameToLayer("ActivePlatforms");
			//foreach (Collider2D col in colliders) 
			//	Physics2D.IgnoreCollision(col, platform, side || IsDead());
		}


	}

	// Initialization of the coroutine part of performJump()
	private void initJump()
	{
		// Reset our y-velocity to 0 to perform our jump.
		unitRigidbody.velocity = new Vector2(unitRigidbody.velocity.x, 0.0f);
		jumpPhase++;
		if (!grounded && jumpPhase <= 1)
			jumpPhase++;
		jumpForceIndex = 0;

		// Initial Jump
		unitRigidbody.AddForce(new Vector2(0.0f, jumpForces[jumpForceIndex]) * unitRigidbody.mass);
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
		jumpTimer = jumpTimer + Time.fixedDeltaTime;
		if (jumpTimer >= 0.1f)
		{
			jumpTimer = jumpTimer - 0.1f;

			++jumpForceIndex;

#if UNITY_ANDROID || UNITY_IOS
			if (Input.touchCount > 0) {
				Touch touch = Input.GetTouch(0);
				if (jumpForceIndex < jumpForces.Count && (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved))
				{
					unitRigidbody.AddForce(new Vector2(0.0f, jumpForces[jumpForceIndex]) * unitRigidbody.mass);
				}
				else
				{
					jumpForceIndex = 0;
				}
			}
			else
			{
				jumpForceIndex = 0;
			}
#elif UNITY_WEBPLAYER || UNITY_STANDALONE

			if (jumpForceIndex < jumpForces.Count && (Input.GetKey (KeyCode.Space) || Input.GetMouseButton (0)))
			{
				unitRigidbody.AddForce(new Vector2(0.0f, jumpForces[jumpForceIndex]) * unitRigidbody.mass);
			}
			else
			{
				jumpForceIndex = 0;
			}
#endif
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
		unitRigidbody.velocity = new Vector2(unitRigidbody.velocity.x, 0.0f);
		jumpPhase++;
		if (!grounded && jumpPhase <= 1)
			jumpPhase++;
		jumpForceIndex = 0;

		while  (jumpForceIndex < jumpForces.Count && Input.GetKey (KeyCode.Space))
		{
			unitRigidbody.AddForce(new Vector2(0.0f, jumpForces[jumpForceIndex]) * unitRigidbody.mass);
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
        gameMaster.showToggleGameOver(); // for testing purposes -- delete or comment out for publish
	}
}
