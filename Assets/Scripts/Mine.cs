using UnityEngine;
using System.Collections;

public class Mine : MonoBehaviour {

	public int resource = 200;
	public Unit miner;

	private const float MAX_OFFSET_FROM_ENCOUNTER = 10.0f;
	private Quaternion defaultRotation = Quaternion.Euler(270,0,0);
	private Vector3 offsetFromEncounter;

	private EncounterResource encounter;
	private GameLogic gameLogic;

	private void Awake () {
		gameLogic = FindObjectOfType(typeof(GameLogic)) as GameLogic;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public bool IsDepleted () {
		if (resource <= 0)
			return true;
		return false;
	}

	private float miningDistance = 20.0f;

	public bool IsAvailableToUnit (Unit u) {
		if (miner) return false;
		if (IsDepleted()) return false;

		float distance = Vector3.Distance(this.transform.position, u.transform.position);
		return (distance <= miningDistance);
	}

	public void SetEncounter (EncounterResource e){
		RandomizeOffset();
		RandomizeRotation();

		encounter = e;
		transform.position = encounter.transform.position + offsetFromEncounter;
	}

	private int mineSpeedPerSec = 100;

	public void Mined() {
		resource -= mineSpeedPerSec;
		gameLogic.ResourceMined(mineSpeedPerSec);
		if (IsDepleted()){
			Deplete();
		}
	}

	private void Deplete() {
		renderer.material.color = Color.gray;
	}

	private void RandomizeOffset () {
		offsetFromEncounter = new Vector3(Random.value * MAX_OFFSET_FROM_ENCOUNTER,
			0,
			Random.value * MAX_OFFSET_FROM_ENCOUNTER);
	}

	private void RandomizeRotation () {
		float randomRotation = Random.value * 360;
		transform.rotation = Quaternion.Euler(0, randomRotation, 0) * defaultRotation;
	}

}
