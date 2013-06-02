using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour {

	private string text = "Testing this very long tutorial";

	private float timeToDisplay = 4.0f;
	private float fadeTime = 1.0f;
	private float alpha = 0.0f;

	private float width = 250.0f;
	private float height = 150.0f;

	private GUIStyle style;
	private InGameGUI menu;
	private Rect rect;

	private Color color;

	void Awake () {
		menu = GetComponent<InGameGUI>();
	}

	// Use this for initialization
	void Start () {
		style = new GUIStyle();
		style.fontSize = menu.mediumFontSize;
		style.alignment = TextAnchor.UpperLeft;
		style.wordWrap = true;
		color = menu.primaryTextColor;
	}
	
	// Update is called once per frame
	void Update () {
		style.normal.textColor = new Color(color.r, color.g, color.b, alpha);
	}

	private void OnGUI () {
		GUI.skin = menu.skin;

		if (alpha <= 0){
			return;
		}

		GUI.Label(rect, text, style);
	}


	public void Show (string s = null) {
		StartCoroutine(ClearAndShowNewTutorial(s));
	}


	IEnumerator ClearAndShowNewTutorial (string s) {
		StopCoroutine("TutorialCycle");

		yield return StartCoroutine(FadeOut());

		if (s != null) {
			text = s;
		}

		InitNewTutorial();
		StartCoroutine("TutorialCycle");

	}

	private float distanceFromCenterX = 200;
	private float distanceFromCenterY = 120;

	private void InitNewTutorial () {
		// TODO: change to a angle based randomization 
		// 		and use the direction to the next encounter
		float randomAngle = Random.Range(0, 360.0f) * Mathf.Deg2Rad;
		float randomX = Screen.width/2 + distanceFromCenterX * Mathf.Cos(randomAngle) - width/2;
		float randomY = Screen.height/2 + distanceFromCenterY * Mathf.Sin(randomAngle) - height/2;

		rect = new Rect(randomX, randomY, width, height);
	}

	IEnumerator TutorialCycle () {
		yield return StartCoroutine(FadeIn());
		yield return new WaitForSeconds(timeToDisplay-fadeTime * 2);
		yield return StartCoroutine(FadeOut());
	}


	IEnumerator FadeOut () {
		while (alpha > 0){
			float alphaChangePerFrame = 1.0f/fadeTime * Time.deltaTime;
			alpha -= alphaChangePerFrame;
			yield return null;
		}
		alpha = 0;
	}

	IEnumerator FadeIn () {
		while (alpha < 1){
			float alphaChangePerFrame = 1.0f/fadeTime * Time.deltaTime;
			alpha += alphaChangePerFrame;
			yield return null;
		}
		alpha = 1;
	}
}
