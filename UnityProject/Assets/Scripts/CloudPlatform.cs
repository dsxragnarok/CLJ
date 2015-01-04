using UnityEngine;
using System.Collections;

public class CloudPlatform : MonoBehaviour {
	public float moveSpeed = 3.0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {
		Vector3 pos = transform.position;
		pos.x = pos.x - moveSpeed * Time.fixedDeltaTime;
		transform.position = pos;
	}
}
