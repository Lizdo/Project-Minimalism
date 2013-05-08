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

		if (GUI.Button (new Rect (10,10,80,80), new GUIContent ("Add Unit", icons[iconNames[0]]))){
			gameLogic.AddPlayerUnit();
		}
	}


	private static string[] iconNames= {
		"AddUnit"
	};

	private Dictionary<string, Texture2D> icons = new Dictionary<string, Texture2D>();

	private void LoadIcons(){
		icons.Add(iconNames[0], Resources.Load(iconNames[0], typeof(Texture2D)) as Texture2D);
	}
}
