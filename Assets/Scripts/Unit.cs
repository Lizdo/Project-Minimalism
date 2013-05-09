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


	private bool useBoid = true;

	public Vector3 boidVelocity = Vector3.zero;

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
			enemy.enemy = this;

			while (!NearEnemy()){
				Debug.Log("Moving!");
				MoveToEnemy();
				yield return null;
			}

			// TODO: Play Attack Anim
			yield return new WaitForSeconds(0.3f);
			AttackEnemy();
		}

		yield return StartCoroutine("ExitState");
	}


	IEnumerator Dead () {
		yield return StartCoroutine("EnterState");

		while (nextState == UnitState.Invalid){
			// TODO: Play Dead Anim
			renderer.material.color = Color.gray;
			Die();
			yield return new WaitForSeconds(5.0f);
			Destroy(gameObject);
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
		RotateTowardsTarget(targetPosition);

		if (useBoid){
			MoveToTargetUsingBoid(targetPosition);
			return;
		}

		transform.position = Vector3.MoveTowards(transform.position,
			targetPosition,
			Time.deltaTime * speed);

		return;
	}

	private void MoveToTargetUsingBoid (Vector3 targetPosition) {
		Vector3 v1, v2, v3, v4;
		v1 = battalion.SeparationVelocity(this);
		v2 = battalion.AlignmentVelocity(this);
		v3 = battalion.CohesionVelocity(this);
		v4 = battalion.TendToPlace(this, targetPosition);

		// Reduce the Separation Velocity to reduce the flickering
		boidVelocity = boidVelocity + v1 * 0.2f + v2 + v3 + v4;

		limitBoidVelocityToSpeed();

		transform.position += boidVelocity;
	}


	private void RotateTowardsTarget (Vector3 targetPosition){
		Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position) * defaultRotation;
		transform.rotation = Quaternion.Lerp(transform.rotation,
			targetRotation,
			Time.deltaTime * turningSpeed);
	}

	private void limitBoidVelocityToSpeed () {
		if (boidVelocity.magnitude >= speed * Time.deltaTime){
			boidVelocity = boidVelocity.normalized * speed * Time.deltaTime;
		}
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
