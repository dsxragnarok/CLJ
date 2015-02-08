using UnityEngine;
using System.Collections.Generic;

public class CloudGroups : Entity {
	public List<GameObject> sceneList;

	protected int gid;

	// Use this for initialization
	public override void Start () {
		base.Start ();
	
		gid = -1;
		int numGroups = -1;
		SpawnOffset[] offsets = GetComponentsInChildren<SpawnOffset>();
		foreach (SpawnOffset off in offsets)
			numGroups = System.Math.Max (numGroups, off.NumGroups);

		if (numGroups > 0)
			gid = UnityEngine.Random.Range (0, numGroups);

		foreach (SpawnOffset off in offsets)
			off.Prune(gid);
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update ();
	}

	void FixedUpdate() {

	}
}
