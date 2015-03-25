using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreDisplay : MonoBehaviour {
	private Text textDisplay;
	private Outline outlineEffect;

	private float timerScale;
	private float amplitudeScale = 0.25f;

	public bool activeGlow;
	private float amplitudeMean = 3.0f;
	private float amplitudeGlow = 1.0f;
	private float timerGlow;

	public Text TextDisplay {
		get { return textDisplay; }
		set { textDisplay = value; }
	}

	// Use this for initialization
	void Start () {
		textDisplay = GetComponent<Text> ();
		outlineEffect = GetComponent<Outline> ();
		timerScale = 0.0f;
		timerGlow = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		if (activeGlow) {
			timerGlow += Time.deltaTime;
			if (timerGlow >= 2.0f)
				timerGlow -= 2.0f;
	
			float strength = amplitudeMean + amplitudeGlow * UnityEngine.Mathf.Sin (timerGlow * UnityEngine.Mathf.PI);
			outlineEffect.effectDistance = new Vector2 (strength, strength);
		} else {
			outlineEffect.effectDistance = Vector2.zero;
		}

		if (timerScale > 0.0f) {
			timerScale -= Time.deltaTime;
		} else
			timerScale = 0.0f;
		this.transform.localScale = Vector3.one * (1 + amplitudeScale * UnityEngine.Mathf.Sin (2 * timerScale * Mathf.PI));
	}

	public void TriggerScale() {
		timerScale = 0.25f;
	}
}
