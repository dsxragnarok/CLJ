using UnityEngine;
using System.Collections.Generic;

// Slowly scrolls the background cloud objects
public class CloudBG : Entity {
    private float moveSpeed = 1.0f;
    private float width;

    BGCloudContainer parentContainer;

	// Use this for initialization
	public override void Start () {
        base.Start();
        width = GetComponent<Renderer>().bounds.size.x;
        parentContainer = GetComponentInParent<BGCloudContainer>();
	}
	
	// Update is called once per frame
	public override void Update () {
        base.Update();
        
        if (transform.position.x + width <= gameMaster.GameBounds.Left)
        {
            GameObject lastCloud = parentContainer.CloudBackgrounds[parentContainer.CloudBackgrounds.Count - 1];
            transform.position = new Vector3(lastCloud.transform.position.x + width, transform.position.y, transform.position.z);

            parentContainer.CloudBackgrounds.Remove(this.gameObject);
            parentContainer.CloudBackgrounds.Add(this.gameObject);
        }
         
	}

    public void Move (float deltaTime)
    {
        if (gameMaster.isGameStarted)
        {
            Vector3 pos = transform.position;
            pos.x = pos.x - moveSpeed * deltaTime;
            transform.position = pos;
        }
    }
}
