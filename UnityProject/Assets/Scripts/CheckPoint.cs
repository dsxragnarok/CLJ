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
    bool passed = false;
	private int scoreBonus = 50;

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

		if (!passed && gameMaster.Player.transform.position.x >= myTransform.position.x) {
			passed = true;
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

    void OnTriggerEnter2D (Collider2D collider)
    {
        if (!collected && collider.gameObject.tag == "Player")
        {
            ActivateCheckPoint();
        }
    }

	
	void ActivateCheckPoint () {
        gameMaster.PlayerData.totalCheckpointsCollected += 1;
        int totalCheckPointBonus = scoreBonus * (gameMaster.checkpointsPassed + 1);
        //Debug.Log("checkpoints passed [" + gameMaster.checkpointsPassed + "] | scoreBonus [" + scoreBonus + "] | total = [" + totalCheckPointBonus + "]");
        gameMaster.updateScore(totalCheckPointBonus);
        gameMaster.generateFloatingTextAt(gameMaster.Player.transform.position, totalCheckPointBonus.ToString());
        collected = true;
	}
	

	public override void SetToEntity(Entity entPrefab)
	{
		base.SetToEntity (entPrefab);
		//CheckPoint checkPointPrefab = entPrefab.GetComponent<CheckPoint>();
		this.collected = false;
        this.passed = false;
		//this.scoreBonus = checkPointPrefab.scoreBonus;

		moveSpeed = MOVE_SPEED;
	}
}
