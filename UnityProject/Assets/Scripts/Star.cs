using UnityEngine;
using System.Collections;

public class Star : Entity {
	private float moveSpeed = 4.0f;
	private bool collected = false;
	public int score = 5;
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
				GameObject.Destroy (this.gameObject);
			}
		}
	}

	public void OnTriggerEnter2D(Collider2D collider) {
		//base.OnTriggerEnter2D (collider);
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
