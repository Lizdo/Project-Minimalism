using UnityEngine;
using System.Collections;

public class GameLogic : MonoBehaviour {

	// Handles Game Progression

	public int round;

	public Battalion playerBattalion;
	public Battalion enemyBattalion;

	public Unit playerUnit;
	public Unit enemyUnit;

	private Encounter previousEncounter;
	private Encounter currentEncounter;


	// Public functions

	public Vector3 CurrentEncounterPosition() {
		return currentEncounter.position;
	}

	public bool AllEnemiesDead () {
		return enemyBattalion.AllUnitsDead();
	}

	// Plugs to the UI

	public void AddPlayerUnit () {
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
		InitializeFirstEncounter();
	}

	private void InitializeFirstEncounter(){
		currentEncounter = NextEncounter();
	}

	private void Start () {
		if (DebugHelper.doTests){
			StartCoroutine(TestAddPlayerUnit());
		}
	}

	private IEnumerator TestAddPlayerUnit () {
		AddPlayerUnit();
		yield return new WaitForSeconds(0.5f);
		AddPlayerUnit();
		yield return new WaitForSeconds(0.5f);
		AddPlayerUnit();
	}

	private void Update () {
		UpdateCurrentEncounter();
		DebugHelper.DrawCross(CurrentEncounterPosition());
	}

	private void UpdateCurrentEncounter (){
		DebugHelper.Assert(currentEncounter != null);
		if (currentEncounter.IsResolved()){
			round++;
			previousEncounter = currentEncounter;
			currentEncounter = NextEncounter();
		}
	}

	private Encounter NextEncounter () {
		Encounter e = (Instantiate(Resources.Load("Encounter", typeof(GameObject))) as GameObject).GetComponent<Encounter>();
		e.SetRoundAndPosition(round, NextEncounterPosition());
		return e;
	}

	private Vector3 NextEncounterPosition () {
		// TODO: Random off-screen location
		return new Vector3(50,0,50);
	}


}
