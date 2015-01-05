using UnityEngine;
using System.Collections;

public class BirdController : MonoBehaviour {

	public float minSpeed;
	public float maxSpeed;

	float moveSpeed;	// determined by a random number between minSpeed and maxSpeed

	// Use this for initialization
	void Start () {
		moveSpeed = Random.Range (minSpeed, maxSpeed);
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pos = transform.position;
		pos.x = pos.x - moveSpeed * Time.fixedDeltaTime;
		transform.position = pos;
	}
}
