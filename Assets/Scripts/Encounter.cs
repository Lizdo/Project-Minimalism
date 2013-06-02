using UnityEngine;
using System.Collections;

public enum EncounterType {
	Combat,
	Resource,
	Unit	
}

public class Encounter : MonoBehaviour {

	[HideInInspector] public int round;
	[HideInInspector] public EncounterType type;
	protected bool isResolved;
	public string description;
	public string tutorial;

	protected GameLogic gameLogic;


	// Public functions

	public void SetRoundAndPosition (int r, Vector3 p) {
		round = r;
		transform.position = p;
	}

	public bool IsResolved () {
		return isResolved;
	}

	public virtual GameObject TargetInRange (Unit u) {
		return null;
	}


	// Core Updates

	protected virtual void Start () {

	}

	protected virtual void Awake () {
		gameLogic = FindObjectOfType(typeof(GameLogic)) as GameLogic;
	}


	protected virtual void Update () {

	}

	// Helper functions



}