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

        // Update the number of waypoints we have remaining.
        _abilityCharges = SquadManager.Instance.WaypointStash;

    }

    public override void PerformAction(Vector3 position, Transform target = null) {
        base.PerformAction(position, target);

        // Whatever happens with the action, attempting to perform it will cancel any follow command.
        ClearFollowTarget();

        // If we have not been given a target, our default action is to place a beacon/waypoint at the given location if we have enough.
        if (target == null) {
            if (SquadManager.Instance.WaypointStash > 0) {
                _wayPointPlacement = position;
                SetStopDistance(UnitStats.ActionRange);
                MoveTo(_wayPointPlacement.Value);
                GameManager.Instance.MoveArrow.Play(GameManager.Instance.SelectionMarker.Position, GameManager.Instance.SelectionMarker.transform.up);
                Anim?.SetTrigger(actionTrigger);
            } else {
                GameManager.Instance.SelectionMarker.InvalidAction();
            }
        } else if ((target.CompareTag(Globals.UNIT_TAG) || target.CompareTag(Globals.LIBERATED_TAG))) {
            _wayPointPlacement = target.position;
            SetStopDistance(UnitStats.ActionRange);
            MoveTo(_wayPointPlacement.Value);

        } else if (target.CompareTag(Globals.WAYPOINT_TAG)) {
            _wayPointToRemove = target.GetComponent<Waypoint>();
            if (_wayPointToRemove != null) {
                MoveTo(_wayPointToRemove.transform.position);
                SetStopDistance(UnitStats.ActionRange);
                GameManager.Instance.MoveArrow.Play(GameManager.Instance.SelectionMarker.Position, GameManager.Instance.SelectionMarker.transform.up);
            }
        }
        GameManager.Instance.SelectionMarker.Activate(this);
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

    public void PlaceWayPoint() {
        GameObject newBeacon = Instantiate(_wayPointPrefab, _wayPointPlacement.Value, Quaternion.identity);
        Waypoint waypoint = newBeacon.GetComponent<Waypoint>();
        GameManager.Instance.AddWaypoint(waypoint);
        _wayPointPlacement = null;
        GameManager.Instance.SelectionMarker.Deactivate();
        SquadManager.Instance.DecrementWaypoints();
    }

    public void RemoveWayPoint() {
        GameManager.Instance.RemoveWaypoint(_wayPointToRemove);
        GameManager.Instance.SelectionMarker.Deactivate();
    }

    private void CheckWayPointToRemove() {

        // If we're not there yet, we don't need to do anything here.
        if (Vector3.Distance(transform.position, _wayPointToRemove.transform.position) > Globals.MIN_ACTION_DIST) return;

        // Once we're there, remove the waypoint.
        //GameManager.Instance.RemoveWaypoint(_wayPointToRemove);
        //GameManager.Instance.SelectionMarker.Deactivate();

    }

}
