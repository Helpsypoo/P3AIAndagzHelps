using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionMarker : MonoBehaviour {

    [field: SerializeField] public Mission Details { get; private set; }

    private void Awake() {
        if (Details == null) {
            Debug.LogWarning($"Mission Marker, {transform.name}, did not have a mission attached. Destroying marker.");
            Destroy(gameObject);
        }
    }

    private void Start() {
        PositionSelf();
    }


    private void PositionSelf() {

        if (Physics.Raycast(transform.position, -transform.forward * 5f, out RaycastHit hit)) {

            transform.position = hit.point;
            transform.forward = hit.normal;

        }
    
    }
}
