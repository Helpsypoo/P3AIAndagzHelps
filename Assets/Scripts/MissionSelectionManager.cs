using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;

public class MissionSelectionManager : MonoBehaviour {

    private BaseInput _input;
    private Camera _camera;

    [SerializeField] private AutoRotator _planet;
    private float _rotateSpeed;
    [SerializeField] private float _accelerationSpeed = 0.5f;
    [SerializeField] private float _maxSpeed = 100f;

    [SerializeField] GameObject _missionWindow;
    [SerializeField] TextMeshProUGUI _missionTitle;
    [SerializeField] TextMeshProUGUI _missionDescription;
    [SerializeField] GameObject _missionButton;
    [SerializeField] GameObject _missionFailedText;
    [SerializeField] GameObject _missionCompletedText;
    [SerializeField] GameObject _missionLockedText;

    [SerializeField] private AudioClip _clickSound;
    [SerializeField] private AudioClip _startSound;
    [SerializeField] private AudioClip _backSound;
    [SerializeField] private UsefulButton _buttonSpinLeft;
    [SerializeField] private UsefulButton _buttonSpinRight;

    private bool _mouseOverUI;

    private MissionMarker _highlightedMissionMarker;        // The currently moused over mission marker.
    private MissionMarker _selectedMissionMarker;

    private bool _mouseDown;
    private bool _mouseOverMission = false;
    private Vector2 _mousePosition;
    private Vector2 _prevMousePosition;
    private Vector2 _mouseDragDelta;
    private Vector2Int _mouseDragDirection;
    private Vector2 _mouseDragPosition;
    private Vector2 _mouseDragStart;
    private float _dragPower;

    private Vector2 _mouseVelocity => _prevMousePosition - _mousePosition;
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
        if (_mouseDown && _clickTimer < _clickLength) {
            _clickTimer += Time.deltaTime;
            return;
        }

        UpdateMouse();

        var normalizedDragDeltaX = _mouseDragDelta.x / Screen.width;
        var activeDragPower = 128f * normalizedDragDeltaX;
        var passiveDragPower = _mouseDragDirection.x / 4f;

        if (Math.Abs(activeDragPower) > Math.Abs(_dragPower))
        {
            if (Math.Sign(activeDragPower) == _mouseDragDirection.x)
            {
                _dragPower = activeDragPower;
            }
        }
        else
        {
            _dragPower = Mathf.MoveTowards(
                _dragPower,
                passiveDragPower,
                Math.Max(Math.Abs(_dragPower), Math.Max(0.01f, Math.Sign(passiveDragPower))) / 2f
            );
            // Debug.Log($"dp={_dragPower} / dd={_mouseDragDirection.x} / adp={activeDragPower} / pdp={passiveDragPower}");
        }

        if (_buttonSpinLeft != default && _buttonSpinLeft.IsPressed)
        {
            Spin(RotationDirection.YawRight);
        }

        if (_buttonSpinRight != default && _buttonSpinRight.IsPressed)
        {
            Spin(RotationDirection.YawLeft);
        }

        if (_mouseDown)
        {
            _rotateSpeed = Mathf.MoveTowards(_rotateSpeed, _dragPower * _accelerationSpeed, Time.deltaTime * 10000f);
            _rotateSpeed = Mathf.Clamp(_rotateSpeed, -_maxSpeed, _maxSpeed);
        } else {
            _rotateSpeed = Mathf.MoveTowards(_rotateSpeed, 0f, Time.deltaTime * _accelerationSpeed);
        }

        // Debug.Log(
        //     $"{nameof(dragDirection)}={dragDirection} / {nameof(_mouseDragDelta)}={_mouseDragDelta} / {nameof(normalizedDragDeltaX)}={normalizedDragDeltaX} / {nameof(_mouseDragDirection)}={_mouseDragDirection}");

