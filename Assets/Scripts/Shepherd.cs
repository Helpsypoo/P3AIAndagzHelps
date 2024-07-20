using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shepherd : Unit {

    [SerializeField] private GameObject _wayPointPrefab;

    private Vector3? _wayPointPlacement;
    private Waypoint _wayPointToRemove;

    public override void Update() {
        base.Update();

        // If we don't have a waypoint destination, we don't need to check to see if we can place one.
        if (_wayPointPlacement != null) CheckWayPointPlacement();
        // If we don't have a waypoint to remove, we don't need to check that either.
        else if (_wayPointToRemove != null) CheckWayPointToRemove();

    }

    public override void PerformAction(Vector3 position, Transform target = null) {
        base.PerformAction(position, target);

        // If we have not been given a target, our default action is to place a beacon/waypoint at the given location if we have enough.
        if (target == null) {
            if (SquadManager.Instance.WaypointStash > 0) {
                _wayPointPlacement = position;
                MoveTo(_wayPointPlacement.Value);
            }
        } else if (target.CompareTag(Globals.WAYPOINT_TAG)) {
            _wayPointToRemove = target.GetComponent<Waypoint>();
            if (_wayPointToRemove != null) {
                MoveTo(_wayPointToRemove.transform.position);
            }
        }

        
    }

    /// <summary>
    /// Function for checking if we are currently placing a waypoint and handling the placement if we are.
    /// </summary>
    private void CheckWayPointPlacement() {

        // If we're not there yet, we don't need to do anything here.
        if (Vector3.Distance(transform.position, _wayPointPlacement.Value) > Globals.MIN_ACTION_DIST) return;

        // If we get to here, we have a waypoint to place and we are close enough to place it. So place it.
        GameObject newBeacon = Instantiate(_wayPointPrefab, _wayPointPlacement.Value, Quaternion.identity);
        Waypoint waypoint = newBeacon.GetComponent<Waypoint>();
        GameManager.Instance.AddWaypoint(waypoint);
        _wayPointPlacement = null;
        GameManager.Instance.SelectionMarker.Deactivate();
        SquadManager.Instance.DecrementWaypoints();

    }

    private void CheckWayPointToRemove() {

        // If we're not there yet, we don't need to do anything here.
        if (Vector3.Distance(transform.position, _wayPointToRemove.transform.position) > Globals.MIN_ACTION_DIST) return;

        // Once we're there, remove the waypoint.
        GameManager.Instance.RemoveWaypoint(_wayPointToRemove);
        GameManager.Instance.SelectionMarker.Deactivate();
        SquadManager.Instance.IncrementWaypoints();

    }

}
