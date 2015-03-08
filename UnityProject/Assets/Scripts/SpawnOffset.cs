using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnOffset : MonoBehaviour {

	// Spawn offset criteria
	[System.Serializable]
	public class CircleOffset
	{
		public Vector2 offset = Vector2.zero;	// Exact offset
		public float radius = 0.1f;				// Radius around offset
		public int group = -1;					// Pathing group, -1 means no association
	}
	public List<CircleOffset> offsets = new List<CircleOffset>();

	void Start ()
	{
	}

	void Update ()
	{
	}

	// Display the spawn object location for readability
	void OnDrawGizmos()
	{
		/*
		foreach (CircleOffset off in offsets) {
			Color col = Color.white;
			// Color the offset based on its group, otherwise assign default
			switch (off.group)
			{
			case 0:
				col = Color.green;
				break;
			case 1:
				col = Color.red;
				break;
			case 2:
				col = Color.blue;
				break;
			case 3:
				col = Color.green;
				break;
			case 4:
				col = Color.magenta;
				break;
			case 5:
				col = Color.cyan;
				break;
			default:
				col = Color.yellow;
				break;
			}
			Gizmos.color = col;
			Gizmos.DrawWireSphere(transform.position + new Vector3(off.offset.x, off.offset.y, 0f), off.radius);
		}
		*/
	}

	public bool hasOffset()
	{
		return offsets.Count > 0;
	}

	// Retrieve a random offset, and within that offset a random location in a defined circle
	public Vector2 getRandomOffset() {
		if (hasOffset())
			return Vector2.zero; 
		CircleOffset off = offsets [UnityEngine.Random.Range (0, offsets.Count)];
		return off.offset + UnityEngine.Random.insideUnitCircle * off.radius;
	}

	public int NumGroups
	{
		get {
			// Find number of groups
			int numGroups = 0;
			foreach (CircleOffset off in offsets) {
				numGroups = System.Math.Max(numGroups, off.group+1);
			}
			return numGroups;
		}
	}

	public void Prune(int gid)
	{	
		/*
		// Choose a *Safe* path by group
		int gid = -1;
		if (numGroups > 0)
			gid = UnityEngine.Random.Range (0, NumGroups);
		*/
		
		// Remove all groups associated to the safe path so
		// spawns do not appear there
		List<CircleOffset> pruned = new List<CircleOffset> ();
		foreach (CircleOffset off in offsets) {
			if (off.group != gid || off.group == -1)
				pruned.Add (off);
		}
		offsets = pruned;
	}
}
