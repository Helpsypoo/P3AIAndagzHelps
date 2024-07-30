using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnUnitEnter : MonoBehaviour {

    [SerializeField] private string _tag;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(_tag)) {
            Destroy(gameObject);
        }

    }

}
