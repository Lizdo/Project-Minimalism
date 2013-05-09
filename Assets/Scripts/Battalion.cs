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
	private GameLogic gameLogic;
	private Battalion enemyBattalion;


	// Public functions

	public void Add (Unit unit) {
		units.Add(unit);
	}

	public void Remove (Unit unit){
		units.Remove(unit);
	}

	public Unit ValidTargetInRange (Unit attackingUnit) {
		foreach (Unit u in enemyBattalion.units){
			if (u.enemy == null
				&& attackingUnit.DistanceToUnit(u) <= attackingUnit.attackingDistance){
				Debug.Log("Found Valid Target", u);
				return u;
			}
		}
		return null;
	}

	public Vector3 MovementTarget () {
		if (isEnemy)
			return transform.position;
		else
			return gameLogic.CurrentEncounterPosition();
	}


	public bool AllUnitsDead () {
		return units.Count == 0;
	}

	// Updates

	private void Awake () {
		gameLogic = FindObjectOfType(typeof(GameLogic)) as GameLogic;
		if (isEnemy){
			enemyBattalion = GameObject.Find("PlayerBattalion").GetComponent<Battalion>();
		}else{
			enemyBattalion = GameObject.Find("EnemyBattalion").GetComponent<Battalion>();
		}
		CalculatePosition();
	}

	private void Start () {
	}
	
	// Update is called once per frame
	private void Update () {
		CalculatePosition();
		DebugHelper.DrawLine(transform.position,
			gameLogic.CurrentEncounterPosition());
	}


	private void CalculatePosition(){
		if (isEnemy){
			transform.position = gameLogic.CurrentEncounterPosition();
			return;
		}

		if (units.Count == 0){
			transform.position = lastBound.center;
			return;
		}

		Bounds b = new Bounds(units[0].transform.position, 
			units[0].transform.position);

		for (int i = 1; i < units.Count; i++){
			b.Encapsulate(units[i].transform.position);
		}

		lastBound = b;
		transform.position = b.center;
	}


	// Boid
	//	http://www.vergenet.net/~conrad/boids/pseudocode.html

	public Vector3 CohesionVelocity (Unit unit) {
		if (units.Count <= 1)
			return Vector3.zero;

		Vector3 center = Vector3.zero;
		foreach (Unit u in units){
			if (u != unit){
				center += u.transform.position;
			}
		}

		center = center/(units.Count - 1);
		return (center - unit.transform.position) / 100;
	}

	private float distanceBetweenUnits = 3.0f;

	public Vector3 SeparationVelocity (Unit unit) {
		Vector3 c = Vector3.zero;
		foreach (Unit u in units){
			if (u != unit){
				if (u.DistanceToUnit(unit) < distanceBetweenUnits){
					c = c - (u.transform.position - unit.transform.position);
				}
			}
		}
		return c;
	}

	public Vector3 AlignmentVelocity (Unit unit) {
		if (units.Count <= 1)
			return Vector3.zero;

		Vector3 perceivedVelocity = Vector3.zero;
		foreach (Unit u in units){
			if (u != unit){
				perceivedVelocity += u.boidVelocity;
			}
		}

		perceivedVelocity /= (units.Count - 1);
		return (perceivedVelocity - unit.boidVelocity)/8;
	}

	public Vector3 TendToPlace (Unit unit, Vector3 destination) {
		return (destination - unit.transform.position) / 100;
	}


}
