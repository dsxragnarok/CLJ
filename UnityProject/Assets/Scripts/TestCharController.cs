using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestCharController : MonoBehaviour {

	public float maxSpeed = 10f;

	private bool jumping;
	public List<float> jumpForces;
	public float jumpMin = 0.25f;
	public float jumpMax = 0.50f;
	private float jumpTimer;

	private bool grounded;
	public Transform groundCheck;
	private float groundRadius = 0.1f;
	public LayerMask whatIsGround;
	
	// Use this for initialization
	void Start () {
		jumpTimer = jumpMax;
		grounded = false;
	
	}
	
	// Update is called once per frame
	void Update () {
		if (grounded && !jumping && Input.GetKeyDown (KeyCode.Space))
		{
			StartCoroutine(performJump());
		}
	}

	void FixedUpdate () {
		grounded = Physics2D.OverlapCircle (groundCheck.position, groundRadius, whatIsGround);

		Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Platforms"), !grounded);
	}

	private IEnumerator performJump()
	{
		jumping = true;
		int jumpPhase = 0;
		for (; jumpPhase < jumpForces.Count && Input.GetKey (KeyCode.Space); ++jumpPhase)
		{
			rigidbody2D.AddForce(new Vector2(0.0f, jumpForces[jumpPhase]));
			yield return new WaitForSeconds(0.1f);
		}
		jumping = false;
	}
}
