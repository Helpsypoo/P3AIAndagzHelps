using System;
using UnityEngine;
using System.Collections.Generic;

public class Enemy : Unit {

	[SerializeField] private List<Unit> _targets = new();

	public override void Awake() {
		transform.SetParent(null);
		base.Awake();
	}
	public override void Start() {
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

    protected override void ProcessAttack() {

        if (_targets[0].State == UnitState.Dead) {
            RemoveTarget(_targets[0]);
			return;
        }

        bool _isInAttackRange = Vector3.Distance(AttackTarget.transform.position, transform.position) <= UnitStats.AttackRange;
        //_anim?.SetBool(hasAttackTargetInRange, _isInAttackRange);

        if (!_isInAttackRange) {
            MoveTo(AttackTarget.transform.position);
        } else {
            TakeAim();
        }
    }

    public override void TakeAim() {
		base.TakeAim();
    }

    private void OnTriggerEnter(Collider other) {

		//Debug.Log($"{other.gameObject.name} entered Enemy range");
		
		if (other.CompareTag(Globals.UNIT_TAG)) {
			//Debug.Log("Enemy registered attack");
			Unit _unit = other.GetComponent<Unit>();
			if (!_unit) {
				//Debug.Log($"No unit found on {other.gameObject.name}");
				return;
			}

			//Debug.Log($"Enemy attacking {_unit.UnitStats.Name}");
			AddTarget(_unit);
		}
	}

    private void OnTriggerExit(Collider other) {
        
		if (other.CompareTag(Globals.UNIT_TAG)) {
            //Debug.Log("Enemy registered attack");
            Unit _unit = other.GetComponent<Unit>();
            if (!_unit) {
                //Debug.Log($"No unit found on {other.gameObject.name}");
                return;
            }
			RemoveTarget(_unit);
        }

    }

	private void AddTarget(Unit unit) {

		if (!_targets.Contains(unit)) {
			_targets.Add(unit);
		}

		SetTarget(_targets[0]);

	}

	private void RemoveTarget(Unit unit) {

		_targets.Remove(unit);

		if (_targets.Count > 0) {
			SetTarget(_targets[0]);
		} else {
			ClearTarget();
		}

	}

}
