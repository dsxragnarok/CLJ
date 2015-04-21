using UnityEngine;
using System.Collections;

// At certain locations which if the player runs into it will receive a score bonus.
// Note: this actually only counts check points passed. This should really be moved
// onto the checkpoint itself maybe.
public class CheckPoint : Entity {
    public GoogleAnalyticsV3 googleAnalytics;
	Transform myTransform;

	const float MOVE_SPEED = 4.5f;
	protected float moveSpeed = MOVE_SPEED;
	bool collected = false;
    bool passed = false;
	private int scoreBonus = 50;
	
	Vector3 rotatePoint = Vector3.zero;
	bool animatePickup = false;
	float animateTimer = 0.0f;

	public ParticleSystem glowFireEffectPrefab;

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

		rotatePoint += Vector3.left * (moveSpeed * Time.deltaTime);
		if (animatePickup)
		{
			animateTimer += Time.deltaTime;
			
			float angularMagnitude = UnityEngine.Mathf.Sin (animateTimer * UnityEngine.Mathf.PI);
			float scaleMagnitude = UnityEngine.Mathf.Sin (animateTimer * 2.0f * UnityEngine.Mathf.PI);
			float degrees = -180.0f * angularMagnitude * Time.deltaTime;
			transform.RotateAround(rotatePoint, Vector3.forward, degrees);
			transform.localScale = Vector3.one;
			if (animateTimer < 0.5f)
				transform.localScale = Vector3.one * (1.0f + scaleMagnitude * 0.5f);
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
        gameMaster.collectedCheckpoints += 1;
        gameMaster.PlayerData.totalCheckpointsCollected += 1;
        int totalCheckPointBonus = scoreBonus * (gameMaster.checkpointsPassed + 1);
        //Debug.Log("checkpoints passed [" + gameMaster.checkpointsPassed + "] | scoreBonus [" + scoreBonus + "] | total = [" + totalCheckPointBonus + "]");
        gameMaster.updateScore(totalCheckPointBonus);
        gameMaster.generateFloatingTextAt(gameMaster.Player.transform.position, totalCheckPointBonus.ToString());
		
		ParticleSystem glowFireEffectInstance = (ParticleSystem)GameObject.Instantiate(glowFireEffectPrefab, this.transform.position, this.transform.rotation);
		glowFireEffectInstance.transform.parent = this.transform.parent;
		GameObject.Destroy (glowFireEffectInstance.gameObject, 1.5f);
        collected = true;
		animatePickup = true;
		animateTimer = 0.0f;
		rotatePoint = this.transform.position + Vector3.right * 0.5f;
	}
	

	public override void SetToEntity(Entity entPrefab)
	{
		base.SetToEntity (entPrefab);
		//CheckPoint checkPointPrefab = entPrefab.GetComponent<CheckPoint>();
		this.collected = false;
        this.passed = false;
		//this.scoreBonus = checkPointPrefab.scoreBonus;

		moveSpeed = MOVE_SPEED;
		
		rotatePoint = Vector3.zero;
		animatePickup = false;
		animateTimer = 0.0f;
	}
}
