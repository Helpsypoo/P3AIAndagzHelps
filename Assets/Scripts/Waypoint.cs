using UnityEngine;

public class Waypoint : MonoBehaviour {
	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Liberated")) {
			Liberated _liberated = other.GetComponentInParent<Liberated>();
			if (!_liberated || !_liberated.IsLeader) {
				return;
			}

			Remove();

		}
	}

	/// <summary>
	/// Removes this waypoint from the waypoints list and destroys the waypoint in the scene.
	/// </summary>
	public void Remove() {
        GameManager.Instance.RemoveWaypoint(this);
        Destroy(gameObject);
    }

}