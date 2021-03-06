using UnityEngine;
using System.Collections;

public enum UnitType{
	Box = 0,
	Triangle = 1,
	Sphere = 2
}

public class Unit : MonoBehaviour {

	// High level behavior is controlled by Battalion, Unit will take care of low level behavior like movement, attack...etc

	public enum UnitState{
		Invalid,
		Idle,
		Moving,
		Attacking,
		Mining,
		Dead,
		PendingAssign
	}

	public bool isEnemy;
	[HideInInspector]
	public bool isAssigned;	// If the units belongs to a battalion

	public UnitState state = UnitState.Idle;
	public UnitState previousState;
	public UnitState nextState = UnitState.Invalid;

	public float speed = 30.0f;
	public float turningSpeed = 5.0f;
	public float size = 1.0f;
	public int cost = 50;
	public UnitType type;

	[HideInInspector]
	public float attackDistance = 30.0f;
	[HideInInspector]
	public float attackSpeed = 0.1f;	// Time Per Attack
	[HideInInspector]
	public float attackDamage = 60;

	[HideInInspector]
	public float joinDistance = 20.0f;
	[HideInInspector]
	public float maxHealth = 100;
	public float health;

	public bool isDead;


	// TODO: plug logic to calculate attack damage from upgrade level
	[HideInInspector]
	public int healthUpgradeLevel = 1;
	[HideInInspector]
	public int attackDamageUpgradeLevel = 1;

	public Unit enemy;
	public Mine mine;

	private Battalion battalion;
	private Vector3 spawnLocation;

	private Vector3 offsetFromSpawnLocation;

	private const float MAX_OFFSET_FROM_SPAWNLOCATION = 10.0f;
	private const float MOVEMENT_TOLERANCE = 3.0f;

	private Quaternion defaultRotation = Quaternion.Euler(270,0,0);



	private bool useBoid = true;

	// Used in boid, check if we really want to reach the target position
	private bool highPriorityMovementTarget;
	public Vector3 boidVelocity = Vector3.zero;
	private Vector3 lastBoidVelocity = Vector3.zero;

	private Color initialColor;

	// Public interface
	public void AddToBattalion (Battalion b) {
		AssignToBattalion(b);
		spawnLocation = new Vector3(battalion.transform.position.x,
			0,
			battalion.transform.position.z);
		SetInitLocation();
		StartCoroutine(FSM());		
	}

	public void AddToEncounter (Encounter e) {
		isEnemy = false;	// Free unit from encounter
		GameLogic gameLogic = FindObjectOfType(typeof(GameLogic)) as GameLogic;
		battalion = gameLogic.playerBattalion;
		spawnLocation = e.transform.position;
		SetInitLocation();
		state = UnitState.PendingAssign;
		StartCoroutine(FSM());
	}


	public float DistanceToUnit (Unit u) {
		return Vector3.Distance(transform.position, u.transform.position);
	}


	public void TakeDamage (float amount, Unit source) {
		Debug.Log("Damaging!", source);
		health -= amount;
		if (health <= 0){
			health = 0;
			Die();
		}
		UpdateHealthVisual();
	}

	public void Heal (float amount, Unit source) {
		health += amount;
		if (health >= maxHealth){
			health = maxHealth;
		}
		UpdateHealthVisual();	
	}

	public bool InCombat () {
		return state == UnitState.Attacking;
	}


	public virtual string CostDescription () {
		return "Cost: " + cost.ToString();
	}

	public virtual string Description () {
		return "";
	}




	///////////////////////////
	//  Core Updates
	///////////////////////////
		

	protected virtual void Awake () {
		RandomizeOffset();
		RandomizeRotation();
		health = maxHealth;
		initialColor = renderer.sharedMaterial.color;
	}

	protected virtual void Start () {
		Vector3 boundExtents = renderer.bounds.extents;
		Vector2 projectedTo2D = new Vector2(boundExtents.x, boundExtents.z);
		size = projectedTo2D.magnitude;
	}

	protected virtual void Update () {
		DebugHelper.DrawLine(transform.position,
			transform.position + new Vector3(size, 0, 0));

	}


