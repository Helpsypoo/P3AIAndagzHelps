using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(Globals.LIBERATED_TAG)) {
            Unit _unit = other.GetComponent<Unit>();
            if (_unit) {
                GameManager.Instance.LiberatedScore++;
                _unit.Die();
            }
        }
    }

    private void OnTriggerStay(Collider other) {
        OnTriggerEnter(other);
    }
}
