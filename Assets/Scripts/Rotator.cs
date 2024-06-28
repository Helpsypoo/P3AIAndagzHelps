using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {

    [Header("Platform Rotation Values")]
    [Tooltip("How quickly the platform ramps up in rotational velocity.")]
    [SerializeField] private float _speed = 2f;

    [Tooltip("The maximum speed the platform will rotate by.")]
    [SerializeField] private float _maxSpeed = 1f;

    private float _ramp;
    private float _input;

    private void Update() {

        GetInput();
        Rotate();

    }

    /// <summary>
    /// Captures player input, modifies _ramp value and validates the result.
    /// </summary>
    private void GetInput() {

        _input = Input.GetAxis("Horizontal");

        if (_input != 0) {
            _ramp += _input * Time.deltaTime * _speed;
        } else {
            _ramp = Mathf.MoveTowards(_ramp, 0f, Time.deltaTime * _speed);
        }

        _ramp = Mathf.Clamp(_ramp, -_maxSpeed, _maxSpeed);

    }

    /// <summary>
    /// Rotates the platform around the Up axis using _ramp as the amount.
    /// </summary>
    private void Rotate() {

        transform.Rotate(Vector3.up * _ramp);

    }

}
