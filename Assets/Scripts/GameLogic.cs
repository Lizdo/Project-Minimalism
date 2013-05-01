using UnityEngine;
using System.Collections;

public class GameLogic : MonoBehaviour {

	// Handles Game Progression

	public int round;

	private Encounter previousEncounter;
	private Encounter currentEncounter;

	private Battalion battalion;


	// Public functions

	public Vector3 CurrentEncounterPosition() {
		return currentEncounter.position;
	}


	// Plugs to the UI

	public void AddNewUnit () {
		Instantiate(Resources.Load("Unit"));
	}

	public void Reset () {
		round = 0;
	}	


	// Routines

	private void Awake () {
		battalion = GetComponent<Battalion>();
		InitializeFirstEncounter();
	}

	private void InitializeFirstEncounter(){
		currentEncounter = NextEncounter();
	}

	private void Start () {
		if (DebugHelper.doTests){
			StartCoroutine(TestAddNewUnit());
		}
	}

	private IEnumerator TestAddNewUnit () {
		AddNewUnit();
		yield return new WaitForSeconds(1);
		AddNewUnit();
		yield return new WaitForSeconds(1);
		AddNewUnit();
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
		return new Vector3(100,0,100);
	}


}
