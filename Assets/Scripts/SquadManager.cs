using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using Cinemachine;

public class SquadManager : MonoBehaviour {

    public static SquadManager Instance { get; private set; }
    
    public int UnitIndex { get; private set; } = 0;

    // The unit that is currently active, and actions will be performed with.
    public Unit SelectedUnit => GameManager.Instance.PlayerUnits[UnitIndex];
    [field: SerializeField] public int WaypointStash { get; private set; }

    [SerializeField] private CinemachineVirtualCamera _cinemachineCamera;

    // Any unit the cursor is currently hovering over.
    [SerializeField] private Transform _highlightedEntity;

    public bool ActionMode { get; private set; }

    private bool _mouseOverUI;

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
        _input.Player.SelectClick.performed += SelectClick;
        _input.Player.ActionClick.performed += ActionClick;
        _input.Player.ActionModifier.performed += ActionModifierClick;
        _input.Player.Unit1.performed += SelectUnit1;
        _input.Player.Unit2.performed += SelectUnit2;
        _input.Player.Unit3.performed += SelectUnit3;
        _input.Player.Unit4.performed += SelectUnit4;
        _input.Player.Enable();
    }

    private void OnDisable() {
        _input.Player.SelectClick.performed -= SelectClick;
        _input.Player.ActionClick.performed -= ActionClick;
        _input.Player.ActionModifier.performed -= ActionModifierClick;
        _input.Player.Unit1.performed -= SelectUnit1;
        _input.Player.Unit2.performed -= SelectUnit2;
        _input.Player.Unit3.performed -= SelectUnit3;
        _input.Player.Unit4.performed -= SelectUnit4;
    }

    public void Init() {
        UpdateSquadNumbers();
        SelectUnit(0);
    }

    private void Update() {
        //UpdateSelectionMarker();
        _mouseOverUI = EventSystem.current.IsPointerOverGameObject();
    }

    /// <summary>
    /// Called when right mouse button (or equivalent) is clicked.
    /// </summary>
    /// <param name="obj"></param>
    private void SelectClick(InputAction.CallbackContext obj) {

        // If the mouse is over the HUD, don't register the click in the actual game.
        if (_mouseOverUI) return;

        // Move the selection marker to the current mouse position and update _highlightedEntity. Since we're either selecting a
        // comrade or doing nothing, deactivate the marker.
        UpdateSelectionMarker();
        GameManager.Instance.SelectionMarker.Deactivate();

        // Since this click is only concerned with selecting our comrades, if we don't currently have a highlighted entity,
        // break out. Grab some dinner. Catch a movie. Clean your room.
        if (_highlightedEntity == null) return;

        // Same if we do have a highlighted entity but it's not tagged as one of our comrades.
        if (!_highlightedEntity.CompareTag(Globals.UNIT_TAG)) return;

        // Attempt to get the Unit component (or component that inherits from Unit). Throw exception if we fail.
        Unit unit = _highlightedEntity.GetComponent<Unit>();
        if (unit == null) {
            throw new System.Exception($"Attempted to get Unit component from object object {_highlightedEntity.name}. Object is tagged as Unit but no Unit componnent was found.");
        }

        if (_input.Player.SelectionModifier.IsPressed()) {
            FollowUnit(unit);
        } else {
            SelectUnit(_highlightedEntity);
        }

        GameManager.Instance.SelectionMarker.Deactivate();

    }

    private void ActionModifierClick(InputAction.CallbackContext obj) {
        if (SelectedUnit.UnitStats.ImmediateAction) {
            SelectedUnit.PerformAction(GameManager.Instance.SelectionMarker.transform.position, _highlightedEntity);
            return;
        }

        ActionMode = !ActionMode;

    }

    /// <summary>
    /// Called when the left mouse button (or equivalent) is clicked.
    /// </summary>
    /// <param name="obj"></param>
    private void ActionClick(InputAction.CallbackContext obj) {

        // If the mouse is over the HUD, don't register the click in the actual game.
        if (_mouseOverUI) return;

        // Move the selection marker to the current mouse position and update _highlightedEntity.
        UpdateSelectionMarker();

        //Debug.Log($"Performing action {SelectedUnit.UnitStats.ImmediateAction}");
        // If Action Mode is toggled on, perform the action.
        if (ActionMode) {
            SelectedUnit.PerformAction(GameManager.Instance.SelectionMarker.transform.position, _highlightedEntity);
            ActionMode = false;
            return;
        }   
        
        // If we don't have a selected entity and the cursor is on a walkable spot, our unit needs to walk there.
        if (_highlightedEntity == null && GameManager.Instance.SelectionMarker.CanWalk) {
            GameManager.Instance.SelectionMarker.Activate(SelectedUnit);
            SelectedUnit.MoveTo(GameManager.Instance.SelectionMarker.Position);
            SelectedUnit.ClearFollowTarget();
            return;
        }

        // If we have a selected entity, how we handle it depends on what it is.
        if (_highlightedEntity != null) {

            // If the highlighted entity has a unit component, that covers a lot of the things we'll be clicking on.
            Unit unit = _highlightedEntity.GetComponent<Unit>();
            if (unit != null) {

                // If the highlighted entity is a comrade, set the currently selected unit to follow them and switch to that unit.
                if (unit.transform.CompareTag(Globals.UNIT_TAG) && unit != SelectedUnit) {

                    // If this unit is down, we need to revive them.
                    /*if (unit.State == UnitState.Dead) {
                        Debug.Log("Attempted to select fallen unit");
                        unit.Select();
                        SelectedUnit.StandDown();
                        SelectedUnit.SetReviveTarget(unit);

                    } else {*/

                        Unit thisUnit = SelectedUnit;
                        thisUnit.StandDown();
                        thisUnit.SetTarget(unit);
                        SelectUnit(_highlightedEntity);

                    //}

                    GameManager.Instance.SelectionMarker.Deactivate();

                // If the highlighted entity is a liberated, follow them.
                } else if (_highlightedEntity.CompareTag(Globals.LIBERATED_TAG)) {
                    
                    SelectedUnit.StandDown();
                    SelectedUnit.SetTarget(unit);

                    // If the highlighted entity is an enemy, attack them.
                } else if (_highlightedEntity.CompareTag(Globals.ENEMY_TAG)) {
                    Debug.Log("Attacking Enemy");
                    SelectedUnit.Attack(unit);
                    unit.Select();
                    GameManager.Instance.SelectionMarker.Deactivate();

                }

            }

        }

    }

    /// <summary>
    /// Updates the postition, orientation, and state of the selection marker.
    /// </summary>
    private void UpdateSelectionMarker() {

        Vector2 mousePosition = _input.Player.Mouse.ReadValue<Vector2>();
        Ray ray = GameManager.Instance.MainCamera.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 1000f, Globals.SELECTION_LAYERMASK, QueryTriggerInteraction.Ignore)) {

            Vector3 newPos = hitInfo.point;
            Vector3 newUp = hitInfo.normal;

            // If the object we have hit is a entity, set our highlighted entity to it.
            if (hitInfo.transform.gameObject.layer == Globals.ENTITIES_LAYER) {

                _highlightedEntity = hitInfo.transform;
                newPos = hitInfo.transform.position;
                newUp = Vector3.up;

            } else {

                _highlightedEntity = null;

            }
            GameManager.Instance.SelectionMarker.Place(newPos, newUp, hitInfo.transform);
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
        if (index >= GameManager.Instance.PlayerUnits.Count) return;

        // If we have changed units, reset action mode.
        if (index != UnitIndex) {
            // Reset action mode so we're not immediately performing the new unit's action.
            ActionMode = false;
        }

        // If Unit is dead, we can't select it.
        if (GameManager.Instance.PlayerUnits[index].State == UnitState.Dead) return;

        UnitIndex = index;
        for (int i = 0; i < GameManager.Instance.PlayerUnits.Count; i++) {
            GameManager.Instance.PlayerUnits[i].Deselect();
        }

        // Loop through each unit.
        foreach (Unit u in GameManager.Instance.PlayerUnits) {

            // If the current unit is not dead we can move on.
            if (u.State == UnitState.Dead) continue;

            Unit nextUnit = u;
            List<Unit> train = new();

            // Loop until we either encounter a unit that is not following anyone, or we encounter the leader.
            do {

                if (!nextUnit.transform.CompareTag(Globals.UNIT_TAG)) break;
                train.Add(nextUnit);
                if (nextUnit.IsLeader) break;
                nextUnit = nextUnit.FollowTarget;
                
            } while (nextUnit != null);


            // If we have anybody in the train, and the train ends with a leader, activate all units in the train.
            if (train.Count > 0 && train[train.Count - 1].IsLeader) {
                foreach(Unit activeUnit in train) {
                    activeUnit.Select();
                }
            }


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

    public void SelectUnit(Unit unit) {
        SelectUnit(unit.SquadNumber);
    }

    public void FollowUnit(Unit unit) {

        if (SelectedUnit == unit) return;

        SelectedUnit.SetTarget(unit);
        unit.StandDown();
        SelectUnit(unit);

    }

    /// <summary>
    /// Selects the next unit in the squad (incrementing upwards). Wraps around to 0.
    /// </summary>
    public void SelectNextAvailableUnit () {
        UnitIndex++;
        if (UnitIndex >= GameManager.Instance.PlayerUnits.Count) {
            UnitIndex = 0;
        }
        SelectUnit(UnitIndex);
    }

    /// <summary>
    /// Goes through each unit in our _units array and assigns their squad number (index in the array).
    /// </summary>
    private void UpdateSquadNumbers() {
        
        for (int i = 0; i < GameManager.Instance.PlayerUnits.Count; i++) {
            GameManager.Instance.PlayerUnits[i].UpdateSquadNumber(i);
        }

    }

    public void DecrementWaypoints() => WaypointStash--;
    public void IncrementWaypoints() => WaypointStash++;

}
