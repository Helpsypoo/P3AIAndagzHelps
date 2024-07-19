using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class SquadManager : MonoBehaviour {

    public static SquadManager Instance { get; private set; }



    [SerializeField] private Unit[] _units;
    private int _unitIndex = 0;

    // The unit that is currently active, and actions will be performed with.
    public Unit SelectedUnit => _units[_unitIndex];

    [SerializeField] private CinemachineVirtualCamera _cinemachineCamera;
    [SerializeField] private SelectionCursor _cursor;

    // Any unit the cursor is currently hovering over.
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
        _input.Player.ActionClick.performed += ActionClick;
        _input.Player.Unit1.performed += SelectUnit1;
        _input.Player.Unit2.performed += SelectUnit2;
        _input.Player.Unit3.performed += SelectUnit3;
        _input.Player.Unit4.performed += SelectUnit4;
        _input.Player.Enable();
    }

    private void OnDisable() {
        _input.Player.MoveClick.performed -= MoveClick;
        _input.Player.ActionClick.performed -= ActionClick;
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
        UpdateSelectionMarker();
    }

    /// <summary>
    /// Called when right mouse button (or equivalent) is clicked.
    /// </summary>
    /// <param name="obj"></param>
    private void MoveClick(InputAction.CallbackContext obj) {

        Unit unit;
        
        // If we have highlighted unit.
        if (_highlightedUnit != null) {

            // Get the Unit component and make sure we succesfully got it.
            unit = _highlightedUnit.GetComponent<Unit>();
            if (unit != null) {
                // If the selected unit's state is not dead, select dat unit.
                if (unit.State != UnitState.Dead) {
                    SelectUnit(_highlightedUnit);
                } else {
                    // TODO get distination near corpse rather than trying to walk onto it.
                    SelectedUnit.MoveTo(_highlightedUnit.position);
                }
            }
        // If we are clicking on navigable terrain, set the selected unit awf to that destination.
        } else if (_cursor.Type == SelectionType.Navigation) {
            SelectedUnit.MoveTo(_cursor.transform.position);
        }

    }

    /// <summary>
    /// Called when the left mouse button (or equivalent) is clicked.
    /// </summary>
    /// <param name="obj"></param>
    private void ActionClick(InputAction.CallbackContext obj) {

        if (_highlightedUnit != null) {
            Unit unit = _highlightedUnit.GetComponent<Unit>();
            if (unit != null && unit.State == UnitState.Dead) {
                unit.Revive();
            }
        }

    }

    /// <summary>
    /// Updates the postition, orientation, and state of the selection marker.
    /// </summary>
    private void UpdateSelectionMarker() {
        Vector2 mousePosition = _input.Player.Mouse.ReadValue<Vector2>();
        Ray ray = GameManager.Instance.MainCamera.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo)) {

            Vector3 newPos = hitInfo.point;
            Vector3 newUp = hitInfo.normal;

            if (hitInfo.transform.CompareTag(Globals.NOT_WALKABLE_TAG)) {

                _cursor.SetSelectionType(SelectionType.Invalid);
                _highlightedUnit = null;

            } else if (hitInfo.transform.CompareTag(Globals.UNIT_TAG)) {

                _cursor.SetSelectionType(SelectionType.SelectUnit);
                _highlightedUnit = hitInfo.transform;
                newPos = hitInfo.transform.position;
                newUp = Vector3.up;

            } else if (hitInfo.transform.CompareTag(Globals.DOWNED_UNIT_TAG)) {

                _cursor.SetSelectionType(SelectionType.ReviveUnit);
                _highlightedUnit = hitInfo.transform;
                newPos = hitInfo.transform.position;
                newUp = Vector3.up;

            } else {
                _cursor.SetSelectionType(SelectionType.Navigation);
                _highlightedUnit = null;
            }
            
            _cursor.Place(newPos, newUp);
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
