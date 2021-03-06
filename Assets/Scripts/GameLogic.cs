using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameLogic : MonoBehaviour {

	// Handles Game Progression

	public int round;

	public Battalion playerBattalion;
	public Battalion enemyBattalion;

	private Unit[] playerUnitPrefabs = new Unit[3];
	private Unit[] enemyUnitPrefabs = new Unit[3];

	public Unit enemyUnit;

	public Encounter previousEncounter;
	public Encounter currentEncounter;

	public int score = 200;


	// Public functions

	public Vector3 CurrentEncounterPosition() {
		return currentEncounter.transform.position;
	}

	public bool AllEnemiesDead () {
		return enemyBattalion.AllUnitsDead();
	}

	public bool EnemyInCombat () {
		return enemyBattalion.InCombat();
	}


	private int costToScoreModifier = 2;

	public void EnemyKilled (Unit u) {
		score += u.cost * costToScoreModifier;
	}

	public void ResourceMined (int value) {
		score += value;
	}

	// Plugs to the UI

	public void AddPlayerUnit (UnitType type) {
		score -= playerUnitPrefabs[(int)type].cost;
		AddPlayerUnitWithoutCost(type);
	}

	private void AddPlayerUnitWithoutCost (UnitType type) {
		Unit u = Instantiate(playerUnitPrefabs[(int)type]) as Unit;
		u.AddToBattalion(playerBattalion);
	}


	public void AddEnemyUnit (UnitType type) {
		Unit u = Instantiate(enemyUnitPrefabs[(int)type]) as Unit;
		u.AddToBattalion(enemyBattalion);
	}

	public void Reset () {
		round = 0;
	}

	public string RoundDescription () {
		string roundString = (round + 1).ToString();
		string encounterString = (currentEncounter!=null) ? 
		currentEncounter.description : "";

		return roundString + ". " + encounterString;
	}

	// Upgrades

	private List <Upgrade> upgrades = new List <Upgrade>();
	private int availableUpgradeCount = 3;

	public List< Upgrade> nextUpgrades = new List <Upgrade>(); // Used by UI


	public const string kUnlockTriangleUnit = "UnlockTriangleUnit";
	public const string kUnlockSphereUnit = "UnlockSphereUnit";
	public const string kUnlockBoxAbility1 = "UnlockBoxAbility1";
	public const string kUnlockBoxAbility2 = "UnlockBoxAbility2";
	public const string kUnlockHealthLevel1 = "UnlockHealthLevel1";
	public const string kUnlockTriangleAbility1 = "UnlockTriangleAbility1";
	public const string kUnlockTriangleAbility2 = "UnlockTriangleAbility2";

	public const string kUnitBox = "UnitBox";
	public const string kUnitTriangle = "UnitTriangle";
	public const string kUnitSphere = "UnitSphere";


	private void InitUpgrades () {
		upgrades.Add(new Upgrade(kUnlockBoxAbility1, "Unlock Box Ability 1", 5));
		upgrades.Add(new Upgrade(kUnlockTriangleUnit, "Unlock new unit type: Triangle", 5));


		upgrades.Add(new Upgrade(kUnlockBoxAbility2, "Unlock Box Ability 2", 5));
		upgrades.Add(new Upgrade(kUnlockHealthLevel1, "Increase all units health level", 5));

		upgrades.Add(new Upgrade(kUnlockSphereUnit, "Unlock new unit type: Sphere", 5));	

		upgrades.Add(new Upgrade(kUnlockTriangleAbility1, "Unlock Triangle Ability 1", 5));
		upgrades.Add(new Upgrade(kUnlockTriangleAbility2, "Unlock Triangle Ability 2", 5));

		// Setup relationship
		UpgradeWithID(kUnlockTriangleAbility1).prequisite = UpgradeWithID(kUnlockTriangleUnit);
		UpgradeWithID(kUnlockTriangleAbility2).prequisite = UpgradeWithID(kUnlockTriangleUnit);


		// Parse upgrade tree
		int currentTreeIndex = 0;
		for (int i = 0; i < upgrades.Count; i++){
			Upgrade u = upgrades[i];
			u.gameLogic = this;
			if (u.prequisite == null){
				u.idX = currentTreeIndex;
				currentTreeIndex++;
			}else{
				u.idX = u.prequisite.idX;
				u.prequisite.numberOfChildUpgrades++;
				u.idY = u.prequisite.numberOfChildUpgrades;
			}
		}

		UpdateNextUpgrades();
	}

	private void UpdateNextUpgrades () {
		nextUpgrades.Clear();

		for (int i = 0; i < upgrades.Count; i++){
			if (upgrades[i].IsAvailableToUnlock()){
				nextUpgrades.Add(upgrades[i]);
			}

			if (nextUpgrades.Count >= availableUpgradeCount){
				return;
			}
		}
	}



	private Upgrade UpgradeWithID (string _id) {
		foreach (Upgrade u in upgrades){
			if (u.id == _id){
				return u;
			}
		}
		return null;
	}

	public bool IsUpgradeUnlocked (string _id) {
		return UpgradeWithID(_id).unlocked;
	}

	public List <Upgrade> AllUpgrades () {
		return upgrades;
	}


	public void UnlockUpgrade (Upgrade u) {
		DebugHelper.Assert(u.unlocked == false);
		score -= u.cost;
		u.unlocked = true;

		if (u.id == kUnlockTriangleUnit){
			UnlockUnit(kUnitTriangle);
		}else if (u.id == kUnlockSphereUnit){
			UnlockUnit(kUnitSphere);			
		}

		UpdateNextUpgrades();
	}

	public bool IsUpgradeButtonAvailable (Upgrade _u) {
		if (!nextUpgrades.Contains(_u))
			return false;
		return score >= _u.cost;
	}


	// Unit Buttons

	public string DescriptionForUnitType (UnitType type) {
		return playerUnitPrefabs[(int)type].Description();
	}

	public string CostDescriptionForUnitType (UnitType type) {
		return playerUnitPrefabs[(int)type].CostDescription();
	}	

	private List<string> unitButtons = new List<string>();

	public List<string> UnitButtons () {
		return unitButtons;
	}

	private void InitUnitButtons() {
		unitButtons.Add(kUnitBox);	//TODO: To plug different units
	}

	private void UnlockUnit (string name) {
		unitButtons.Add(name);
	}

	public bool IsUnitButtonAvailable (UnitType type) {
		return score >= playerUnitPrefabs[(int)type].cost;
	}

	// Routines

	private Tutorial tutorial;

	private void Awake () {
		tutorial = GetComponent<Tutorial>();

		LoadUnitPrefabs();
		LoadEncounters();

		InitEntityPool();
		InitializeFirstEncounter();
		InitUnitButtons();
		InitUpgrades();
	}

	private void LoadUnitPrefabs () {
		playerUnitPrefabs[0] = (Resources.Load("PlayerUnitBox", typeof(GameObject)) as GameObject).GetComponent<Unit>();
		playerUnitPrefabs[1] = (Resources.Load("PlayerUnitTriangle", typeof(GameObject)) as GameObject).GetComponent<Unit>();
		playerUnitPrefabs[2] = (Resources.Load("PlayerUnitSphere", typeof(GameObject)) as GameObject).GetComponent<Unit>();

		enemyUnitPrefabs[0] = (Resources.Load("EnemyUnitBox", typeof(GameObject)) as GameObject).GetComponent<Unit>();
		enemyUnitPrefabs[1] = (Resources.Load("EnemyUnitTriangle", typeof(GameObject)) as GameObject).GetComponent<Unit>();
		enemyUnitPrefabs[2] = (Resources.Load("EnemyUnitSphere", typeof(GameObject)) as GameObject).GetComponent<Unit>();		
	}

	private void InitializeFirstEncounter() {
		currentEncounter = NextEncounter();
	}

	private void Start () {
		SpawnEntitiesAroundNextEncounter();		
		if (DebugHelper.doTests){
			StartCoroutine(TestAddPlayerUnit());
		}
	}

	private IEnumerator TestAddPlayerUnit () {
		AddPlayerUnitWithoutCost(UnitType.Box);
		yield return new WaitForSeconds(0.5f);
		AddPlayerUnitWithoutCost(UnitType.Box);
		yield return new WaitForSeconds(0.5f);
		AddPlayerUnitWithoutCost(UnitType.Box);
	}

	private void Update () {
		UpdateCurrentEncounter();
		DebugHelper.DrawCross(CurrentEncounterPosition());
	}


	///////////////////////////
	// Encounter Management
	///////////////////////////

	private List<Encounter> encounters = new List<Encounter>();
	private int waves = 100;

	private void LoadEncounters () {
		for (int i = 0; i < waves; i++){
			Encounter e = Resources.Load("Encounter" + i.ToString(), typeof(Encounter)) as Encounter;
			if (e != null){
				encounters.Add(e);
			}
		}
	}

	private void UpdateCurrentEncounter () {
		DebugHelper.Assert(currentEncounter != null);
		if (currentEncounter.IsResolved()){
			round++;
			Encounter nextEncounter = NextEncounter();
			previousEncounter = currentEncounter;
			currentEncounter = nextEncounter;
			SpawnEntitiesAroundNextEncounter();
			UpdateNextUpgrades();
		}
	}

	private Encounter NextEncounter () {
		Encounter e = Instantiate(encounters[round]) as Encounter;

		e.SetRoundAndPosition(round, NextEncounterPosition());
		Debug.Log("New Encounter!");

		tutorial.Show(e.tutorial);

		return e;
	}

	private float distanceBetweenEncounters = 100.0f;
	private float maxAngleBetweenEncounters = 40.0f;

	private Vector3 NextEncounterPosition () {
		Vector3 startingPosition = Vector3.zero;

		if (currentEncounter){
			startingPosition = currentEncounter.transform.position;
		}

		float lastAngle = 0;
		if (currentEncounter && previousEncounter){
			Vector3 directionBetweenEncounters = currentEncounter.transform.position - previousEncounter.transform.position;
			lastAngle = Vector3.Angle(directionBetweenEncounters, Vector3.right);
		}

		float angleToNextEncounter = lastAngle + Random.Range(-1.0f, 1.0f) * maxAngleBetweenEncounters;

		Vector3 offsetToNextEncounter = Quaternion.Euler(0, angleToNextEncounter, 0) * Vector3.right * distanceBetweenEncounters;

		return startingPosition + offsetToNextEncounter;
	}

	// Entity Spawner

	private int entityPoolSize = 100;
	private List <Entity> entityPool = new List <Entity> ();

	private void InitEntityPool () {
		Entity stone = (Resources.Load("Stone", typeof(GameObject)) as GameObject).GetComponent<Entity>();
		for (int i = 0; i < entityPoolSize; i++){
			entityPool.Add(Instantiate(stone) as Entity);
		}
	}

	private int amountOfEntityAroundEncounter = 15;
	private float maxOffsetFromEncounter = 30.0f;

	private void SpawnEntitiesAroundNextEncounter () {
		Debug.Log("Try to spawn around encounters");
		for (int i = 0; i < amountOfEntityAroundEncounter; i++){
			float randomX = Random.Range(-maxOffsetFromEncounter, maxOffsetFromEncounter);
			float randomZ = Random.Range(-maxOffsetFromEncounter, maxOffsetFromEncounter);
			Vector3 randomOffset = new Vector3(randomX, 0, randomZ);
			SpawnAnEntityAt(currentEncounter.transform.position + randomOffset);
		}
	}

	private void SpawnAnEntityAt(Vector3 p){
		foreach (Entity e in entityPool){
			if (e.canBeUsed){
				e.UseInPosition(p);
				return;
			}
		}
	}

}
