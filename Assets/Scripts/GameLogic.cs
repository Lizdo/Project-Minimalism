using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameLogic : MonoBehaviour {

	// Handles Game Progression

	public int round;

	public Battalion playerBattalion;
	public Battalion enemyBattalion;

	public Unit playerUnit;
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


	private int scorePerUnit = 100;
	private int costPerUnit = 50;

	public void EnemyKilled (Unit u) {
		score += scorePerUnit;
	}

	public void ResourceMined (int value) {
		score += value;
	}

	// Plugs to the UI

	public void AddPlayerUnit () {
		score -= costPerUnit;
		AddPlayerUnitWithoutCost();
	}

	private void AddPlayerUnitWithoutCost () {
		Unit u = Instantiate(playerUnit) as Unit;
		u.AddToBattalion(playerBattalion);
	}

	public void AddEnemyUnit () {
		Unit u = Instantiate(enemyUnit) as Unit;
		u.AddToBattalion(enemyBattalion);
	}

	public void Reset () {
		round = 0;
	}


	// Routines

	private void Awake () {
		InitEntityPool();
		InitializeFirstEncounter();
	}

	private void InitializeFirstEncounter(){
		currentEncounter = NextEncounter();
	}

	private void Start () {
		SpawnEntitiesAroundNextEncounter();		
		if (DebugHelper.doTests){
			StartCoroutine(TestAddPlayerUnit());
		}
	}

	private IEnumerator TestAddPlayerUnit () {
		AddPlayerUnitWithoutCost();
		yield return new WaitForSeconds(0.5f);
		AddPlayerUnitWithoutCost();
		yield return new WaitForSeconds(0.5f);
		AddPlayerUnitWithoutCost();
	}

	private void Update () {
		UpdateCurrentEncounter();
		DebugHelper.DrawCross(CurrentEncounterPosition());
	}


	///////////////////////////
	// Encounter Management
	///////////////////////////

	private void UpdateCurrentEncounter (){
		DebugHelper.Assert(currentEncounter != null);
		if (currentEncounter.IsResolved()){
			round++;
			Encounter nextEncounter = NextEncounter();
			previousEncounter = currentEncounter;
			currentEncounter = nextEncounter;
			SpawnEntitiesAroundNextEncounter();
		}
	}

	private Encounter NextEncounter () {
		float rand = Random.value;
		Encounter e;

		if (rand >= 0.33){
			e = (Instantiate(Resources.Load("EncounterCombat", typeof(GameObject))) as GameObject).GetComponent<Encounter>();
		}else{
			e = (Instantiate(Resources.Load("EncounterResource", typeof(GameObject))) as GameObject).GetComponent<Encounter>();
		}


		e.SetRoundAndPosition(round, NextEncounterPosition());
		Debug.Log("New Encounter!");
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
