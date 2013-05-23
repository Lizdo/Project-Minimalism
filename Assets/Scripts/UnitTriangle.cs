using UnityEngine;
using System.Collections;

public class UnitTriangle : Unit {

	protected override void Awake () {
		cost = 100;
		maxHealth = 80;
		attackDamage = 100;
		attackSpeed = 0.1f;
		type = UnitType.Triangle;

		base.Awake();
	}

	public override string Description () {
		return base.Description() + "Intermediate unit";
	}

}
