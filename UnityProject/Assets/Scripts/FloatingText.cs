﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FloatingText : Entity {
	private float life = 1.00f;

	private Vector3 initialPos;
	private GameObject bind = null;

	public Color startColor;
	public Color endColor;

	// Use this for initialization
	public override void Start () {
		base.Start ();
		initialPos = this.transform.position;
	}

	Vector3 determinePosition(float t) {
		const float AMPLITUDE = 50f;
		return initialPos + new Vector3(UnityEngine.Mathf.Cos(t * UnityEngine.Mathf.PI), UnityEngine.Mathf.Sin(t * UnityEngine.Mathf.PI), 0f) * AMPLITUDE;
	}

	Color determineColor(float t) {
		return Color.Lerp(startColor, endColor, t);
	}

	// Update is called once per frame
	public override void Update () {
		base.Update ();
		const float limit = 0.25f;
		life -= Time.deltaTime;
		//this.transform.Translate(0f, SPEED * Time.deltaTime, 0f);
		if (bind != null)
		{
			Vector3 worldOffset = new Vector3(0.1f, 0.1f, 0.0f);
			initialPos = gameMaster.MainCamera.WorldToScreenPoint(bind.transform.position + worldOffset);
		}
		float t = 1.0f - life;
		this.transform.position = determinePosition(t);
		GetComponent<Text>().color = determineColor(t);

		this.transform.localScale = Vector3.one * 1.5f * life;
		if (life < limit)
			GameObject.DestroyImmediate(this.gameObject);
	}

	public void BindToTarget(GameObject target) {
		bind = target;
	}
}