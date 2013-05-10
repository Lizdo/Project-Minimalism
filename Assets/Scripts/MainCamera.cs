using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour {

	public float height;
	private float targetHeight;
	private float minHeight = 30.0f;

	private Vector3 targetPosition;

	private GameObject floor;
	private GameLogic gameLogic;
	private Battalion battalion;
	
	void Awake (){
		floor = GameObject.Find("Floor");
		gameLogic = GetComponent<GameLogic>();
		battalion = gameLogic.playerBattalion;
		height = minHeight;
	}

	void Update () {
		UpdatePosition();
		UpdateHeight();
		UpdateFloorPosition();
	}

	private float maxCameraPredictionOffsetRatio = 0.4f;
	private float positionUpdateStep = 50.0f;

	private void UpdatePosition () {
		Bounds battalionBounds = battalion.bounds;
		float sizeInCameraView = (new Vector2(battalionBounds.size.x, battalionBounds.size.z)).magnitude;

		Vector3 battalionPosition = battalion.transform.position;
		Vector3 nextEncounterPosition = gameLogic.CurrentEncounterPosition();

		float distanceToNextEncounter = Vector3.Distance(battalionPosition, nextEncounterPosition);
		float cameraPredictionRatio = Mathf.Clamp01(distanceToNextEncounter/sizeInCameraView);

		Vector3 predictionOffset = (nextEncounterPosition - battalionPosition).normalized * cameraPredictionRatio * maxCameraPredictionOffsetRatio * sizeInCameraView;

		targetPosition = battalionPosition + new Vector3(0, height, 0) + predictionOffset;

		transform.position = Vector3.MoveTowards(transform.position, targetPosition, positionUpdateStep * Time.deltaTime);
	}

	private float heightUpdateRatio = 1.0f;
	private float heightRatio = 2.0f;

	private void UpdateHeight () {
		Bounds battalionBounds = battalion.bounds;
		float sizeInCameraView = (new Vector2(battalionBounds.size.x, battalionBounds.size.z)).magnitude;

		targetHeight = Mathf.Max(sizeInCameraView*heightRatio, minHeight);

		height = Mathf.Lerp(height, targetHeight, Time.deltaTime * heightUpdateRatio);
	}


	private void UpdateFloorPosition () {
		floor.transform.localPosition = new Vector3(0, 0, height);		
	}

}
