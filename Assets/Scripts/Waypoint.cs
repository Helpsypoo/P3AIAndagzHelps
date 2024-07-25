using UnityEngine;

public class Waypoint : MonoBehaviour {

    private TickEntity _tickEntity;

    [SerializeField] private LineRenderer _line;

    private void Awake() {
        _tickEntity = GetComponent<TickEntity>();
    }

    private void Start() {
        TickEventManager.Instance.AddTickEntity(_tickEntity);
    }

    public bool IsNextWayPoint() {

        if (GameManager.Instance.ActiveWaypoints.Count < 1) {
            throw new System.Exception("Waypoint exists in world but not in ActiveWaypoints list. Thass not right.");
        }

        Waypoint waypoint = GameManager.Instance.ActiveWaypoints[0];
        if (waypoint != this) return false;
        return true;

    }

    public void DistanceCheck() {

        if (GameManager.Instance.ActiveLiberated.Count < 1) return;

        if (!IsNextWayPoint()) return;

        Vector3 leadLiberatedPos = GameManager.Instance.ActiveLiberated[0].transform.position;

        float sqrThreshold = Globals.MIN_ACTION_DIST * Globals.MIN_ACTION_DIST;
        if ((leadLiberatedPos - transform.position).sqrMagnitude <= sqrThreshold) {
            Remove();
        }

    }

    public void UpdateLine(int index) {

        // Makes sure we are not the next waypoint for collection (ie, not 0 in the list)
        if (IsNextWayPoint()) {
            _line.gameObject.SetActive(false);
            return;
        }

        Vector3 linkedPosition = GameManager.Instance.ActiveWaypoints[index - 1].transform.position;

        _line.SetPosition(0, transform.position);
        _line.SetPosition(1, linkedPosition);
        _line.gameObject.SetActive(true);

    }

	/// <summary>
	/// Removes this waypoint from the waypoints list and destroys the waypoint in the scene.
	/// </summary>
	public void Remove() {
        GameManager.Instance.RemoveWaypoint(this);
        Destroy(gameObject);
    }

}