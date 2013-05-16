using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class InGameMenu : MonoBehaviour {


	private GameLogic gameLogic;
	private GUISkin skin;

	private float padding = 10.0f;
	private float unitIconSize = 80.0f;
	
	private float labelWidth = 100.0f;

	private int smallFontSize = 12;
	private int largeFontSize = 24;

	private void Awake () {
		gameLogic = GetComponent<GameLogic>();
		InitGUIStyles();
		InitGuiRects();
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
			if (GUI.Button (new Rect (padding,padding,unitIconSize,unitIconSize), new GUIContent ("Add Unit\n(Cost 50)", icons[iconNames[0]]))){
				gameLogic.AddPlayerUnit();
			}	
		}else{
			GUI.color = disabledColor;
			GUI.Button (new Rect (padding,padding,unitIconSize,unitIconSize), new GUIContent ("Add Unit\n(Cost 50)", icons[iconNames[0]]));
			RestoreGUIColor();

		}
		
		GUI.Label(moneyLabelRect, "$" + gameLogic.score.ToString(), moneyLabelStyle);
		GUI.Label(roundLabelRect, "Round " + (gameLogic.round + 1).ToString(), roundLabelStyle);
	}

	private Color moneyLabelColor = ColorWithHex(0x528D35);
	private Color roundLabelColor = ColorWithHex(0x3C3F39);
	private Color disabledColor = new Color(0.0f,0.0f,0.0f, 0.2f);
	
	private void RestoreGUIColor () {
		GUI.color = Color.white;	
	}


	static Color ColorWithHex(int hex){
	    // 0xRRGGBB
	    float r = ((hex & 0xFF0000) >> 16)/255.0f;
	    float g = ((hex & 0xFF00) >> 8)/255.0f;
	    float b = (hex & 0xFF)/255.0f;
	    return new Color(r,g,b,1.0f);
	}


	private int addUnitCost = 50;

	private bool CanAddUnit() {
		return gameLogic.score >= addUnitCost;
	}

	private static string[] iconNames= {
		"AddUnit"
	};

	private Dictionary<string, Texture2D> icons = new Dictionary<string, Texture2D>();

	private void LoadIcons() {
		icons.Add(iconNames[0], Resources.Load(iconNames[0], typeof(Texture2D)) as Texture2D);
	}


	// GUI Styles

	private GUIStyle moneyLabelStyle;
	private GUIStyle roundLabelStyle;

	private void InitGUIStyles () {
		moneyLabelStyle = new GUIStyle();
		moneyLabelStyle.fontSize = largeFontSize;
		moneyLabelStyle.alignment = TextAnchor.UpperRight;
		moneyLabelStyle.normal.textColor = moneyLabelColor;

		roundLabelStyle = new GUIStyle();
		roundLabelStyle.fontSize = largeFontSize;
		roundLabelStyle.alignment = TextAnchor.UpperCenter;
		roundLabelStyle.normal.textColor = roundLabelColor;
	}

	private Rect moneyLabelRect;
	private Rect roundLabelRect;

	private void InitGuiRects () {
		moneyLabelRect = new Rect(Screen.width - padding - labelWidth,
			padding,
			labelWidth,
			largeFontSize);

		roundLabelRect = new Rect(Screen.width/2 - labelWidth/2,
			padding,
			labelWidth,
			largeFontSize);
	}
	

}
