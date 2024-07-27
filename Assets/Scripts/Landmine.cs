using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Landmine : MonoBehaviour {

    [Header("Visuals")]
    [Tooltip("The colour of the area of effect circle")]
    [SerializeField] private Color _aoeColour;

    [Header("Blast Variables")]
    [Tooltip("How wide the blast radius in Unity units.")]
    [SerializeField] private float _blastRadius = -50f;
    [Tooltip("The damange recieved by units closest to the centre.")]
    [SerializeField] private float _maxDamage;

    [Header("Timing Variables")]
    [Tooltip("The base amount of time (in seconds) between warning beeps when triggered.")]
    [SerializeField] private float _beepInterval;
    [Tooltip("The number of times the beeps get faster before exploding.")]
    [SerializeField] private int _beepStages;
    [Tooltip("How quickly the visual effect expands (has no bearing on the functionality of the mine.")]
    [SerializeField] private float _blastExpansionSpeed;

    [Header("References")]
    [SerializeField] private AudioClip _beepClip;
    [SerializeField] private AudioClip _explosionClip;
    [SerializeField] private GameObject _lightOff;
    [SerializeField] private GameObject _lightOn;   
    [SerializeField] private Transform _blastVisualSphere;
    [SerializeField] private AOEProjector _projector;

    public Vector3 BlastOrigin => new Vector3(transform.position.x, transform.position.y - _blastRadius, transform.position.z);

    private Coroutine _explosionRoutine;

    private void Awake() {

        Vector2 decalSize = new Vector3(_blastRadius * 2f, _blastRadius * 2f);
        _projector.SetSize(decalSize);
        _projector.SetColour(_aoeColour);
        _projector.Deactivate();

    }

    float timer;
    private void Update() {

        // If we are currently exploding, don't blink the light.
        if (_explosionRoutine != null) {
            return;
        }

        timer += Time.deltaTime;
        if (timer > _beepInterval) {
            ToggleLight();
            timer = 0f;
        }

    }

    /// <summary>
    /// Toggles the warning light on and off.
    /// </summary>
    private void ToggleLight() {
        _lightOff.SetActive(!_lightOff.activeSelf);
        _lightOn.SetActive(!_lightOn.activeSelf);

        // If the light has just come on and we are currently counting down to explosion, then, play the beep.
        if (_lightOn.activeSelf && _explosionRoutine != null) {
            AudioManager.Instance.Play(_beepClip, MixerGroups.SFX, default, 1f, transform.position, 0.95f);
        }

    }

    /// <summary>
    /// Returns a list of Units that are within the blast radius.
    /// </summary>
    /// <returns>List<Unit>()</returns>
    /// <exception cref="System.Exception">Throws exception if any objects tagged as Unit do not have a Unit component on them.</exception>
    private List<Unit> GetVictims() {

        // Get any colliders in the explosion radius.
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _blastRadius);
        List<Unit> units = new List<Unit>();

        // Remove any colliders that are not Unit base classes from the list.
        for (int i = 0; i < hitColliders.Length; i++) {
            if (hitColliders[i].transform.CompareTag(Globals.UNIT_TAG) ||
                hitColliders[i].transform.CompareTag(Globals.LIBERATED_TAG) ||
                hitColliders[i].transform.CompareTag(Globals.ENEMY_TAG)
                ) {

                // Get the unit component from the collider object.
                Unit unit = hitColliders[i].GetComponent<Unit>();
                if (unit == null) {
                    // If we have something tagged as a unit that doesn't have a unit component, that's a fatal error, cry about it.
                    throw new System.Exception($"Attempted to get Unit component from {hitColliders[i].transform.name}, which was tagged as a Unit, but no Unit component was found.");
                } else {
                    units.Add(unit);
                }
            }
        }
        return units;
    }

    /// <summary>
    /// Takes in a list of Units and applys _blastRadius damage to them.
    /// </summary>
    /// <param name="units">List<Unit>()</param>
    private void ApplyDamage(List<Unit> units) {

        foreach (Unit unit in units) {

            unit.ChangeHealth(_maxDamage);

            // If the unit is a liberated, launch the fucker.
            if (unit.transform.CompareTag(Globals.LIBERATED_TAG)) {
                Liberated liberated = unit.GetComponent<Liberated>();
                if (liberated == null) {
                    throw new System.Exception($"Attempted to get Liberated component from {unit.transform.name}, which was tagged as a Liberated, but no Liberated component was found.");
                }

                liberated.Launch(16f * _blastRadius, BlastOrigin);

            }

        }

    }

    private void OnTriggerEnter(Collider other) {
        if (other.transform.CompareTag(Globals.UNIT_TAG) ||
            other.transform.CompareTag(Globals.LIBERATED_TAG) ||
            other.transform.CompareTag(Globals.ENEMY_TAG)
            ) {
            Debug.Log("Landmine detected something");
            if (_explosionRoutine == null) {
                _explosionRoutine = StartCoroutine(Explode());
            }
        }
    }

    private IEnumerator Explode() {

        _projector.ActivateExpand(5f, _blastRadius * 2f, _blastRadius * 2f);
        float blinkTimer = 0f;
        float interval = _beepInterval;
        int blinkCount = 0;
        int stageCount = 1;
        while (stageCount < _beepStages) {

            blinkTimer += Time.deltaTime;

            if (blinkTimer > interval) {
                blinkTimer = 0f;
                ToggleLight();

                blinkCount++;
                if (blinkCount > 5 * stageCount) {
                    interval *= 0.5f;
                    stageCount++;
                    blinkCount = 0;
                }

            }

            yield return null;
        }

        AudioManager.Instance.Play(_explosionClip, MixerGroups.SFX, default, 1f, transform.position, 0.95f);
        _blastVisualSphere.gameObject.SetActive(true);
        float size = 0f;
        while (size < _blastRadius * 2f) {

            _blastVisualSphere.localScale = Vector3.one * size;
            size += Time.deltaTime * _blastExpansionSpeed;
            yield return null;

        }

        ApplyDamage(GetVictims());

        _blastVisualSphere.localScale = Vector3.zero;
        _explosionRoutine = null;
        _projector.Deactivate();

        Destroy(gameObject);

    }

}
