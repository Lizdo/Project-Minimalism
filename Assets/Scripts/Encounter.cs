using UnityEngine;
using System.Collections;

public class Encounter : MonoBehaviour {

	public enum EncounterType {
		Combat,
		Resource,
		Unit	
	}

	public int round;
	private bool isResolved;

	private GameLogic gameLogic;


	// Public functions

	public void SetRoundAndPosition (int r, Vector3 p) {
		round = r;
		transform.position = p;
	}

	public bool IsResolved () {
		return isResolved;
	}


	// Core Updates

	private void Awake () {
		gameLogic = FindObjectOfType(typeof(GameLogic)) as GameLogic;
	}

	private void Start () {
		SpawnEnemies();
	}

	private void Update () {
		if (!isResolved)
			isResolved = gameLogic.AllEnemiesDead();
	}



	// Helper functions

	private void SpawnEnemies () {
		for (int i = 0; i < round+1; i++){
			gameLogic.AddEnemyUnit();
		}
	}

}