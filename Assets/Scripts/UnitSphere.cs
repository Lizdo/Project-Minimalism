using UnityEngine;
using System.Collections;

public class UnitSphere : Unit {

	protected override void Awake () {
		cost = 500;
		maxHealth = 500;
		attackDamage = 80;
		attackSpeed = 0.1f;
		type = UnitType.Sphere;

		base.Awake();
	}

	public override string Description () {
		return base.Description() + " Advanced unit";
	}
}
