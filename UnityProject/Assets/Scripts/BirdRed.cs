using UnityEngine;
using System.Collections;

public class BirdRed : BirdController {
	public ParticleSystem collectEffectPrefab;

	public override BirdType Type
	{
		get { return BirdType.RED; }
	}

	// Use this for initialization
	public override void Start () {
		base.Start ();

		score = 5;
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update ();
	}

	public override void FixedUpdate () {
		base.FixedUpdate ();

		//moveSpeed = Mathf.Clamp(moveSpeed + acceleration, minSpeed, maxSpeed);
		//Debug.Log ("moveSpeed: " + moveSpeed + " _ DeltaTime: " + Time.deltaTime);
		//rigidbody2D.velocity = new Vector2 (-1.0f * moveSpeed, rigidbody2D.velocity.y);
		//rigidbody2D.AddForce(new Vector2(acceleration * rigidbody2D.mass, 0.0f)); 
		rigidbody2D.AddRelativeForce (new Vector2 (initialAcceleration * rigidbody2D.mass, 0.0f));
	}

	public override void OnTriggerEnter2D(Collider2D collider) {
		base.OnTriggerEnter2D (collider);
		if (collider.tag == "Player")
		{
			if (!collected)
			{
				gameMaster.updateScore(score);
				//gameMaster.playerScore++;
				ParticleSystem collectEffectInstance = (ParticleSystem)GameObject.Instantiate(collectEffectPrefab, this.transform.position, this.transform.rotation);
				collectEffectInstance.transform.parent = this.transform;
				renderer.enabled = false;
				collected = true;
				gameMaster.SoundEffects.PlaySoundClip("coin");

				//Debug.Log ("Score: " + gameMaster.playerScore.ToString());
				GameObject.Destroy (this.gameObject, 2.0f);
			}
		}

	}
}
