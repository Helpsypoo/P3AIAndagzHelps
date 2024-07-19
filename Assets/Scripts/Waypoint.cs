using UnityEngine;

public class Waypoint : MonoBehaviour {
	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Liberated")) {
			Liberated _liberated = other.GetComponentInParent<Liberated>();
			if (!_liberated || !_liberated.IsLeader) {
				return;
			}

			GameManager.Instance.RemoveWaypoint(this);
			Destroy(gameObject);
		}
	}

}