using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionCursor : MonoBehaviour {

    private MeshRenderer _renderer;
    [SerializeField] private float _heightOffset = 0.2f;
    public SelectionType Type { get; private set; }
    public Transform Attached { get ; private set; }
    public bool CanWalk { get; private set; }

    private void Awake() {
        _renderer = GetComponent<MeshRenderer>();
    }

    public void Activate() => gameObject.SetActive(true);
    public void Deactivate() => gameObject.SetActive(false);

    /// <summary>
    /// Places the selection marker at the given position and orientation.
    /// </summary>
    /// <param name="position">Vector3 representing global position where selection marker is being moved to.</param>
    /// <param name="up">Vector3 representing the direction of the selection marker's up.</param>
    public void Place(Vector3 position, Vector3 up, Transform attached) {

        Vector3 newPos = position;
        newPos.y += _heightOffset;
        transform.position = newPos;
        transform.up = up;
        Attached = attached;
        CanWalk = Attached.CompareTag(Globals.WALKABLE_TAG);

    }

    /// <summary>
    /// Checks the location of a completed action against the location of the cursor. If they are within a small amount,
    /// we can assume this is the action the cursor was highlighting and deactivate the cursor.
    /// </summary>
    /// <param name="location"></param>
    public void CheckAction(Vector3 location) {

        if (Vector3.Distance(location, transform.position) < Globals.MIN_ACTION_DIST) {
            Attached = null;
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Sets the current selection type of the selection marker.
    /// </summary>
    /// <param name="type">SelectionType</param>
    public void SetSelectionType(SelectionType type) {

        Type = type;

        switch (type) {
            case SelectionType.Navigation:
                _renderer.material.color = Color.white;
                break;
            case SelectionType.SelectUnit:
                _renderer.material.color = Color.blue;
                break;

            case SelectionType.ReviveUnit:
                _renderer.material.color = Color.green;
                break;
            case SelectionType.Invalid:
                _renderer.material.color = Color.red;
                break;
            default:
                break;
        }

    }

}