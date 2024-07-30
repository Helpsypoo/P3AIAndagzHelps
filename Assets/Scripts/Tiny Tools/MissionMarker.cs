using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MissionMarker : MonoBehaviour {

    public enum State
    {
        Available,
        Locked,
        Completed
    }

    [field: SerializeField] public Mission Details { get; private set; }
    [SerializeField] private GameObject _availableMarker;
    [SerializeField] private GameObject _completedMarker;
    [SerializeField] private GameObject _lockedMarker;

    private void Awake() {
        if (Details == null) {
            Debug.LogWarning($"Mission Marker, {transform.name}, did not have a mission attached. Destroying marker.");
            Destroy(gameObject);
        }
    }

    private void Start() {
        PositionSelf();

        switch (Details.Condition.ToMissionMarkerState())
        {
            case State.Available:
                _availableMarker.SetActive(true);
                _completedMarker.SetActive(false);
                _lockedMarker.SetActive(false);
                break;
            case State.Locked:
                _availableMarker.SetActive(false);
                _completedMarker.SetActive(false);
                _lockedMarker.SetActive(true);
                break;
            case State.Completed:
                _availableMarker.SetActive(false);
                _completedMarker.SetActive(true);
                _lockedMarker.SetActive(false);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    private void PositionSelf() {

        if (Physics.Raycast(transform.position, -transform.forward * 5f, out RaycastHit hit)) {

            transform.position = hit.point;
            transform.forward = hit.normal;

        }

    }
}
