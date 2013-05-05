using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Battalion : MonoBehaviour {

	// This class will be in charge of the high level group behavior of the player's battalion

	public enum BattalionState{
		Idle,
		Moving,
		Attacking
	}

	public BattalionState state;
	public bool isEnemy;

	public List <Unit> units = new List <Unit> ();

	private Bounds lastBound = new Bounds();
	private Vector3 position;
	private GameLogic gameLogic;

	public Vector3 Position(){
		return position;
	}

	public void Add (Unit unit) {
		units.Add(unit);
	}

	public void Remove (Unit unit){
		units.Remove(unit);
	}


	public Vector3 MovementTarget () {
		return gameLogic.CurrentEncounterPosition();
	}


	public bool AllUnitsDead () {
		return units.Count == 0;
	}

	// Updates

	private void Awake () {
		gameLogic = FindObjectOfType(typeof(GameLogic)) as GameLogic;
	}

	private void Start () {
	
	}
	
	// Update is called once per frame
	private void Update () {
		CalculatePosition();
		DebugHelper.DrawLine(position, gameLogic.CurrentEncounterPosition());
	}


	private void CalculatePosition(){
		if (units.Count == 0){
			position = lastBound.center;
			return;
		}

		Bounds b = new Bounds(units[0].transform.position, 
			units[0].transform.position);

		for (int i = 1; i < units.Count; i++){
			b.Encapsulate(units[i].transform.position);
		}

		lastBound = b;
		position = b.center;
	}
}
