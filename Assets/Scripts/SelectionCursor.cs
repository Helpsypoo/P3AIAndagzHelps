using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionCursor : MonoBehaviour {

    private MeshRenderer _renderer;
    [SerializeField] private float _heightOffset = 0.2f;
    public SelectionType Type { get; private set; }

    private void Awake() {
        _renderer = GetComponent<MeshRenderer>();
    }

    /// <summary>
    /// Places the selection marker at the given position and orientation.
    /// </summary>
    /// <param name="position">Vector3 representing global position where selection marker is being moved to.</param>
    /// <param name="up">Vector3 representing the direction of the selection marker's up.</param>
    public void Place(Vector3 position, Vector3 up) {

        Vector3 newPos = position;
        newPos.y += _heightOffset;
        transform.position = newPos;
        transform.up = up;

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