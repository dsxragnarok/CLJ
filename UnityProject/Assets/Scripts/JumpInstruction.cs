using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class JumpInstruction : MonoBehaviour {

	// Use this for initialization
	void Start () {
#if UNITY_ANDROID
		GameObject spaceTextObj = GameObject.Find ("SpaceText");
		Text spaceText = spaceTextObj.GetComponent<Text>();
		Debug.Log (spaceText.text);
		spaceText.text = "Tap to jump";
#endif
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
