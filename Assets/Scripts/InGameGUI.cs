using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class InGameGUI : MonoBehaviour {


	private GameLogic gameLogic;

	[HideInInspector]
	public GUISkin skin;

	private float padding = 10.0f;
	private float textPaddingFromIcon = 5.0f;
	private float unitIconSize = 80.0f;
	private float upgradeIconSize = 40.0f;

	private float paddingBetweenUnitButtonAndUpgradeButton = 20.0f;

	private float labelWidth = 100.0f;

	[HideInInspector]
	public int smallFontSize = 12;

	[HideInInspector]
	public int mediumFontSize = 18;
	
	[HideInInspector]
	public int largeFontSize = 24;

	private Texture2D lightBackground;
	private Texture2D darkBackground;

	private bool showUpgradeTree;

	private void Awake () {
		gameLogic = GetComponent<GameLogic>();
	}

	// Use this for initialization
	void Start () {
		LoadIcons();
		skin = Resources.Load("GUISkin", typeof(GUISkin)) as GUISkin;
		lightBackground = Resources.Load("LightBackground", typeof(Texture2D)) as Texture2D;
		darkBackground = Resources.Load("DarkBackground", typeof(Texture2D)) as Texture2D;

		InitGUIStyles();
		InitGUIRects();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI () {
		GUI.skin = skin;

		DrawUnitButtons();
		if (showUpgradeTree){
			DrawUpgradeTree();
		}else{
			DrawUpgradeButtons();
		}
		DrawGlobalUI();
	}


	// Unit Buttons

	private void DrawUnitButtons () {
		List <string> buttons = gameLogic.UnitButtons();
		for (int i = 0; i < buttons.Count; i++){
			Rect r = new Rect(padding, 
				padding + i * (unitIconSize + padding),
				unitIconSize,
				unitIconSize);
			string unitTypeInString = buttons[i].Substring(4); //UnitTriangle -> Triangle
			UnitType type = (UnitType)Enum.Parse(typeof(UnitType), unitTypeInString);
			DrawAUnitButton(r, buttons[i], type);

			Rect nameRect = new Rect(r.x + r.width + padding,
				r.y + textPaddingFromIcon,
				labelWidth,
				smallFontSize);
			float nameRectWidth = DrawAUnitName(nameRect, type);

			Rect costDecriptionRect = new Rect(r.x + r.width + padding + nameRectWidth + padding,
				r.y + textPaddingFromIcon,
				labelWidth,
				smallFontSize);
			DrawAUnitButtonCostDescription(costDecriptionRect, type);

			Rect decriptionRect = new Rect(r.x + r.width + padding,
				r.y + textPaddingFromIcon + smallFontSize,
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

	private float DrawAUnitName (Rect r, UnitType type) {
		GUI.Label(r, type.ToString(), nameLabelStyle);
		float minWidth, maxWidth;
		nameLabelStyle.CalcMinMaxWidth(new GUIContent(type.ToString()), out minWidth, out maxWidth);
		return maxWidth;
	}

	private void DrawAUnitButtonCostDescription (Rect r, UnitType type){
		GUI.Label(r, gameLogic.CostDescriptionForUnitType(type), costLabelStyle);
	}

	private void DrawAUnitButtonDescription (Rect r, UnitType type){
		GUI.Label(r, gameLogic.DescriptionForUnitType(type), descriptionLabelStyle);
	}


	// Upgrade Buttons

	private void DrawUpgradeButtons () {
		List <Upgrade> nextUpgrades = gameLogic.nextUpgrades;
		float unitButtonSectionSize = (padding + unitIconSize) * 3 + padding;

		for (int i = 0; i < nextUpgrades.Count; i++){
			Upgrade u = nextUpgrades[i];

			Rect r = new Rect(padding + unitIconSize - upgradeIconSize,
				unitButtonSectionSize + i * (upgradeIconSize + padding) + paddingBetweenUnitButtonAndUpgradeButton,
				upgradeIconSize,
				upgradeIconSize);
			DrawAUpgradeButton(r, u);

			Rect nameRect = new Rect(r.x + r.width + padding,
				r.y + textPaddingFromIcon,
				labelWidth,
				smallFontSize);
			float nameRectWidth = DrawAUpgradeName(nameRect, u);			

			Rect costRect = new Rect(r.x + r.width + padding + nameRectWidth + padding,
				r.y + textPaddingFromIcon,
				labelWidth,
				smallFontSize);
			DrawAUpgradeCost(costRect, u);			


			Rect decriptionRect = new Rect(r.x + r.width + padding,
				r.y + textPaddingFromIcon + smallFontSize,
				labelWidth,
				smallFontSize);
			DrawAUpgradeDescription(decriptionRect, u);
		}
	}

	private void DrawAUpgradeButton (Rect r, Upgrade u) {
		if (u.unlocked){
			GUI.color = enabledColor;
			GUI.Button(r, IconWithName(u.id));
			RestoreGUIColor();
			return;
		}
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

	private float DrawAUpgradeName (Rect r, Upgrade u) {
		GUI.Label(r, u.id, nameLabelStyle);
		float minWidth, maxWidth;
		nameLabelStyle.CalcMinMaxWidth(new GUIContent(u.id), out minWidth, out maxWidth);
		return maxWidth;
	}


	private void DrawAUpgradeCost (Rect r, Upgrade u) {
		GUI.Label(r, u.CostDescription(), costLabelStyle);
	}

	
	private void DrawAUpgradeDescription (Rect r, Upgrade u) {
		GUI.Label(r, u.description, descriptionLabelStyle);
	}



	// GUI Styles

	private GUIStyle moneyLabelStyle;
	private GUIStyle roundLabelStyle;
	private GUIStyle costLabelStyle;
	private GUIStyle descriptionLabelStyle;
	private GUIStyle nameLabelStyle;
	private GUIStyle toggleUpgradeButtonStyle;

	private void InitGUIStyles () {
		moneyLabelStyle = new GUIStyle();
		moneyLabelStyle.fontSize = largeFontSize;
		moneyLabelStyle.alignment = TextAnchor.UpperRight;
		moneyLabelStyle.normal.textColor = moneyLabelColor;

		roundLabelStyle = new GUIStyle();
		roundLabelStyle.fontSize = largeFontSize;
		roundLabelStyle.alignment = TextAnchor.UpperCenter;
		roundLabelStyle.normal.textColor = roundLabelColor;


		descriptionLabelStyle = new GUIStyle();
		descriptionLabelStyle.fontSize = smallFontSize;
		descriptionLabelStyle.alignment = TextAnchor.UpperLeft;
		descriptionLabelStyle.normal.textColor = secondaryTextColor;

		costLabelStyle = new GUIStyle();
		costLabelStyle.fontSize = smallFontSize;
		costLabelStyle.alignment = TextAnchor.UpperLeft;
		costLabelStyle.normal.textColor = costLabelColor;

		nameLabelStyle = new GUIStyle();
		nameLabelStyle.fontSize = smallFontSize;
		nameLabelStyle.alignment = TextAnchor.UpperLeft;
		nameLabelStyle.normal.textColor = primaryTextColor;

		toggleUpgradeButtonStyle = new GUIStyle();
		toggleUpgradeButtonStyle.alignment = TextAnchor.MiddleCenter;
		toggleUpgradeButtonStyle.normal.background = lightBackground;
		toggleUpgradeButtonStyle.normal.textColor = secondaryTextColor;
		toggleUpgradeButtonStyle.active.background = darkBackground;

	}

	private Rect moneyLabelRect;
	private Rect roundLabelRect;
	private Rect toggleUpgradeButtonRect;

	private float showUpgradeButtonWidth = 60.0f;
	private float showUpgradeButtonHeight = 40.0f;

	private void InitGUIRects () {
		moneyLabelRect = new Rect(Screen.width - padding - labelWidth,
			padding,
			labelWidth,
			largeFontSize);

		roundLabelRect = new Rect(Screen.width/2 - labelWidth/2,
			padding,
			labelWidth,
			largeFontSize);

		toggleUpgradeButtonRect = new Rect(padding, Screen.height-padding-showUpgradeButtonHeight,
			showUpgradeButtonWidth,showUpgradeButtonHeight);
	}

	// Upgrade Tree

	private float upgradeTreeLineWidth = 2.0f;

	private void DrawUpgradeTree () {
		List <Upgrade> upgrades = gameLogic.AllUpgrades();


		for (int i = 0; i < upgrades.Count; i++){
			Upgrade u = upgrades[i];
			Rect r = RectForUpgradeIDXandIDY(u.idX, u.idY);
			DrawAUpgradeButton(r,u);


			Vector2 rectCenter;
			Vector2 lastRectCenter;
			Rect lineR;

			GUI.color = disabledColor;

			if (u.idY > 0){
				// Draw a line to idY - 1
				rectCenter = r.center;
				lastRectCenter = RectForUpgradeIDXandIDY(u.idX, u.idY-1).center;
				lineR = new Rect(lastRectCenter.x - upgradeTreeLineWidth/2,
					lastRectCenter.y + upgradeIconSize/2,
					upgradeTreeLineWidth,
					rectCenter.y-lastRectCenter.y - upgradeIconSize);
				GUI.Box(lineR, "");
			}else if (u.idX > 0){
				// Draw a line to idX - 1, no need if idY > 0 because its parent will draw it
				rectCenter = r.center;
				lastRectCenter = RectForUpgradeIDXandIDY(u.idX-1, u.idY).center;
				lineR = new Rect(lastRectCenter.x + upgradeIconSize/2,
					lastRectCenter.y - upgradeTreeLineWidth/2,
					rectCenter.x - lastRectCenter.x - upgradeIconSize,
					upgradeTreeLineWidth);
				GUI.Box(lineR, "");

			}

			RestoreGUIColor();

			Rect nameRect = new Rect(r.x,
				r.y - smallFontSize - padding,
				labelWidth,
				smallFontSize);
			DrawAUpgradeName(nameRect,u);

			Rect costRect = new Rect(r.x + r.width,
				r.y + textPaddingFromIcon,
				labelWidth,
				smallFontSize);

			DrawAUpgradeCost(costRect,u);

		}

		// TODO: Draw a background Rect

	}


	private Rect RectForUpgradeIDXandIDY (int idX, int idY) {
		float unitButtonSectionSize = (padding + unitIconSize) * 3 + padding;
		float nameRectWidth = 80;
		return new Rect (padding + idX * (upgradeIconSize + padding + nameRectWidth) + unitIconSize - upgradeIconSize,
				unitButtonSectionSize + padding + padding + idY * (upgradeIconSize + padding*3),
				upgradeIconSize,
				upgradeIconSize
			);
	}


	// Global UI

	private void DrawGlobalUI () {
		GUI.Label(moneyLabelRect, "$" + gameLogic.score.ToString(), moneyLabelStyle);
		GUI.Label(roundLabelRect, gameLogic.RoundDescription(), roundLabelStyle);

		string toggleUpgradeButtonString = "Upgrade\nButtons";

		if (!showUpgradeTree){
			toggleUpgradeButtonString = "Upgrade\nTree";
		}


		if (GUI.Button(toggleUpgradeButtonRect, toggleUpgradeButtonString, toggleUpgradeButtonStyle)){
			showUpgradeTree = !showUpgradeTree;
		}
	}


	// Helper Functions

	[HideInInspector]
	public Color moneyLabelColor = ColorWithHex(0x528D35);
	[HideInInspector]
	public Color roundLabelColor = ColorWithHex(0x3C3F39);
	[HideInInspector]
	public Color costLabelColor = ColorWithHex(0x528D35);
	[HideInInspector]
	public Color primaryTextColor = ColorWithHex(0x505353);	
	[HideInInspector]
	public Color secondaryTextColor = ColorWithHex(0x9AA09D);


	[HideInInspector]
	public Color disabledColor = new Color(0.0f,0.0f,0.0f, 0.2f);
	[HideInInspector]
	public Color enabledColor = new Color(0.0f,0.0f,0.0f, 0.8f);
	
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
