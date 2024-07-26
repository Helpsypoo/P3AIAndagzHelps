using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AOEProjector : MonoBehaviour {

    [field: SerializeField] public Color Colour { get; private set; }

    [SerializeField] private DecalProjector _projector;

    private void Awake() {
        if (_projector == null) {
            Destroy(gameObject);
        } else {
            Material material = new Material(_projector.material);
            material.SetColor("_Colour", Colour);
            _projector.material = material;
        }
    }

    public void SetColour(Color colour) {
        Material material = new Material(_projector.material);
        Colour = colour;
        material.SetColor("_Colour", Colour);
        _projector.material = material;
    }

    public void SetSize(Vector2 size) {
        Vector3 decalSize = new Vector3(size.x, size.y, _projector.size.z);
        _projector.size = decalSize;
    }

    public void SetSize(float x, float y) {
        SetSize(new Vector2(x, y));
    }

    public void Deactivate() => gameObject.SetActive(false);
    public void Activate() => gameObject.SetActive(true);

}
