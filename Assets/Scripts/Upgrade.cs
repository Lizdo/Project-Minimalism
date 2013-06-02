using UnityEngine;
using System.Collections;

public class Upgrade : ScriptableObject {
	public string id;
	public string description;
	public int idX;
	public int idY;
	public bool unlocked;
	public int cost = 200;
	public int availableRound;

	public int numberOfChildUpgrades;

	public Upgrade prequisite;

	public GameLogic gameLogic;

	public Upgrade (string _id, string _description, int _availableRound) {
		id = _id;
		description = _description;
		availableRound = _availableRound;
	}

	public bool IsAvailableToUnlock () {
		if (unlocked){
			return false;
		}

		if (gameLogic.round < availableRound){
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