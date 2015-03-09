using UnityEngine;
using System.Collections;

public class FloatingText : MonoBehaviour {
	private float life = 1.00f;

	private Vector3 initialPos;
	// Use this for initialization
	void Start () {
		initialPos = this.transform.position;
	}

	Vector3 determinePosition(float t) {
		const float AMPLITUDE = 50f;
		return initialPos + new Vector3(UnityEngine.Mathf.Cos(t * UnityEngine.Mathf.PI), UnityEngine.Mathf.Sin(t * UnityEngine.Mathf.PI), 0f) * AMPLITUDE;
	}

	// Update is called once per frame
	void Update () {
		const float limit = 0.25f;
		life -= Time.deltaTime;
		//this.transform.Translate(0f, SPEED * Time.deltaTime, 0f);
		this.transform.position = determinePosition(1.0f - life);

		this.transform.localScale = Vector3.one * life;
		if (life < limit)
			GameObject.DestroyImmediate(this.gameObject);
	}
}
