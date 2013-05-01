using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {

	// High level behavior is controlled by Battalion, Unit will take care of low level behavior like movement, attack...etc
	private Battalion battalion;
	private Vector3 offsetFromBattalion;

	private const int MAX_OFFSET_FROM_BATTALION = 20;

	public void RandomizeOffset(){
		offsetFromBattalion = new Vector3(Random.value * MAX_OFFSET_FROM_BATTALION,
			0,
			Random.value * MAX_OFFSET_FROM_BATTALION);
	}

	void Awake (){
		RandomizeOffset();
		battalion = FindObjectOfType(typeof(Battalion)) as Battalion;
	}

	void Start () {
		Vector3 battalionPositionOnFloor = new Vector3(battalion.transform.position.x,
			0,
			battalion.transform.position.z);

		transform.position = battalionPositionOnFloor + offsetFromBattalion;
		battalion.Add(this);
	}
	
	void Update () {
		// If Enemy/Obj In Range && Not Processed, deal with it
		// Move to next encounter's location
	}

}
