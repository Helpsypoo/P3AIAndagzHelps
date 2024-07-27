using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AOEProjector : MonoBehaviour {

    [Tooltip("The starting width of the AOE. Can be modified at runtime using SetSize or SetRadius")]
    [SerializeField] private float _width;
    [Tooltip("The starting height of the AOE. Can be modified at runtime using SetSize or SetRadius")]
    [SerializeField] private float _height;


    [Tooltip("The colour tint of the AOE.")]
    [field: SerializeField] public Color Colour { get; private set; }

    [Tooltip("The DecalProjector we are using. Ideally set as a child of this component and locally rotated in the correct direction.")]
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

    /// <summary>
    /// Sets the colour tint of the AOE decal.
    /// </summary>
    /// <param name="colour">The colour of the AOE, alpha value not factored.</param>
    public void SetColour(Color colour) {
        Material material = new Material(_projector.material);
        Colour = colour;
        material.SetColor("_Colour", Colour);
        _projector.material = material;
    }

    /// <summary>
    /// Set the size of the AOE.
    /// </summary>
    /// <param name="size">Vector2 representing width (x) and height (y) of AOE.</param>
    public void SetSize(Vector2 size) {
        Vector3 decalSize = new Vector3(size.x, size.y, _projector.size.z);
        _height = size.x;
        _width = size.y;
        _projector.size = decalSize;
    }

    /// <summary>
    /// Set the size of the AOE.
    /// </summary>
    /// <param name="width">The full width.</param>
    /// <param name="height">The full height.</param>
    public void SetSize(float width, float height) {
        SetSize(new Vector2(width, height));
    }

    /// <summary>
    /// Sets the radius of the AOE, equal size in both directions.
    /// </summary>
    /// <param name="radius">Half the width/height of the AOE.</param>
    public void SetRadius(float radius) {
        SetSize(new Vector2(radius * 2f, radius * 2f));
    }

    /// <summary>
    /// Disables the AOE GameObject.
    /// </summary>
    public void Deactivate() => gameObject.SetActive(false);

    /// <summary>
    /// Enables the AOE GameObject.
    /// </summary>
    public void Activate() => gameObject.SetActive(true);

    /// <summary>
    /// Activates the AOE Projector and smoothly transitions from the current size to the desired size.
    /// </summary>
    /// <param name="speed">How fast the transition should be (1 = 1 second).</param>
    /// <param name="width">The width of the AOE.</param>
    /// <param name="height">The height of the AOE.</param>
    public void ActivateExpand(float speed, float width, float height) {

        if (_expander != null) {
            StopCoroutine(_expander);
            _expander = null;
        }

        Activate();
        _expander = StartCoroutine(Expand(speed, width, height));
    }

    private Coroutine _expander;
    private IEnumerator Expand(float speed, float width, float height) {

        float timer = 0f;

        while (timer < 1f) {

            SetSize(
                Mathf.Lerp(0f, width, timer),
                Mathf.Lerp(0f, height, timer)
                );
            timer += Time.deltaTime * speed;
            yield return null;

        }
        SetSize(width, height);
    }

}
