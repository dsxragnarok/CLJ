﻿using UnityEngine;
using System.Collections;

// The YELLOW birds are birds that like to knock you out in one blow.
public class BirdBlack : BirdController {
	
	public override BirdType Type
	{
		get { return BirdType.BLACK; }
	}

	// Use this for initialization
	public override void Start () {
		base.Start ();

		score = -10;
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
		unitRigidbody.AddRelativeForce (initialAcceleration *  unitRigidbody.mass);
	}

	public override void OnTriggerEnter2D(Collider2D collider)
	{
		base.OnTriggerEnter2D (collider);
		if (collider.tag == "Player") 
		{
			if (!collected)
			{
				gameMaster.PlayerData.totalBlackBirdsCollected += 1;

				CharController charController = collider.GetComponent<CharController> ();
				//charController.Die ();	
				charController.StunIt (0.5f, 4);	
				collected = true;

				//gameMaster.SoundEffects.PlaySoundClip("thump");
				gameMaster.SoundEffects.PlaySoundClip("thump2");
                gameMaster.generateFloatingTextAt(collider.transform.position, "WHAT THE PECK!?");
                //gameMaster.generateFloatingTextAt(collider.transform.position, "WHAT THE PECK!?", Color.red, Color.yellow);
			}
		}
	}
}
