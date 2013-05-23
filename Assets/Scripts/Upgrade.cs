using UnityEngine;
using System.Collections;

public class Upgrade {
	public string id;
	public string description;
	public int idX;
	public int idY;
	public bool unlocked;
	public int cost = 200;

	public int numberOfChildUpgrades;

	public Upgrade prequisite;

	public Upgrade (string _id, string _description) {
		id = _id;
		description = _description;
	}

	public bool IsAvailableToUnlock () {
		if (unlocked){
			return false;
		}

		if (prequisite != null && !prequisite.unlocked){
			return false;
		}

		return true;
	}

	public string CostDescription () {
		return "Cost: " + cost.ToString();
	}
}