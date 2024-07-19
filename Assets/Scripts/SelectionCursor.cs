using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionCursor : MonoBehaviour {

    private MeshRenderer _renderer;

    private void Awake() {
        _renderer = GetComponent<MeshRenderer>();
    }

    public void SetSelectionType(SelectionType type) {

        switch (type) {
            case SelectionType.Navigation:
                _renderer.material.color = Color.white;
                break;
            case SelectionType.SelectUnit:
                _renderer.material.color = Color.blue;
                break;
            case SelectionType.Invalid:
                _renderer.material.color = Color.red;
                break;
            default:
                break;
        }

    }

}

public enum SelectionType {

    Navigation,
    SelectUnit,
    Invalid

}