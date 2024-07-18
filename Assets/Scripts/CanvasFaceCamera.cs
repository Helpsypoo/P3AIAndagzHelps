using UnityEngine;

public class CanvasFaceCamera : MonoBehaviour {
	private Camera mainCam;
	private Canvas canvas;

	private void Awake() {
		mainCam = Camera.main;
		canvas = GetComponent<Canvas>();
		if (canvas && mainCam) {
			canvas.worldCamera = mainCam;
		}
	}

	private void Update() {
		if (!mainCam || !canvas) {
			enabled = false;
			return;
		}
		transform.LookAt(mainCam.transform, Vector3.up);
	}
}
