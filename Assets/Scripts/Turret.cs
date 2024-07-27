using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Enemy {
	[SerializeField] private Transform _turretPivot;
	public override void PeriodicUpdate() {
		base.PeriodicUpdate();

		if (AttackTarget) {
			//rotate _turretPivot towards AttackTarget
			//
		}
	}
}
