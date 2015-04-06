using UnityEngine;
using System.Collections;

// At certain locations which if the player runs into it will receive a score bonus.
// Note: this actually only counts check points passed. This should really be moved
// onto the checkpoint itself maybe.
public class CheckPoint : Entity {
	Transform myTransform;

	const float MOVE_SPEED = 4.5f;
	protected float moveSpeed = MOVE_SPEED;
	bool collected = false;
	public int scoreBonus = 200;

	// Use this for initialization
	public override void Start () {
		base.Start ();
		myTransform = this.transform;
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update ();
		if (!gameMaster.isGameStarted)
			return;

		if (gameMaster.Player.IsDead ())
			moveSpeed = 0f;

		if (!collected && gameMaster.Player.transform.position.x >= myTransform.position.x) {
			collected = true;
			gameMaster.checkpointsPassed += 1;
		}
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
				gameMaster.InstancingManager.RecycleObject (this);
			}
		}
	}

	/*
	void ActivateCheckPoint () {
		gameMaster.PlayerData.totalCheckpointsCollected += 1;
		collected = true;
		gameMaster.updateScore (scoreBonus);
		gameMaster.generateFloatingTextAt(gameMaster.Player.transform.position, scoreBonus.ToString());
	}
	*/

	public override void SetToEntity(Entity entPrefab)
	{
		base.SetToEntity (entPrefab);
		CheckPoint checkPointPrefab = entPrefab.GetComponent<CheckPoint>();
		this.collected = false;
		this.scoreBonus = checkPointPrefab.scoreBonus;

		moveSpeed = MOVE_SPEED;
	}
}
