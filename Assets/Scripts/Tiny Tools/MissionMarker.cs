using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionMarker : MonoBehaviour {

    [field: SerializeField] public string SceneName { get; private set; }

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
