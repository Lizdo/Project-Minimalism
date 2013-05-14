using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EncounterUnit : Encounter {

	private List <Unit> units = new List <Unit> ();
	public Unit playerUnit;
	
	protected override void Awake () {
		base.Awake();
		type = EncounterType.Unit;
	}

	protected override void Start () {
		base.Start();
		SpawnUnits();
	}

	protected override void Update () {
		base.Update();
		if (!isResolved)
			isResolved = AllUnitsAssigned();
	}


	private void SpawnUnits () {
		for (int i = 0; i < round/3 + 1; i++){
			Unit u = Instantiate(playerUnit) as Unit;
			u.AddToEncounter(this);
			units.Add(u);
		}
	}

	private bool AllUnitsAssigned () {
		foreach (Unit u in units){
			if (!u.isAssigned){
				return false;
			}
		}
		return true;
	}


}
