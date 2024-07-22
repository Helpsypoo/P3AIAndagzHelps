using System;
using UnityEngine;

public class Enemy : Unit {

	private void Start() {
		base.Start();
		//Debug.Log($"Running Start on Enemy");
		if (!UnitStats) {
			//Debug.Log($"No UnitStats on Enemy");
			return;
		}

		SphereCollider _sphereCollider = GetComponent<SphereCollider>();
		if (!_sphereCollider) {
			//Debug.Log($"No SphereCollider on Enemy");
			return;
		}
		
		_sphereCollider.radius = UnitStats.AttackRange;
	}

	/*private void PeriodicUpdate() {
		base.PeriodicUpdate();
	}*/

	private void OnTriggerEnter(Collider other) {
		//Debug.Log($"{other.gameObject.name} entered Enemy range");
		if (AttackTarget) {
			//Debug.Log($"{other.gameObject.name} entered Enemy range but is already attacking a current target");
			return;
		}
		
		if (other.CompareTag(Globals.UNIT_TAG)) {
			//Debug.Log("Enemy registered attack");
			Unit _unit = other.GetComponent<Unit>();
			if (!_unit) {
				//Debug.Log($"No unit found on {other.gameObject.name}");
				return;
			}
			
			//Debug.Log($"Enemy attacking {_unit.UnitStats.Name}");
			SetTarget(_unit);
		}
	}
}
