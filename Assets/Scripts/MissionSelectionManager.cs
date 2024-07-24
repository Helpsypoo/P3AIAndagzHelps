using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MissionSelectionManager : MonoBehaviour {

    private BaseInput _input;
    private Camera _camera;

    [SerializeField] private AutoRotator _planet;
    private float _rotateSpeed;
    [SerializeField] private float _accelerationSpeed = 0.5f;
    [SerializeField] private float _maxSpeed = 100f;

    private bool _mouseDown;
    private bool _mouseOverMission = false;
    private Vector2 _mousePosition;
    private Vector2 _prevMousePosition;
    private Vector2 _mouseVelocity => _prevMousePosition - _mousePosition;
    public Vector2 MouseVelocity;
    [SerializeField] private float _clickLength = 0.25f;
    private float _clickTimer = 0f;

    private void Awake() {
        _input = new BaseInput();
        _camera = Camera.main;
    }

    private void OnEnable() {
        _input.Player.SelectClick.performed += MouseDown;
        _input.Player.SelectClick.canceled += MouseUp;
        _input.Player.Enable();
    }

    private void OnDisable() {
        _input.Player.SelectClick.performed -= MouseDown;
        _input.Player.SelectClick.canceled -= MouseUp;
    }

    private void Update() {

        _mousePosition = _input.Player.Mouse.ReadValue<Vector2>();
        MouseVelocity = _mouseVelocity;


        if (_mouseDown && _clickTimer < _clickLength) {
            _clickTimer += Time.deltaTime;
            return;
        }

        if (_mouseDown) {
            //_rotateSpeed = _mouseVelocity.x * _accelerationSpeed;
            _rotateSpeed = Mathf.MoveTowards(_rotateSpeed, _mouseVelocity.x * _accelerationSpeed, Time.deltaTime * 3000f);
            _rotateSpeed = Mathf.Clamp(_rotateSpeed, -_maxSpeed, _maxSpeed);
        } else {
            _rotateSpeed = Mathf.MoveTowards(_rotateSpeed, 0f, Time.deltaTime * _maxSpeed);
        }



        if (_rotateSpeed < 0) {
            _planet.Rotate(RotationDirection.YawRight, Mathf.Abs(_rotateSpeed));
        } else {
            _planet.Rotate(RotationDirection.YawLeft, _rotateSpeed);
        }

        _prevMousePosition = _input.Player.Mouse.ReadValue<Vector2>();

    }

    private void MouseDown(InputAction.CallbackContext obj) {
        _mouseDown = true;
        _mousePosition = _input.Player.Mouse.ReadValue<Vector2>();
        _clickTimer = 0f;
    }

    private void MouseUp(InputAction.CallbackContext obj) {
        _mouseDown = false;
        if (_clickTimer < _clickLength) {
            Click();
        }
        //TransitionManager.Instance.TransitionToScene("Game");
    }

    private void Click() {
        _mousePosition = _input.Player.Mouse.ReadValue<Vector2>();
        Ray ray = _camera.ScreenPointToRay(_mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 1000f)) {
            if (hitInfo.transform.CompareTag(Globals.MISSION_MARKER_TAG)) {
                MissionMarker mission = hitInfo.transform.GetComponent<MissionMarker>();
                if (mission != null) {
                    TransitionManager.Instance.TransitionToScene(mission.SceneName);
                }
            }

        }

    }
}
