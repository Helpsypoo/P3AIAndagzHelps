using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour {

    [SerializeField] private Transform _shieldObject;
    [SerializeField] private SphereCollider _collider;
    [SerializeField] private HealthDisplay _healthBar;
    [SerializeField] private AudioClip _activateSound;
    [SerializeField] private AudioClip _deactivateSound;
    public float CurrentRadius => _collider.radius;
    public bool IsActive => _collider.radius > 0.01f;

    [SerializeField] private float _health;
    [SerializeField] private float _maxHealth;

    private List<Liberated> _protectedLibs = new();

    private TickEntity _tickEntity;

    private void Awake() {
        _tickEntity = GetComponent<TickEntity>();
    }

    private void Start() {
        if (_tickEntity) { _tickEntity.AddToTickEventManager(); }
    }

    public void PeriodicUpdate() {
        if (!IsActive) return;

        List<Liberated> currentLibs = new();
        foreach (Liberated lib in GameManager.Instance.ActiveLiberated) {

            // Get a list of liberated currently within range of the shield.
            if (Vector3.Distance (transform.position, lib.transform.position) <= CurrentRadius * 2f) {
                currentLibs.Add(lib);
                lib.ImmuneFromSun = true;
            }

        }

        // Any liberated that are no longer under the shield need to be made vulnerable to sun again.
        foreach (Liberated l in _protectedLibs) {
            if (!currentLibs.Contains(l)) {
                l.ImmuneFromSun = false;
            }
        }

        _protectedLibs = currentLibs;

        foreach (Unit unit in GameManager.Instance.PlayerUnits) {
            
            if (Vector3.Distance(transform.position, unit.transform.position) <= CurrentRadius * 2f) {
                unit.ImmuneFromSun = true;
            } else {
                unit.ImmuneFromSun = false;
            }
        }

    }

    private Unit _unit;

    /// <summary>
    /// Shrinks the shield down to nothing and deactivates it.
    /// </summary>
    public void Deactivate() {

        StartCoroutine(ExpandContract(CurrentRadius, 0f));
        foreach (Liberated l in _protectedLibs) {
            l.ImmuneFromSun = false;
        }

    }

    /// <summary>
    /// Expands the forcefield out to the given radius and activates it.
    /// </summary>
    /// <param name="radius">The desired radius of the forcefield.</param>
    public void Activate(float radius, float health, float maxHealth, Unit unit) {
        _health = health;
        _maxHealth = maxHealth;
        _unit = unit;
        StartCoroutine(ExpandContract(CurrentRadius, radius));

    }

    private IEnumerator ExpandContract(float startRadius, float endRadius) {

        float elapsedTime = 0f;

        // If we are expanding, play the activating sound.
        if (endRadius > startRadius) {
            AudioManager.Instance.Play(_activateSound, MixerGroups.SFX, default, 1f, transform.position, 0.95f);
        } else {
            AudioManager.Instance.Play(_deactivateSound, MixerGroups.SFX, default, 1f, transform.position, 0.95f);
        }

        Vector3 start = 2f * startRadius * Vector3.one;
        Vector3 end = 2f * endRadius * Vector3.one;

        while (Vector3.Distance(_shieldObject.localScale, end) > 0.05f) {
            _shieldObject.localScale = Vector3.MoveTowards(start, end, elapsedTime);
            elapsedTime += Time.deltaTime * 25f;
            yield return null;
        }

        transform.localScale = end;

        // If we're shrinking the shield down to zero, turn it off afterwards.
        if (endRadius == 0f) {
            _shieldObject.gameObject.SetActive(false);
            _collider.radius = 0f;
            _collider.enabled = false;
        } else {
            _shieldObject.gameObject.SetActive(true);
            _collider.enabled = true;
            _collider.radius = endRadius;
        }
    }

    public void Damage(float amount) {
        _health += amount;
        if (_health < 0f) {
            _unit.UnitStats.InvunerableToSun = false;
            Deactivate();
        } else {
            _healthBar.UpdateHealthDisplay(_health, _maxHealth);
        }
    }

    private void OnTriggerEnter(Collider other) {

        if (other.CompareTag(Globals.BULLET_TAG)) {
            
            Bullet bullet = other.GetComponent<Bullet>();
            if (bullet == null) {
                throw new System.Exception($"{bullet.gameObject.name} is tagged as Bullet but no Bullet component was found.");
            }

            bullet.ReturnToPool();
            Damage(bullet.Damage);

        }

    }

}
