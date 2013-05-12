using UnityEngine;
using System.Collections;

public class EncounterCombat : Encounter {
	
	protected override void Awake () {
		base.Awake();
		type = EncounterType.Combat;
	}

	protected override void Start () {
		base.Start();
		SpawnEnemies();
	}

	protected override void Update () {
		base.Update();
		if (!isResolved)
			isResolved = gameLogic.AllEnemiesDead();
	}


	private void SpawnEnemies () {
		for (int i = 0; i < round+1; i++){
			gameLogic.AddEnemyUnit();
		}
	}

	public override GameObject TargetInRange (Unit u){
		// TODO: Refactor this so that we have a reference of the enemy battalion
		return null;
	}
}
