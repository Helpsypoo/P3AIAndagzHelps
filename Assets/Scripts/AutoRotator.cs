using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotator : MonoBehaviour {

    [Tooltip("How quickly the object rotates")]
    [SerializeField] private float _speed = 2f;

    private void Update() {

        transform.Rotate(Vector3.up * _speed * Time.deltaTime);

    }

}