	///////////////////////////
	// Coroutine based State Machine
	///////////////////////////

	IEnumerator FSM () {
	    // Execute the current coroutine (state)
	    while (true)
			yield return StartCoroutine(state.ToString());
	}

	IEnumerator Idle () {
		yield return StartCoroutine("EnterState");

		while (nextState == UnitState.Invalid){
		// Execute State
			RegenerateHealth();

			if (!IsAtMovementTarget()){
				nextState = UnitState.Moving;
			}

			AssignEnemy();
			mine = battalion.ValidMineInRange(this);
			if (mine){
				nextState = UnitState.Mining;
			}

			yield return null;
		}

		yield return StartCoroutine("ExitState");
	}

	IEnumerator Moving () {
		yield return StartCoroutine("EnterState");

		while (nextState == UnitState.Invalid){
		// Execute State
			RegenerateHealth();

			if (IsAtMovementTarget()){
				nextState = UnitState.Idle;
			}

			AssignEnemy();

			mine = battalion.ValidMineInRange(this);
			if (mine){
				nextState = UnitState.Mining;
			}
			
			MoveToBattalionMovementTarget();
			yield return null;
		}

		yield return StartCoroutine("ExitState");
	}


	IEnumerator Attacking () {
		yield return StartCoroutine("EnterState");
		timeTryingToMoveToTarget = 0.0f;

		while (nextState == UnitState.Invalid){

			if (!enemy || enemy.isDead){
				enemy = null;
				nextState = UnitState.Idle;
				continue;
			}

			if (isDead){
				nextState = UnitState.Dead;
				continue;
			}

			while (!NearTarget()){
				MoveToTarget();
				timeTryingToMoveToTarget += Time.deltaTime;
				if (!isEnemy && timeTryingToMoveToTarget >= movingToTargetTimeOut){
					// Only find new targets for friendly units
					enemy.enemy = null;
					enemy = null;
					nextState = UnitState.Idle;
					break;
				}				
				yield return null;
			}

			if (enemy && !enemy.isDead){
				// TODO: Play Attack Anim
				yield return new WaitForSeconds(attackSpeed);
				AttackEnemy();
			}

			yield return null;
			
		}

		yield return StartCoroutine("ExitState");
	}

	private float timeTryingToMoveToTarget;
	private float movingToTargetTimeOut = 2.0f;

	IEnumerator Mining () {
		yield return StartCoroutine("EnterState");
		timeTryingToMoveToTarget = 0.0f;

		while (nextState == UnitState.Invalid){
			mine.miner = this;

			while (!NearTarget()){
				MoveToTarget();
				timeTryingToMoveToTarget += Time.deltaTime;
				if (timeTryingToMoveToTarget >= movingToTargetTimeOut){
					mine.miner = null;
					mine = null;
					nextState = UnitState.Idle;
					break;
				}

				yield return null;
			}

			while (mine && !mine.IsDepleted()){
				Mine();
				yield return new WaitForSeconds(1.0f);
			}

			nextState = UnitState.Idle;
		}

		yield return StartCoroutine("ExitState");
	}	


	IEnumerator Dead () {
		yield return StartCoroutine("EnterState");
		while (true){
			yield return new WaitForSeconds(1.0f);
		}
	}

