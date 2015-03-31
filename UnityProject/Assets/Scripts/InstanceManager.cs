using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Efficiently caches game objects for re-use.
// It uses the Entity base object which contains the ID as the key
public class InstanceManager : MonoBehaviour {

	// A decent way to hash into all available game objects based on their entity ID
	private Dictionary<int, Queue<Entity>> objectCache;

	public void Awake () {
		GameObject.DontDestroyOnLoad (this.gameObject);

		objectCache = new Dictionary<int, Queue<Entity>>();
	}

	// Use this for initialization
	public void Start () {

		// Grab all active and inactive entities and store them in the cache
		Entity[] initialStorage = GetComponentsInChildren<Entity>(true);
		foreach (Entity ent in initialStorage)
		{
			if (ent.gameObject != this.gameObject)
				RecycleObject(ent);
		}
	}
	
	// Update is called once per frame
	public void Update () {
	}

	// Retrieves an object of the same type, if one does not exist,
	// we instantiate one.
	public Entity RetrieveObject(Entity ent)
	{
		Entity ret = null;
		int key = ent.entityID;
		if (key < 0) return ret;
		if (objectCache.ContainsKey(key))
		{
			Queue<Entity> objQueue = objectCache[key];
			if (objQueue.Count > 0)
			{
				// Object exists, extract it from the queue
				ret = objQueue.Dequeue();
				ret.transform.position = ent.transform.position;
				ret.transform.rotation = ent.transform.rotation;
				ret.gameObject.SetActive(true);
			}
			else
			{
				// Does not exist, instantiate the object
				ret = GameObject.Instantiate<Entity>(ent);
				ret.transform.position = ent.transform.position;
				ret.transform.rotation = ent.transform.rotation;
			}
		}
		else
		{
			// List for object type does not exist, instantiate the object
			ret = GameObject.Instantiate<Entity>(ent);
			ret.transform.position = ent.transform.position;
			ret.transform.rotation = ent.transform.rotation;
		}
		return ret;
	}

	// Objects which wish to give themselves up for re-use can call this function
	public void RecycleObject(Entity ent)
	{
		int key = ent.entityID;
		if (key < 0) return;
		if (objectCache.ContainsKey(key))
		{
			// List for object type does exist, add it to this list
			Queue<Entity> objQueue = objectCache[key];
			objQueue.Enqueue (ent);
			ent.gameObject.SetActive(false);
			ent.transform.parent = this.transform;
		}
		else
		{
			// List for object type does not exist, create a list and add it to this list
			Queue<Entity> objQueue = new Queue<Entity>();
			objQueue.Enqueue (ent);
			ent.gameObject.SetActive(false);
			ent.transform.parent = this.transform;
			objectCache.Add (key, objQueue);
		}
	}

	public void UpdateCachedObjectLinks()
	{
		// Grab all active and inactive entities and store them in the cache
		Entity[] initialStorage = GetComponentsInChildren<Entity>(true);
		foreach (Entity ent in initialStorage)
		{
			if (ent.gameObject != this.gameObject)
				ent.Link();
		}
	}
}
