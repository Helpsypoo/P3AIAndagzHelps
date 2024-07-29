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

    [SerializeField] private AudioClip _clickSound;
    [SerializeField] private AudioClip _startSound;
    [SerializeField] private AudioClip _backSound;

    private bool _mouseOverUI;

    private MissionMarker _highlightedMissionMarker;        // The currently moused over mission marker.
    private MissionMarker _selectedMissionMarker;

    private bool _mouseDown;
    private bool _mouseOverMission = false;
    private Vector2 _mousePosition;
    private Vector2 _prevMousePosition;
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

        _mouseOverUI = EventSystem.current.IsPointerOverGameObject();

        _mousePosition = _input.Player.Mouse.ReadValue<Vector2>();
        float normalisedMouseX = (_mousePosition.x / Screen.width) - 0.5f;

        if (_mouseDown && _clickTimer < _clickLength) {
            _clickTimer += Time.deltaTime;
            return;
        }

        if (_mouseDown) {
            //_rotateSpeed = _mouseVelocity.x * _accelerationSpeed;
            _rotateSpeed = Mathf.MoveTowards(_rotateSpeed, normalisedMouseX * _accelerationSpeed, Time.deltaTime * 1000f);
            _rotateSpeed = Mathf.Clamp(_rotateSpeed, -_maxSpeed, _maxSpeed);
        } else {
            _rotateSpeed = Mathf.MoveTowards(_rotateSpeed, 0f, Time.deltaTime * _accelerationSpeed);
        }



        if (_rotateSpeed < 0) {
            _planet.Rotate(RotationDirection.YawRight, Mathf.Abs(_rotateSpeed));
        } else {
            _planet.Rotate(RotationDirection.YawLeft, _rotateSpeed);
        }

        UpdateMouse();

        _prevMousePosition = _input.Player.Mouse.ReadValue<Vector2>();

    }

    private void MouseDown(InputAction.CallbackContext obj) {

        if (_mouseOverUI) return;
        _mouseDown = true;
        //_mousePosition = _input.Player.Mouse.ReadValue<Vector2>();
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

    private void UpdateMouse() {
        //_mousePosition = _input.Player.Mouse.ReadValue<Vector2>();
        Ray ray = _camera.ScreenPointToRay(_mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 1000f)) {
            if (hitInfo.transform.CompareTag(Globals.MISSION_MARKER_TAG)) {
                if (_highlightedMissionMarker != null && _highlightedMissionMarker.gameObject != hitInfo.transform.gameObject) {
                    UpdateSelectedMission(hitInfo.transform.GetComponent<MissionMarker>());
                } else if (_highlightedMissionMarker == null) {
                    UpdateSelectedMission(hitInfo.transform.GetComponent<MissionMarker>());
                }
                return;
            }

        }
        
        _highlightedMissionMarker = null;
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
        switch (_selectedMissionMarker.Details.MissionStatus()) {
            case 0:
                _missionButton.SetActive(true);
                _missionFailedText.SetActive(false);
                _missionCompletedText.SetActive(false);
                break;
            case 1:
                _missionButton.SetActive(false);
                _missionFailedText.SetActive(false);
                _missionCompletedText.SetActive(true);
                break;
            default:
                _missionButton.SetActive(false);
                _missionFailedText.SetActive(true);
                _missionCompletedText.SetActive(false);
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

}
