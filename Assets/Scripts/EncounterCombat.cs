using UnityEngine;
using System.Collections;

public class EncounterCombat : Encounter {
	
	private bool inCombat;
	private float combatStartTime = 10000000.0f;

	[System.Serializable]
	public class SpawnInfo{
		public UnitType type;
		public int amount;
		public float delay;
		[HideInInspector]
		public bool spawned;
	}

	public SpawnInfo[] spawnInfos;

	protected override void Awake () {
		base.Awake();
		type = EncounterType.Combat;
		inCombat = false;
	}

	protected override void Start () {
		base.Start();
	}

	protected override void Update () {
		base.Update();

		if (isResolved){
			return;
		}
		
		if (!inCombat && gameLogic.EnemyInCombat()){
			combatStartTime = Time.time;
			inCombat = true;
		}

		UpdateSpawnInfo();

		isResolved = AllUnitsSpawned() && gameLogic.AllEnemiesDead();
	}


	private bool AllUnitsSpawned () {
		foreach (SpawnInfo s in spawnInfos){
			if (!s.spawned){
				return false;
			}
		}
		return true;
	}

	private void UpdateSpawnInfo () {
		foreach (SpawnInfo s in spawnInfos){
			if (s.spawned){
				continue;
			}

			if (s.delay == 0){
				SpawnWave(s);
			}else if (Time.time - combatStartTime >= s.delay){
				SpawnWave(s);
			}
		}
	}

	private void SpawnWave (SpawnInfo s){
		for (int i = 0; i < s.amount; i++){
			gameLogic.AddEnemyUnit(s.type);
		}
		s.spawned = true;
	}

	public override GameObject TargetInRange (Unit u){
		// TODO: Refactor this so that we have a reference of the enemy battalion
		return null;
	}
}