        if (_rotateSpeed < 0) {
            _planet.Rotate(RotationDirection.YawRight, Mathf.Abs(_rotateSpeed));
        } else {
            _planet.Rotate(RotationDirection.YawLeft, _rotateSpeed);
        }
    }

    private void UpdateMouse() {
        _mouseOverUI = EventSystem.current.IsPointerOverGameObject();

        _mousePosition = _input.Player.Mouse.ReadValue<Vector2>();

        if (_mouseDown)
        {
            var previousMouseDragPosition = _mouseDragPosition;
            _mouseDragPosition = _prevMousePosition;

            if (previousMouseDragPosition == _mouseDragStart && _mouseDragPosition != _mousePosition)
            {
                var mouseDragDeltaFromStart = _mouseDragPosition - _mouseDragStart;
                _mouseDragDirection = new Vector2Int(Math.Sign(mouseDragDeltaFromStart.x), Math.Sign(mouseDragDeltaFromStart.y));
            }
            // else
            // {
            //     Debug.Log($"mdp={_mouseDragPosition} / mds={_mouseDragStart} / == -> {_mouseDragPosition == _mouseDragStart} / pmp={_prevMousePosition} / mp={_mousePosition} / != -> {_prevMousePosition != _mousePosition}");
            // }
        }
        else
        {
            _mouseDragPosition = _mousePosition;
            _mouseDragStart = _mousePosition;
        }

        _mouseDragDelta = _mousePosition - _mouseDragPosition;

        Ray ray = _camera.ScreenPointToRay(_mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 1000f)) {
            if (hitInfo.transform.CompareTag(Globals.MISSION_MARKER_TAG)) {
                if (_highlightedMissionMarker == null || _highlightedMissionMarker.gameObject != hitInfo.transform.gameObject) {
                    UpdateSelectedMission(hitInfo.transform.GetComponent<MissionMarker>());
                }
                return;
            }

        }

        _highlightedMissionMarker = null;
        _prevMousePosition = _mousePosition;
    }

    private void MouseDown(InputAction.CallbackContext obj) {

        if (_mouseOverUI) return;

        _mouseDown = true;
        _mouseDragStart = _mousePosition;
        _mouseDragPosition = _mouseDragStart;

        _clickTimer = 0f;

        if (_highlightedMissionMarker == null) {
            _missionWindow.SetActive(false);
            _selectedMissionMarker = null;
        }
    }

    private void MouseUp(InputAction.CallbackContext obj) {
        if (_mouseOverUI) return;

        _mouseDown = false;

        if (_clickTimer < _clickLength) {
            MapClick();
        }
        //TransitionManager.Instance.TransitionToScene("Game");
    }

    /// <summary>
    /// Called when user clicks on the planet map.
    /// </summary>
    private void MapClick() {

        if (_mouseOverUI) return;
        //if (_highlightedMissionMarker != null) {
        //    TransitionManager.Instance.TransitionToScene(_highlightedMissionMarker.SceneName);
        //}
        if (_highlightedMissionMarker == null) return;
        _selectedMissionMarker = _highlightedMissionMarker;
        _missionTitle.text = _selectedMissionMarker.Details.Name;
        _missionDescription.text = _selectedMissionMarker.Details.Description;

        //_startMissionButton.enabled = _selectedMissionMarker.Details.Available;
        switch (_selectedMissionMarker.Details.Condition) {
            case MissionCondition.Available:
                _missionButton.SetActive(true);
                _missionCompletedText.SetActive(false);
                _missionFailedText.SetActive(false);
                _missionLockedText.SetActive(false);
                break;
            case MissionCondition.Complete:
                _missionButton.SetActive(false);
                _missionCompletedText.SetActive(true);
                _missionFailedText.SetActive(false);
                _missionLockedText.SetActive(false);
                break;
            case MissionCondition.Locked:
                _missionButton.SetActive(false);
                _missionCompletedText.SetActive(false);
                _missionFailedText.SetActive(false);
                _missionLockedText.SetActive(true);
                break;
            default:
                _missionButton.SetActive(false);
                _missionCompletedText.SetActive(false);
                _missionFailedText.SetActive(true);
                _missionLockedText.SetActive(false);
                break;
        }
        AudioManager.Instance.Play(_clickSound, MixerGroups.UI);
        _missionWindow.SetActive(true);


    }

    public void UIClick() {
        if (_selectedMissionMarker == null) {
            Debug.LogWarning("Attempted to start a mission but no mission was highlighted. This should not be able to happen.");
            return;
        }

        AudioManager.Instance.Play(_startSound, MixerGroups.UI);
        SessionManager.Instance.Level = _selectedMissionMarker.Details.Level;
        TransitionManager.Instance.TransitionToScene(_selectedMissionMarker.Details.SceneName);

    }

    private void UpdateSelectedMission(MissionMarker marker) {

        _highlightedMissionMarker = marker;

    }

    public void BackToMenu() {
        AudioManager.Instance.Play(_backSound, MixerGroups.UI);
        TransitionManager.Instance.TransitionToScene("Menu");
    }

    private void Spin(RotationDirection direction)
    {
        var targetSpeed = (direction == RotationDirection.YawLeft ? 1 : -1) * 0.35f * _accelerationSpeed;
        _rotateSpeed = Mathf.MoveTowards(_rotateSpeed, targetSpeed, Time.deltaTime * 10000f);
        _rotateSpeed = Mathf.Clamp(_rotateSpeed, -_maxSpeed, _maxSpeed);

        if (Math.Abs(_rotateSpeed) > 0) {
            _planet.Rotate(direction, Mathf.Abs(_rotateSpeed));
        }
    }

}
