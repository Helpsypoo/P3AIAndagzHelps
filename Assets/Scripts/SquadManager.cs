using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class SquadManager : MonoBehaviour {

    public static SquadManager Instance { get; private set; }

    [SerializeField] private Unit[] _units;
    private int _unitIndex = 0;
    public Unit SelectedUnit => _units[_unitIndex];

    [SerializeField] private CinemachineVirtualCamera _cinemachineCamera;
    [SerializeField] private SelectionCursor _cursor;
    private Transform _highlightedUnit;

    private BaseInput _input;

    private void Awake() {
        if (Instance) {
            Destroy(this);
        } else {
            Instance = this;
        }

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
        UpdateSquadNumbers();
        SelectUnit(0);
    }

    private void Update() {
        UpdateCursorPos();
    }

    private void MoveClick(InputAction.CallbackContext obj) {

        if (_highlightedUnit != null) {
            SelectUnit(_highlightedUnit);
        } else {
            SelectedUnit.MoveTo(_cursor.transform.position);
        }
    }

    private void UpdateCursorPos() {
        Vector2 mousePosition = _input.Player.Mouse.ReadValue<Vector2>();
        Ray ray = GameManager.Instance.MainCamera.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo)) {

            Vector3 newPos = hitInfo.point;

            if (hitInfo.transform.CompareTag("NotWalkable")) {
                _cursor.SetSelectionType(SelectionType.Invalid);
                _highlightedUnit = null;
            } else if (hitInfo.transform.CompareTag("Unit")) {
                _cursor.SetSelectionType(SelectionType.SelectUnit);
                _highlightedUnit = hitInfo.transform;
                newPos = hitInfo.transform.position;
            } else {
                _cursor.SetSelectionType(SelectionType.Navigation);
                _highlightedUnit = null;
            }
            
            newPos.y += 0.01f;  // Raise cursor up a smidgeon to avoid Z fighting.
            _cursor.transform.position = newPos;
        }
    }

    private void SelectUnit1(InputAction.CallbackContext obj) => SelectUnit(0);
    private void SelectUnit2(InputAction.CallbackContext obj) => SelectUnit(1);
    private void SelectUnit3(InputAction.CallbackContext obj) => SelectUnit(2);
    private void SelectUnit4(InputAction.CallbackContext obj) => SelectUnit(3);

    /// <summary>
    /// Selects a unit based on the squad number/index in the units array.
    /// </summary>
    /// <param name="index">The index of the unit in the _units array.</param>
    private void SelectUnit(int index) {

        // Make sure we're not trying to use a unit we don't have.
        if (index >= _units.Length) return;

        // If Unit is dead, we can't select it.
        if (_units[index].State == UnitState.Dead) return;

        _unitIndex = index;
        for (int i = 0; i < _units.Length; i++) {
            if (i == _unitIndex) _units[i].Select();
            else _units[i].Deselect();
        }

        _cinemachineCamera.Follow = SelectedUnit.transform;
        _cinemachineCamera.LookAt = SelectedUnit.transform;

    }

    /// <summary>
    /// Selects unit based on the transform passed in (if it is a unit).
    /// </summary>
    /// <param name="unitTransform">The transform of the unit we are selecting.</param>
    public void SelectUnit(Transform unitTransform) {

        Unit unit = unitTransform.GetComponent<Unit>();
        if (unit == null) return;
        Debug.Log("Selecting unit by click");
        SelectUnit(unit.SquadNumber);

    }

    /// <summary>
    /// Selects the next unit in the squad (incrementing upwards). Wraps around to 0.
    /// </summary>
    public void SelectNextAvailableUnit () {
        _unitIndex++;
        if (_unitIndex >= _units.Length) {
            _unitIndex = 0;
        }
        SelectUnit(_unitIndex);
    }

    /// <summary>
    /// Goes through each unit in our _units array and assigns their squad number (index in the array).
    /// </summary>
    private void UpdateSquadNumbers() {
        
        for (int i = 0; i < _units.Length; i++) {
            _units[i].UpdateSquadNumber(i);
        }

    }

}
