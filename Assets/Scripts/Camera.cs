using UnityEngine;
using System.Collections;

public class Camera : MonoBehaviour {


	public float cameraHeight = 30;

	private GameObject floor;
	private Battalion battalion;
	
	void Awake (){
		floor = GameObject.Find("Floor");
		battalion = gameObject.GetComponent<Battalion>();
	}

	void Start () {
		floor.transform.localPosition = new Vector3(0, 0, cameraHeight);
	}
	
	void Update () {
		UpdateCameraPosition();
	}


	private void UpdateCameraPosition() {
		Vector3 battalionPosition = battalion.Position();
		transform.position = battalionPosition + new Vector3(0, cameraHeight, 0);
	}
}
