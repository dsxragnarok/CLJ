using UnityEngine;
using System.Collections;

// The red birds are special birds that evaporate into thin air when they touch you.
public class BirdRed : BirdController {
	public ParticleSystem collectEffectPrefab;

	public override BirdType Type
	{
		get { return BirdType.RED; }
	}

	// Use this for initialization
	public override void Start () {
		base.Start ();
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update ();
	}

	public override void FixedUpdate () {
		base.FixedUpdate ();

		//moveSpeed = Mathf.Clamp(moveSpeed + acceleration, minSpeed, maxSpeed);
		//Debug.Log ("moveSpeed: " + moveSpeed + " _ DeltaTime: " + Time.deltaTime);
		//unitRigidbody.velocity = new Vector2 (-1.0f * moveSpeed, unitRigidbody.velocity.y);
		//unitRigidbody.AddForce(new Vector2(acceleration * unitRigidbody.mass, 0.0f)); 
		unitRigidbody.AddRelativeForce (initialAcceleration * unitRigidbody.mass);
	}

	public override void OnTriggerEnter2D(Collider2D collider) {
		base.OnTriggerEnter2D (collider);
		if (collider.tag == "Player")
		{
			if (!collected)
			{
				gameMaster.collectedBalloons += 1;
				gameMaster.PlayerData.totalRedBirdsCollected += 1;

				int value = gameMaster.scoreMultiplier * gameMaster.collectedStars;
				gameMaster.updateScore(value);
				gameMaster.generateFloatingTextAt(gameMaster.Player.transform.position, value.ToString());
				//gameMaster.playerScore++;
				ParticleSystem collectEffectInstance = (ParticleSystem)GameObject.Instantiate(collectEffectPrefab, this.transform.position, this.transform.rotation);
				collectEffectInstance.transform.parent = this.transform;
				GameObject.Destroy (collectEffectInstance.gameObject, 2.0f);
				GetComponent<Renderer>().enabled = false;
				collected = true;
				gameMaster.SoundEffects.PlaySoundClip("coin", 0.5f);

				//Debug.Log ("Score: " + gameMaster.playerScore.ToString());
			}
		}

	}

	public override void SetToEntity(Entity entPrefab)
	{
		base.SetToEntity (entPrefab);
		BirdRed birdRedPrefab = entPrefab.GetComponent<BirdRed>();
		this.GetComponent<Renderer>().enabled = birdRedPrefab.GetComponent<Renderer>().enabled;
	}
}
