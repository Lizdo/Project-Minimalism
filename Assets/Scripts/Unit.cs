using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {

	// High level behavior is controlled by Battalion, Unit will take care of low level behavior like movement, attack...etc

	public enum UnitState{
		Invalid,
		Idle,
		Moving,
		Attacking,
		Dead
	}

	public bool isEnemy;

	public UnitState state = UnitState.Idle;
	public UnitState previousState;
	public UnitState nextState = UnitState.Invalid;

	public float speed = 30.0f;
	public float turningSpeed = 5.0f;
	public int size = 1;
	public float attackingDistance = 30.0f;

	public Unit enemy = null;


	private Battalion battalion;
	private Vector3 offsetFromBattalion;

	private const float MAX_OFFSET_FROM_BATTALION = 10.0f;
	private const float MOVEMENT_TOLERANCE = 5.0f;

	private Quaternion defaultRotation = Quaternion.Euler(270,0,0);


	// Public interface
	public void AddToBattalion (Battalion b) {
		battalion = b;
		isEnemy = b.isEnemy;
		Vector3 battalionPositionOnFloor = new Vector3(battalion.transform.position.x,
			0,
			battalion.transform.position.z);

		transform.position = battalionPositionOnFloor + offsetFromBattalion;
		battalion.Add(this);
		StartCoroutine(FSM());		
	}

	public float DistanceToUnit (Unit u) {
		return Vector3.Distance(transform.position, u.transform.position);
	}


	// Core Updates

	private void Awake (){
		RandomizeOffset();
		RandomizeRotation();
	}

	private void Start () {		
	}

	private void RandomizeOffset (){
		offsetFromBattalion = new Vector3(Random.value * MAX_OFFSET_FROM_BATTALION,
			0,
			Random.value * MAX_OFFSET_FROM_BATTALION);
	}

	private void RandomizeRotation () {
		float randomRotation = Random.value * 360;
		transform.rotation = Quaternion.Euler(0, randomRotation, 0) * defaultRotation;
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
			}

			enemy = battalion.ValidTargetInRange(this);
			if (enemy){
				nextState = UnitState.Attacking;
			}

			yield return null;
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

			if (enemy){
				nextState = UnitState.Attacking;
			}
			
			MoveToBattalionMovementTarget();
			yield return null;
		}

		yield return StartCoroutine("ExitState");
	}


	IEnumerator Attacking () {
		yield return StartCoroutine("EnterState");

		while (nextState == UnitState.Invalid){
			// TODO: Play Attack Anim
			enemy.enemy = this;

			if (!NearEnemy()){
				MoveToEnemy();
			}else{
				AttackEnemy();
			}

			yield return null;
		}

		yield return StartCoroutine("ExitState");
	}


	IEnumerator Dead () {
		yield return StartCoroutine("EnterState");

		while (nextState == UnitState.Invalid){
			// TODO: Play Dead Anim
			renderer.material.color = Color.gray;
			Die();
			yield return new WaitForEndOfFrame();
		}

		yield return StartCoroutine("ExitState");
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

	private bool NearEnemy() {
		if (Vector3.Distance(transform.position, enemy.transform.position) < MOVEMENT_TOLERANCE){
			return true;
		}
		return false;
	}


	private bool IsAtMovementTarget () {
		if (isEnemy)
			return true;

		Vector3 targetPosition = battalion.MovementTarget();
		if (Vector3.Distance(transform.position, targetPosition) <= MOVEMENT_TOLERANCE){
			return true;
		}
		return false;
	}

	private void MoveToTarget (Vector3 targetPosition) {
		transform.position = Vector3.MoveTowards(transform.position,
			targetPosition,
			Time.deltaTime * speed);

		Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position) * defaultRotation;
		transform.rotation = Quaternion.Lerp(transform.rotation,
			targetRotation,
			Time.deltaTime * turningSpeed);

		return;
	}

	private void MoveToEnemy () {
		MoveToTarget(enemy.transform.position);
	}

	private void MoveToBattalionMovementTarget () {
		MoveToTarget(battalion.MovementTarget());
	}


	private void AttackEnemy () {
		if (enemy.size >= size){
			nextState = UnitState.Dead;
		}else{
			nextState = UnitState.Idle;
			enemy = null;
		}
	}

	private void Die () {
		battalion.Remove(this);
	}

}
