using UnityEngine;

public class KillZone : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        Unit _unit = other.GetComponent<Unit>();
        if (!_unit) {
            return;
        }
        
        Liberated _liberated = other.GetComponent<Liberated>();
        if (_liberated) {
            GameManager.Instance.ProcessLiberatedScore(_liberated);
        }
        
        _unit.Die();
    }

    private void OnTriggerStay(Collider other) {
        OnTriggerEnter(other);
    }
}
