using UnityEngine;
using System.Collections;

public class CheckPoint : Entity {
	Transform player;
	Transform myTransform;

	float moveSpeed = 4.5f;
	bool collected = false;
	public int scoreBonus = 200;

	// Use this for initialization
	public override void Start () {
		base.Start ();
		myTransform = this.transform;
		player = GameObject.FindGameObjectWithTag ("Player").transform;
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update ();
		if (gameMaster.Player.IsDead ())
			moveSpeed = 0f;
		/*
		if (!collected && player.position.x >= myTransform.position.x) {
			ActivateCheckPoint();
		}*/
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

	void ActivateCheckPoint () {
		gameMaster.PlayerData.totalCheckpointsCollected += 1;
		collected = true;
		gameMaster.updateScore (scoreBonus);
		gameMaster.generateFloatingTextAt(gameMaster.Player.transform.position, scoreBonus.ToString());
		GameObject.Destroy (this.gameObject, 2.0f);
	}
}
