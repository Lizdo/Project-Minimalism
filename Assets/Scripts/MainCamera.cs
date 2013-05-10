using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour {


	public float cameraHeight = 30.0f;

	private GameObject floor;
	private GameLogic gameLogic;
	private Battalion battalion;
	
	void Awake (){
		floor = GameObject.Find("Floor");
		gameLogic = GetComponent<GameLogic>();
		battalion = gameLogic.playerBattalion;
	}

	void Start () {
		floor.transform.localPosition = new Vector3(0, 0, cameraHeight);
	}
	
	void Update () {
		UpdateCameraPosition();
	}

	private void UpdateCameraPosition() {
		Vector3 battalionPosition = battalion.transform.position;
		transform.position = battalionPosition + new Vector3(0, cameraHeight, 0);
	}
}
