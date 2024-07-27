using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chonk : Unit {

    [SerializeField] private Shield _shield;
    [field: SerializeField] public float Radius { get; private set; }
    [field: SerializeField] public float ShieldHealth { get; private set; }
    [field: SerializeField] public float MaxShieldHealth { get; private set; }

    public override void PerformAction(Vector3 position, Transform target = null) {
        base.PerformAction(position, target);

        if (!_shield.IsActive) {
            _shield.Activate(Radius, MaxShieldHealth);
        } else {
            _shield.Deactivate();
            //
        }

    }

}
