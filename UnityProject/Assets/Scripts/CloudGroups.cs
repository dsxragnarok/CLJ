﻿using UnityEngine;
using System.Collections.Generic;

// Container for a list of game objects in a cloud scene
public class CloudGroups : Entity {
	public List<GameObject> sceneList;

	// Use this for initialization
	public override void Start () {
		base.Start ();
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update ();
		if (transform.childCount <= 0) 
			GameObject.Destroy (this.gameObject);
	}
	
	void FixedUpdate() {

	}
}
