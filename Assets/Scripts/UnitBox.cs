using UnityEngine;
using System.Collections;

public class UnitBox : Unit {

	protected override void Awake () {
		cost = 50;
		maxHealth = 50;
		attackDamage = 60;
		attackSpeed = 0.1f;
		type = UnitType.Box;

		base.Awake();
	}

	public override string Description () {
		return base.Description() + "Basic unit";
	}

}
