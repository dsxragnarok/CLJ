using UnityEngine;
using System.Collections;

public class FloatingText : MonoBehaviour {
	private float life = 1.00f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		const float SPEED = 50f;
		const float limit = 0.25f;
		life -= Time.deltaTime;
		this.transform.Translate(0f, SPEED * Time.deltaTime, 0f);
		this.transform.localScale = Vector3.one * life;
		if (life < limit)
			GameObject.DestroyImmediate(this.gameObject);
	}
}
