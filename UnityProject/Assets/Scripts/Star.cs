using UnityEngine;
using System.Collections;

// This game object can be collected the player for a bit of points.
public class Star : Entity {
	const float MOVE_SPEED = 4.0f;
	private float moveSpeed = MOVE_SPEED;
	private bool collected = false;
	public ParticleSystem collectEffectPrefab;

	public override void Start () {
		base.Start ();
	}
	
	public override void Update () {
		base.Update ();

		if (gameMaster.Player.IsDead ())
			moveSpeed = 0f;
	}

	void FixedUpdate () {
		if (gameMaster.isGameStarted) {
			Vector3 pos = transform.position;
			pos.x = pos.x - moveSpeed * Time.fixedDeltaTime;
			transform.position = pos;
			
			if (gameMaster.GameBounds.IsOutOfBounds (this.gameObject)) {
				gameMaster.InstancingManager.RecycleObject(this);
			}
		}
	}

	public void OnTriggerEnter2D(Collider2D collider) {
		//base.OnTriggerEnter2D (collider);
		if (collider.tag == "Player")
		{
			if (!collected)
			{
				gameMaster.collectedStars += 1;
				gameMaster.PlayerData.totalStarsCollected += 1;
				int value = gameMaster.scoreMultiplier * (gameMaster.collectedBirds + 1);
				gameMaster.updateScore(value);
				gameMaster.generateFloatingTextAt(gameMaster.Player.transform.position, value.ToString());
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
		Star starPrefab = entPrefab.GetComponent<Star>();
		this.GetComponent<Renderer>().enabled = starPrefab.GetComponent<Renderer>().enabled;
		this.collected = false;

		moveSpeed = MOVE_SPEED;
	}
}
