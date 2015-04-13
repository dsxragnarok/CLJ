using UnityEngine;
using System.Collections.Generic;

// The class containing a list of all the background cloud objects
// The list keeps the clouds in order so that the background clouds
// can reposition themselves based on position of the last cloud
public class BGCloudContainer : Entity {

    public List<GameObject> CloudBackgrounds;

	// Use this for initialization
	public override void Start () {
        base.Start();
	}
	
	// Update is called once per frame
	public override void Update () {
        base.Update();
	}

    void FixedUpdate()
    {
        foreach (GameObject cloud in CloudBackgrounds)
        {
            CloudBG cbg = cloud.GetComponent<CloudBG>();
            cbg.Move(Time.fixedDeltaTime);
        }
    }
}
