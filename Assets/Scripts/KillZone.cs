using UnityEngine;

public class KillZone : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        Unit _unit = other.GetComponent<Unit>();
        if (!_unit) {
            return;
        }
        
        _unit.Die();
        
        if (_unit.CompareTag(Globals.LIBERATED_TAG)) {
            GameManager.Instance.ProcessLiberatedScore();
        }
        

    }

    private void OnTriggerStay(Collider other) {
        OnTriggerEnter(other);
    }
}
