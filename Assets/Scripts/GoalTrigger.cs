using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalTrigger : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        if (GameManager.Instance.IsProcessing) {
            return;
        }
        
        if (other.CompareTag(Globals.LIBERATED_TAG)) {
            Unit _unit = other.GetComponent<Unit>();
            if (_unit) {
                GameManager.Instance.SetIsProcessing(true);
                gameObject.SetActive(false);
            }
        }
    }
}
