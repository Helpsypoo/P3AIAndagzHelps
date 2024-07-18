using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour {

    public UnitType Type;
    private NavMeshAgent _navAgent;


    [SerializeField] GameObject _selectionIndicator;

    private void Awake() {
        
        _navAgent = GetComponent<NavMeshAgent>();
        if (_navAgent == null) {
            throw new System.Exception($"Cannot find NavMeshAgent on unit {transform.name}.");
        }

    }

    private void Update() {
        LookWhereYoureGoing();
    }

    /// <summary>
    /// If we have path, turn the unit to face the direction they are going.
    /// </summary>
    private void LookWhereYoureGoing() {
        if (_navAgent.hasPath) {
            Vector3 lookTarget = _navAgent.nextPosition;
            lookTarget.y = transform.position.y;
            transform.LookAt(lookTarget);
        }
    }

    /// <summary>
    /// Sends unit to a new destination.
    /// </summary>
    /// <param name="destination">The location the unit will attempt to move to.</param>
    public void MoveTo(Vector3 destination) {
        _navAgent.SetDestination(destination);
    }

    /// <summary>
    /// Called when this unit is selected by the player.
    /// </summary>
    public void Select() {
        _selectionIndicator.SetActive(true);
    }

    /// <summary>
    /// Called when this unit becomes deselected.
    /// </summary>
    public void Deselect() {
        _selectionIndicator?.SetActive(false);
    }

}


