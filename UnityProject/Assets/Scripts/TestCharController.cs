using UnityEngine;
using System.Collections;

public class TestCharController : MonoBehaviour {

	public float maxSpeed = 10f;
	public float jumpForce = 300f;

	bool grounded = false;
	public Transform groundCheck;
	float groundRadius = 0.2f;
	public LayerMask whatIsGround;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (grounded && Input.GetKeyDown (KeyCode.Space)) {
			rigidbody2D.AddForce(new Vector2(0, jumpForce));
		}
	}

	void FixedUpdate () {
		grounded = Physics2D.OverlapCircle (groundCheck.position, groundRadius, whatIsGround);

		float move = Input.GetAxis ("Horizontal");

		rigidbody2D.velocity = new Vector2 (move * maxSpeed, rigidbody2D.velocity.y);
	}
}
