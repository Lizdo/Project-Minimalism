using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour {

	public bool canBeUsed;
	private bool inView;
	private Quaternion defaultRotation = Quaternion.Euler(270,0,0);

	// Use this for initialization
	void Awake () {
		canBeUsed = true;
		renderer.enabled = false;			
	}

	void OnBecameVisible() {
		inView = true;
	}
	
	void OnBecameInvisible () {
		if (inView){
			canBeUsed = true;
		}
	}

	public void UseInPosition(Vector3 p){
		Debug.Log("Spawned!!");
		transform.position = p;
		canBeUsed = false;
		renderer.enabled = true;

		Quaternion randomRotation = Quaternion.Euler(0, Random.value * 360, 0);
		transform.rotation = randomRotation * defaultRotation;
	}

}
