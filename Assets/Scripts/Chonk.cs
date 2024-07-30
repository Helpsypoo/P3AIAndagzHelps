using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chonk : Unit {

    [SerializeField] private Shield _shield;
    [field: SerializeField] public float Radius { get; private set; }
    [field: SerializeField] public float MaxShieldHealth { get; private set; }
    [field: SerializeField] public int ShieldCharges { get; private set; }

    public override void Start() {
        base.Start();
        StartCoroutine(SetAfterAppliedUpgrades());
    }
    
    IEnumerator SetAfterAppliedUpgrades() {
        yield return new WaitUntil(() => UpgradesSet);
        _abilityCharges = UnitStats.ActionCharges;
    }

    public override void PerformAction(Vector3 position, Transform target = null) {

        if (_shield.IsActive) return;

        base.PerformAction(position, target);
        Debug.Log("Activating Shield");
        if (_abilityCharges > 0 && !_shield.IsActive) {
            _shield.Activate(Radius, MaxShieldHealth, MaxShieldHealth, this);
            UnitStats.InvunerableToSun = true;
            _abilityCharges--;
        }

    }

    /// <summary>
    /// Checks if the object is in shade or not
    /// </summary>
    public override void CheckLightingStatus(float _timeSinceLastCheck) {

        // If shield is not active, let the base function handle light checks as normal.
        if (!_shield.IsActive) {
            base.CheckLightingStatus(_timeSinceLastCheck);
            return;
        }

        Vector3 lightDir = -_mainLight.transform.forward;
        //Debug.DrawLine(transform.position, transform.position + lightDir * 100f, Color.red, 1f);
        _isInShadow = Physics.Raycast(transform.position, lightDir, out RaycastHit _hitInfo, 2000f, GameManager.Instance.ShadeLayerMask, QueryTriggerInteraction.Ignore);

        if (_isInShadow) {
            //Debug.Log(gameObject.name + $" is shaded by {_hitInfo.transform.gameObject.name}.");
            return;
        }

        //Debug.Log(gameObject.name + " is NOT in shadow.");
        _shield.Damage(GameManager.Instance.SunDamagePerSecond * _timeSinceLastCheck);

    }

    public void OnDisable() {
        UnitStats.InvunerableToSun = false;
    }

}
