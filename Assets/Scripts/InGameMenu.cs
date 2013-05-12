using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class InGameMenu : MonoBehaviour {


	private GameLogic gameLogic;
	private GUISkin skin;

	private void Awake () {
		gameLogic = GetComponent<GameLogic>();
	}

	// Use this for initialization
	void Start () {
		LoadIcons();
		skin = Resources.Load("GUISkin", typeof(GUISkin)) as GUISkin;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI () {
		GUI.skin = skin;

		if (CanAddUnit()){
			if (GUI.Button (new Rect (10,40,80,80), new GUIContent ("Add Unit\n(Cost 50)", icons[iconNames[0]]))){
				gameLogic.AddPlayerUnit();
			}	
		}else{
			SetDisabledColor();
			GUI.Button (new Rect (10,40,80,80), new GUIContent ("Add Unit\n(Cost 50)", icons[iconNames[0]]));
			RestoreDisabledColor();

		}
		

		GUI.Label(new Rect (10,10, 100, 80), "$" + gameLogic.score.ToString());
	}


	private void SetDisabledColor () {
		GUI.color = new Color(0.0f,0.0f,0.0f, 0.2f);
	}

	private void RestoreDisabledColor () {
		GUI.color = Color.white;	
	}

	private int addUnitCost = 50;

	private bool CanAddUnit(){
		return gameLogic.score >= addUnitCost;
	}


	private static string[] iconNames= {
		"AddUnit"
	};

	private Dictionary<string, Texture2D> icons = new Dictionary<string, Texture2D>();

	private void LoadIcons(){
		icons.Add(iconNames[0], Resources.Load(iconNames[0], typeof(Texture2D)) as Texture2D);
	}
}