	IEnumerator PendingAssign () {
		yield return StartCoroutine("EnterState");

		while (nextState == UnitState.Invalid){
			// TODO: Play Dead Anim
			if (battalion.HasValidFriendlyInRange(this)){
				AssignToBattalion(battalion);
				nextState = UnitState.Idle;
			}
			yield return null;
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


	///////////////////////////
	// Helper Functions
	///////////////////////////

	private void RandomizeOffset () {
		offsetFromSpawnLocation = new Vector3(Random.value * MAX_OFFSET_FROM_SPAWNLOCATION,
			0,
			Random.value * MAX_OFFSET_FROM_SPAWNLOCATION);
	}

	private void RandomizeRotation () {
		float randomRotation = Random.value * 360;
		transform.rotation = Quaternion.Euler(0, randomRotation, 0) * defaultRotation;
	}

	private void SetInitLocation () {
		transform.position = spawnLocation + offsetFromSpawnLocation;
	}


	private GameObject Target () {
		if (state == UnitState.Attacking){
			return enemy.gameObject;
		}else if (state == UnitState.Mining){
			return mine.gameObject;
		}else{
			return null;
		}
	}

	private bool NearTarget () {
		GameObject target = Target();
		DebugHelper.Assert(target!=null);

		if (Vector3.Distance(transform.position, target.transform.position) < MOVEMENT_TOLERANCE){
			return true;
		}
		return false;
	}

	private void AssignToBattalion (Battalion b) {
		isEnemy = b.isEnemy;
		battalion = b;
		isAssigned = true;
		battalion.Add(this);
	}

	private bool IsAtMovementTarget () {
		if (isEnemy)
			return true;

		if (Vector3.Distance(transform.position, IdleMovementTarget()) <= MOVEMENT_TOLERANCE){
			return true;
		}
		return false;
	}

	private void MoveToTargetPosition (Vector3 targetPosition) {
		RotateTowardsTarget(targetPosition);

		DebugHelper.DrawCross(targetPosition);

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
		float tendToPlaceMultiplier = highPriorityMovementTarget ? 2.0f : 0.8f;

		Vector3 v1, v2, v3, v4;
		v1 = battalion.SeparationVelocity(this) * 0.7f; // Allow some separation.
		v2 = battalion.AlignmentVelocity(this);
		v3 = battalion.CohesionVelocity(this) * 2.0f;
		v4 = battalion.TendToPlace(this, targetPosition) * tendToPlaceMultiplier;

		// Reduce the Separation Velocity to reduce the flickering
		boidVelocity = boidVelocity + v1 + v2 + v3 + v4;

		limitBoidVelocityToSpeed();
		SmoothBoidVelocity();

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

	private void SmoothBoidVelocity () {
		boidVelocity = (boidVelocity + lastBoidVelocity) / 2;
		lastBoidVelocity = boidVelocity;
	}

	private void MoveToTarget() {
		GameObject target = Target();
		DebugHelper.Assert(target!=null);

		highPriorityMovementTarget = true;
		MoveToTargetPosition(target.transform.position);
	}

	private void MoveToEnemy () {
		highPriorityMovementTarget = true;
		MoveToTargetPosition(enemy.transform.position);
	}

	private void MoveToBattalionMovementTarget () {
		highPriorityMovementTarget = false;
		MoveToTargetPosition(IdleMovementTarget());
	}

	private Vector3 IdleMovementTarget () {
		Vector3 offsetFromBattalion = transform.position - battalion.bounds.center;
		return battalion.MovementTarget() + offsetFromBattalion;
	}

	private void AttackEnemy () {
		enemy.TakeDamage(attackDamage * attackSpeed, this);
		if (enemy.isDead && !isDead){
			nextState = UnitState.Idle;
			enemy = null;
		}
	}

	private void AssignEnemy () {
		if (!enemy){
			enemy = battalion.ValidTargetInRange(this);
		}			

		if (enemy){
			enemy.enemy = this;
			nextState = UnitState.Attacking;
		}
	}

	private void Mine() {
		// TODO: Play Mine Anim
		mine.Mined();
	}

	private void UpdateHealthVisual () {
		renderer.material.color = Color.Lerp(initialColor, Color.white, 1-health/maxHealth);
	}
	
	private float healthRegenRatioPerSec = 0.1f; // 10% per sec

	private void RegenerateHealth () {
		// No regen for Enemies
		if (isEnemy){
			return;
		}

		float HPtoRegen = healthRegenRatioPerSec * maxHealth * Time.deltaTime;
		Heal(HPtoRegen, this);
	}



	private void Die () {
		if (isDead){
			Debug.Log("Already Dead.....", this);
			return;
		}

		Debug.Log("Unit Dead.....", this);
		isDead = true;
		nextState = UnitState.Dead;
		battalion.Remove(this);
		StartCoroutine("DestroyGO");
	}

	IEnumerator DestroyGO (){
		yield return new WaitForSeconds(3.0f);
		Destroy(gameObject);
	}

}
