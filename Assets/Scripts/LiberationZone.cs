using UnityEngine;

public class LiberationZone : MonoBehaviour {

    private Liberated _self;

    private void Awake() {
        _self = GetComponentInParent<Liberated>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(Globals.UNIT_TAG)) {
            _self.FreeGroup();
        }
    }

}
