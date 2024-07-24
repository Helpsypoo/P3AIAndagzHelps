using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotator : MonoBehaviour {

    [Tooltip("How quickly the object rotates")]
    [SerializeField] private float _speed = 2f;
    [SerializeField] private RotationDirection _direction;
    [SerializeField] private bool _autoRotate;

    private Vector3 _rotationDelta;

    private void Update() {

        AutoRotate();

    }

    public void Rotate(RotationDirection direction, float speed) {

        switch (direction) {
            case RotationDirection.YawRight:
                _rotationDelta = Vector3.up;
                break;
            case RotationDirection.YawLeft:
                _rotationDelta = -Vector3.up;
                break;
            case RotationDirection.PitchUp:
                _rotationDelta = -Vector3.right;
                break;
            case RotationDirection.PitchDown:
                _rotationDelta = Vector3.right;
                break;
            case RotationDirection.RollLeft:
                _rotationDelta = Vector3.forward;
                break;
            case RotationDirection.RollRight:
                _rotationDelta = -Vector3.forward;
                break;
            default:
                _rotationDelta = Vector3.up;
                break;
        }

        _rotationDelta *= speed;
        transform.Rotate(_rotationDelta * Time.deltaTime);

    }

    private void AutoRotate() {
        if (!_autoRotate) return;
        Vector3 dir;

        switch (_direction) {
            case RotationDirection.YawRight:
                dir = Vector3.up;
                break;
            case RotationDirection.YawLeft:
                dir = -Vector3.up;
                break;
            case RotationDirection.PitchUp:
                dir = -Vector3.right;
                break;
            case RotationDirection.PitchDown:
                dir = Vector3.right;
                break;
            case RotationDirection.RollLeft:
                dir = Vector3.forward;
                break;
            case RotationDirection.RollRight:
                dir = -Vector3.forward;
                break;
            default:
                dir = Vector3.up;
                break;
        }

            transform.Rotate(dir * _speed * Time.deltaTime);
    }

}

public enum RotationDirection {

    YawRight,
    YawLeft,
    PitchUp,
    PitchDown,
    RollLeft,
    RollRight

}
