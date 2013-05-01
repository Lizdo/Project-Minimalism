using UnityEngine;
using System.Collections;

public class Encounter : MonoBehaviour {

	public enum EncounterType {
		Combat,
		Resource,
		Unit	
	}

	public int round;
	public Vector3 position;
	private bool isResolved;

	public void SetRoundAndPosition (int r, Vector3 p) {
		round = r;
		position = p;
	}

	public bool IsResolved () {
		return isResolved;
	}

	void Update () {

	}

}
