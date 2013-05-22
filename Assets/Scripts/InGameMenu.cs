using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class InGameMenu : MonoBehaviour {


	private GameLogic gameLogic;
	private GUISkin skin;

	private float padding = 10.0f;
	private float textPaddingFromIcon = 5.0f;
	private float unitIconSize = 80.0f;
	private float upgradeIconSize = 40.0f;
	
	private float labelWidth = 100.0f;

	private int smallFontSize = 12;
	private int largeFontSize = 24;

	private void Awake () {
		gameLogic = GetComponent<GameLogic>();
		InitGUIStyles();
		InitGUIRects();
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

		DrawUnitButtons();
		DrawUpgradeButtons();

		GUI.Label(moneyLabelRect, "$" + gameLogic.score.ToString(), moneyLabelStyle);
		GUI.Label(roundLabelRect, "Round " + (gameLogic.round + 1).ToString(), roundLabelStyle);
	}


	// Unit Buttons

	private void DrawUnitButtons () {
		List <string> buttons = gameLogic.UnitButtons();
		for (int i = 0; i < buttons.Count; i++){
			Rect r = new Rect(padding, 
				padding + i * (unitIconSize + padding),
				unitIconSize,
				unitIconSize);
			UnitType type = (UnitType)i;
			DrawAUnitButton(r, buttons[i], type);

			Rect decriptionRect = new Rect(padding + unitIconSize + padding,
				padding + i * (unitIconSize + padding) + textPaddingFromIcon,
				labelWidth,
				smallFontSize);
			DrawAUnitButtonDescription(decriptionRect, type);
		}
	}

	private void DrawAUnitButton (Rect r, string s, UnitType type){
		if (gameLogic.IsUnitButtonAvailable(type)){
			if (GUI.Button(r, IconWithName(s))){
				gameLogic.AddPlayerUnit(type);
			}
		}else{
			GUI.color = disabledColor;
			GUI.Button(r, IconWithName(s));
			RestoreGUIColor();
		}
	}

	private void DrawAUnitButtonDescription (Rect r, UnitType type){
		GUI.Label(r, gameLogic.DescriptionForUnitType(type), unitLabelStyle);
	}


	// Upgrade Buttons

	private void DrawUpgradeButtons () {
		List <Upgrade> nextUpgrades = gameLogic.nextUpgrades;
		float unitButtonSectionSize = (padding + unitIconSize) * 3 + padding;

		for (int i = 0; i < nextUpgrades.Count; i++){
			Upgrade u = nextUpgrades[i];

			Rect r = new Rect(padding,
				unitButtonSectionSize + i * (upgradeIconSize + padding),
				upgradeIconSize,
				upgradeIconSize);
			DrawAUpgradeButton(r, u);


			Rect decriptionRect = new Rect(padding + upgradeIconSize + padding,
				unitButtonSectionSize + i * (upgradeIconSize + padding) + textPaddingFromIcon,
				labelWidth,
				smallFontSize);
			DrawAUpgradeButtonDescription(decriptionRect, u);
		}
	}

	private void DrawAUpgradeButton (Rect r, Upgrade u) {
		if (gameLogic.IsUpgradeButtonAvailable(u)){
			if (GUI.Button(r, IconWithName(u.id))){
				gameLogic.UnlockUpgrade(u);
			}
		}else{
			GUI.color = disabledColor;
			GUI.Button(r, IconWithName(u.id));
			RestoreGUIColor();
		}
	}

	private void DrawAUpgradeButtonDescription (Rect r, Upgrade u) {
		GUI.Label(r, u.description, unitLabelStyle);
	}



	// GUI Styles

	private GUIStyle moneyLabelStyle;
	private GUIStyle roundLabelStyle;
	private GUIStyle unitLabelStyle;

	private void InitGUIStyles () {
		moneyLabelStyle = new GUIStyle();
		moneyLabelStyle.fontSize = largeFontSize;
		moneyLabelStyle.alignment = TextAnchor.UpperRight;
		moneyLabelStyle.normal.textColor = moneyLabelColor;

		roundLabelStyle = new GUIStyle();
		roundLabelStyle.fontSize = largeFontSize;
		roundLabelStyle.alignment = TextAnchor.UpperCenter;
		roundLabelStyle.normal.textColor = roundLabelColor;


		unitLabelStyle = new GUIStyle();
		unitLabelStyle.fontSize = smallFontSize;
		unitLabelStyle.alignment = TextAnchor.UpperLeft;
		unitLabelStyle.normal.textColor = secondaryTextColor;
	}

	private Rect moneyLabelRect;
	private Rect roundLabelRect;

	private void InitGUIRects () {
		moneyLabelRect = new Rect(Screen.width - padding - labelWidth,
			padding,
			labelWidth,
			largeFontSize);

		roundLabelRect = new Rect(Screen.width/2 - labelWidth/2,
			padding,
			labelWidth,
			largeFontSize);
	}



	// Helper Functions

	private Color moneyLabelColor = ColorWithHex(0x528D35);
	private Color roundLabelColor = ColorWithHex(0x3C3F39);
	private Color secondaryTextColor = ColorWithHex(0x9AA09D);


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


	// Load/Unload

	private static string[] iconNames= {
		"AddUnit",
		"UnitBox",
		"UnitTriangle",
		"UnitSphere",
		"NotFound"
	};

	private Dictionary<string, Texture2D> icons = new Dictionary<string, Texture2D>();

	private void LoadIcons() {
		for (int i = 0; i < iconNames.Length; i++){
			icons.Add(iconNames[i], Resources.Load(iconNames[i], typeof(Texture2D)) as Texture2D);
		}
	}

	private Texture2D IconWithName(string s){
		if (icons.ContainsKey(s)){
			return icons[s];
		}
		return icons["NotFound"];
	}

	

}
