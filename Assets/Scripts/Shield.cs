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

    private Unit _unit;

    /// <summary>
    /// Shrinks the shield down to nothing and deactivates it.
    /// </summary>
    public void Deactivate() {

        StartCoroutine(ExpandContract(CurrentRadius, 0f));

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
