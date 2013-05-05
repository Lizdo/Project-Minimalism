using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {

	// High level behavior is controlled by Battalion, Unit will take care of low level behavior like movement, attack...etc

	public enum UnitState{
		Invalid,
		Idle,
		Moving,
		Attacking
	}

	public UnitState state = UnitState.Idle;
	public UnitState previousState;
	public UnitState nextState = UnitState.Invalid;

	public float speed = 30.0f;
	public float turningSpeed = 5.0f;

	private Battalion battalion;
	private Vector3 offsetFromBattalion;

	private const int MAX_OFFSET_FROM_BATTALION = 20;
	private Quaternion defaultRotation = Quaternion.Euler(270,0,0);

	// Core Updates

	private void Awake (){
		RandomizeOffset();
		battalion = FindObjectOfType(typeof(Battalion)) as Battalion;
	}

	private void Start () {
		Vector3 battalionPositionOnFloor = new Vector3(battalion.transform.position.x,
			0,
			battalion.transform.position.z);

		transform.position = battalionPositionOnFloor + offsetFromBattalion;
		battalion.Add(this);

		StartCoroutine(FSM());
	}

	// Coroutine based State Machine

	IEnumerator FSM () {
	    // Execute the current coroutine (state)
	    while (true)
			yield return StartCoroutine(state.ToString());
	}

	IEnumerator Idle () {
		yield return StartCoroutine("EnterState");

		while (nextState == UnitState.Invalid){
		// Execute State
			if (!IsAtMovementTarget()){
				nextState = UnitState.Moving;
			}else if (HasValidTargetInRange()){
				nextState = UnitState.Attacking;
			}

			yield return new WaitForEndOfFrame();
		}

		yield return StartCoroutine("ExitState");
	}

	IEnumerator Moving () {
		yield return StartCoroutine("EnterState");

		while (nextState == UnitState.Invalid){
		// Execute State
			if (IsAtMovementTarget()){
				nextState = UnitState.Idle;
			}
			
			MoveToTarget();
			yield return new WaitForEndOfFrame();
		}

		yield return StartCoroutine("ExitState");
	}


	IEnumerator Attacking () {
		yield return null;
	}

	IEnumerator EnterState () {
		Debug.Log("Entering State:" + state.ToString(), this);
		yield return null;
	}

	IEnumerator ExitState () {
		Debug.Log("Exiting State:" + state.ToString(), this);
		previousState = state;
		state = nextState;
		nextState = UnitState.Invalid;
		yield return null;
	}


	// Helper Functions

	private void RandomizeOffset (){
		offsetFromBattalion = new Vector3(Random.value * MAX_OFFSET_FROM_BATTALION,
			0,
			Random.value * MAX_OFFSET_FROM_BATTALION);
	}

	private bool IsAtMovementTarget () {
		Vector3 targetPosition = battalion.MovementTarget();
		if (Vector3.Distance(transform.position, targetPosition) <= 2.0){
			return true;
		}
		return false;
	}

	private void MoveToTarget () {
		Vector3 targetPosition = battalion.MovementTarget();
		transform.position = Vector3.MoveTowards(transform.position,
			targetPosition,
			Time.deltaTime * speed);

		Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position) * defaultRotation;
		transform.rotation = Quaternion.Lerp(transform.rotation,
			targetRotation,
			Time.deltaTime * turningSpeed);

		return;
	}

	private bool HasValidTargetInRange () {
		return false;
	}

	private void Die () {
		// TODO: Play Death Anim
		battalion.Remove(this);
	}

}
