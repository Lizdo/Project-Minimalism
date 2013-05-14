using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class EncounterResource : Encounter {

	private List <Mine> mines = new List <Mine> ();
	public Mine mine;
	
	protected override void Awake () {
		base.Awake();
		type = EncounterType.Resource;
	}

	protected override void Start () {
		base.Start();
		SpawnMines();
	}

	protected override void Update () {
		base.Update();
		if (!isResolved)
			isResolved = AllMinesDepleted();
	}



	private void SpawnMines () {
		for (int i = 0; i < round/2; i++){
			Mine m = Instantiate(mine) as Mine;
			mines.Add(m);
			m.SetEncounter(this);
		}
	}

	private bool AllMinesDepleted () {
		foreach (Mine m in mines){
			if (!m.IsDepleted()){
				return false;
			}
		}
		return true;
	}

	public override GameObject TargetInRange (Unit u){
		foreach (Mine m in mines){
			if (m.IsAvailableToUnit(u)){
				return m.gameObject;
			}
		}
		return null;
	}

}