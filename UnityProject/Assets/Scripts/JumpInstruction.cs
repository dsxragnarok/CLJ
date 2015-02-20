using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class JumpInstruction : MonoBehaviour {
	void Start () {
#if UNITY_ANDROID
		GameObject spaceTextObj = GameObject.Find ("SpaceText");
		Text spaceText = spaceTextObj.GetComponent<Text>();
		Debug.Log (spaceText.text);
		spaceText.text = "Tap to jump";
#endif
	}
}
