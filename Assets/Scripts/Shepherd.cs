using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shepherd : Unit {

    [SerializeField] private GameObject _wayPointPrefab;

    private Vector3? _wayPointDestination;

    public override void Update() {
        base.Update();

        CheckWayPointPlacement();

    }

    public override void PerformAction(Vector3 position, Transform target = null) {
        base.PerformAction(position, target);

        // If we have not been given a target, our default action is to place a beacon/waypoint at the given location.
        if (target == null) {
            _wayPointDestination = position;
            MoveTo(_wayPointDestination.Value);
        }

        
    }

    /// <summary>
    /// Function for checking if we are currently placing a waypoint and handling the placement if we are.
    /// </summary>
    private void CheckWayPointPlacement() {

        // If we don't have a waypoint destination, we don't need to anything else here.
        if (_wayPointDestination == null) return;

        // If we have a waypoint destination but we're not there yet, we don't need to do anything here.
        if (Vector3.Distance(transform.position, _wayPointDestination.Value) > Globals.MIN_ACTION_DIST) return;

        // If we get to here, we have a waypoint to place and we are close enough to place it. So place it.
        GameObject newBeacon = Instantiate(_wayPointPrefab, _wayPointDestination.Value, Quaternion.identity);
        Waypoint waypoint = newBeacon.GetComponent<Waypoint>();
        GameManager.Instance.AddWaypoint(waypoint);
        _wayPointDestination = null;

    }

}
