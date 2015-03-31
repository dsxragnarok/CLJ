using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;

// This menu option allows easy assignment to all entities in the scene to be set to the same ID value.
// This makes it easier than going through every object in the scene prefab and setting it manually.
// You just need to have all entities to be in the unity scene and this script can do the rest provided 
// with the object tag, cloud name, and ID you want to set for those cloud names.
public class EditorSetEntityID : EditorWindow
{	
	string tagName;
	string entityName;
	int id;
	
	// Add menu item named "EditorSetEntityID" to the Window menu
	[MenuItem("Window/EditorSetEntityID")]
	public static void ShowWindow()
	{
		//Show existing window instance. If one doesn't exist, make one.
		EditorWindow.GetWindow(typeof(EditorSetEntityID));
	}
	
	void OnGUI()
	{
		// Title
		GUILayout.Label ("Editor Set Entity ID", EditorStyles.boldLabel);

		// Name of tag for cloud platforms
		tagName = EditorGUILayout.TextField("Tag", tagName);
		// Name of entity we want to assign the ID to
		entityName = EditorGUILayout.TextField("Name", entityName);
		// Assign what ID to set for the clouds
		id = EditorGUILayout.IntField("ID", id);
		
		// Button to run operation
		if (GUILayout.Button("Set ID"))
		{
			if (EditorUtility.DisplayDialog("Set " + entityName + " to store ID " + id.ToString(),
			                                "All entities found with the tag and name will be modified.",
			                                "Continue", "Cancel"))
			{
				Debug.Log ("Setting ID to Entities in Scene");
				
				GameObject[] objs = GameObject.FindGameObjectsWithTag(tagName);
				
				// Chance for each matching Entity
				foreach (GameObject obj in objs)
				{
					Entity entity = obj.GetComponent<Entity>();
					if (entity.name == entityName)
						entity.entityID = id;
				}
				
				Debug.Log ("Complete");
			}
		}
	}
}

