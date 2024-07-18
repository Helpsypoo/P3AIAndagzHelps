using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class SquadManager : MonoBehaviour {

    [SerializeField] private Unit[] _units;
    private int _unitIndex = 0;
    public Unit SelectedUnit => _units[_unitIndex];

    [SerializeField] private CinemachineVirtualCamera _cinemachineCamera;
    [SerializeField] private Transform _cursor;

    private BaseInput _input;

    private void Awake() {
        _input = new BaseInput();
    }

    private void OnEnable() {
        _input.Player.MoveClick.performed += MoveClick;
        _input.Player.Unit1.performed += SelectUnit1;
        _input.Player.Unit2.performed += SelectUnit2;
        _input.Player.Unit3.performed += SelectUnit3;
        _input.Player.Unit4.performed += SelectUnit4;
        _input.Player.Enable();
    }

    private void OnDisable() {
        _input.Player.MoveClick.performed -= MoveClick;
        _input.Player.Unit1.performed -= SelectUnit1;
        _input.Player.Unit2.performed -= SelectUnit2;
        _input.Player.Unit3.performed -= SelectUnit3;
        _input.Player.Unit4.performed -= SelectUnit4;
    }

    private void Start() {
        Cursor.visible = false;
        SelectUnit(0);
    }

    private void Update() {
        UpdateCursorPos();
    }

    private void MoveClick(InputAction.CallbackContext obj) {
        SelectedUnit.MoveTo(_cursor.position);
    }

    private void UpdateCursorPos() {
        Vector2 mousePosition = _input.Player.Mouse.ReadValue<Vector2>();
        Ray ray = GameManager.Instance.MainCamera.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo)) {
            Vector3 newPos = hitInfo.point;
            newPos.y += 0.01f;  // Raise cursor up a smidgeon to avoid Z fighting.
            _cursor.position = newPos;
        }
    }

    private void SelectUnit1(InputAction.CallbackContext obj) => SelectUnit(0);
    private void SelectUnit2(InputAction.CallbackContext obj) => SelectUnit(1);
    private void SelectUnit3(InputAction.CallbackContext obj) => SelectUnit(2);
    private void SelectUnit4(InputAction.CallbackContext obj) => SelectUnit(3);

    private void SelectUnit(int index) {

        // Make sure we're not trying to use a unit we don't have.
        if (index >= _units.Length) return;

        _unitIndex = index;
        for (int i = 0; i < _units.Length; i++) {
            if (i == _unitIndex) _units[i].Select();
            else _units[i].Deselect();
        }

        _cinemachineCamera.Follow = SelectedUnit.transform;
        _cinemachineCamera.LookAt = SelectedUnit.transform;

    }

}
