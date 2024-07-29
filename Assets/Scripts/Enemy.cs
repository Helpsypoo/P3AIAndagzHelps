using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Enemy : Unit {
	[SerializeField] private TargetMode _targetMode;
	[SerializeField] private List<Unit> _targets = new();
	

	public override void Awake() {
		transform.SetParent(null);
		base.Awake();
	}
	public override void Start() {
		GameManager.Instance.EnemyTotal++;
		
		base.Start();
		
		//Debug.Log($"Running Start on Enemy");
		if (!UnitStats) {
			//Debug.Log($"No UnitStats on Enemy");
			return;
		}
		
		//Debug.Log($"Enemy total: {GameManager.Instance.EnemyTotal}");
		SphereCollider _sphereCollider = GetComponent<SphereCollider>();
		if (!_sphereCollider) {
			//Debug.Log($"No SphereCollider on Enemy");
			return;
		}
		
		_sphereCollider.radius = UnitStats.AttackRange;
	}

	protected override void ProcessAttack() {
		PrioritizeTargets();

        bool _isInAttackRange;
        if (!AttackTarget) {
	        return;
        }
        if (UnitStats.Speed > 0) {
	        _isInAttackRange = Vector3.Distance(AttackTarget.transform.position, transform.position) <= UnitStats.AttackRange;
        } else {
	        _isInAttackRange = Vector3.Distance(AttackTarget.transform.position, transform.position) <= UnitStats.AttackRange + 2; //give turrets a slightly larger attack range to account for colliders
        }

        //_anim?.SetBool(hasAttackTargetInRange, _isInAttackRange);

        if (!_isInAttackRange) {
            MoveTo(AttackTarget.transform.position);
        } else {
            TakeAim();
        }
    }

    private void OnTriggerEnter(Collider other) {
	    //Debug.Log($"{other.gameObject.name} entered Enemy range");
		
		if (IsTargetableUnit(other.tag)) {
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
        
		if (IsTargetableUnit(other.tag)) {
            //Debug.Log("Enemy registered attack");
            Unit _unit = other.GetComponent<Unit>();
            if (!_unit) {
                //Debug.Log($"No unit found on {other.gameObject.name}");
                return;
            }

            if (_unit.State == UnitState.Dead || UnitStats.Speed <= 0) {
	            RemoveTarget(_unit);
            }
		}
    }

    public override void ChangeHealth(float _amount, bool _hiddenDisplayUpdate = false, Unit _lastDamager = null) {
	    if (_lastDamager) {
		    LastDamagedBy = _lastDamager;
		    AlertNearbyEnemies();
		    if (!AttackTarget) {
			    AddTarget(_lastDamager);
		    }
	    }
	    
	    base.ChangeHealth(_amount, _hiddenDisplayUpdate, _lastDamager);
    }

	private void AddTarget(Unit unit) {

		if (_targets.Contains(unit)) {
			return;
		}
		
		_targets.Add(unit);
		PrioritizeTargets();

	}

	private void RemoveTarget(Unit unit) {

		_targets.Remove(unit);

		if (_targets.Count > 0) {
			PrioritizeTargets();
		} else {
			ClearTarget();
		}

	}
	
	private void AlertNearbyEnemies() {
		Collider[] _colliders = Physics.OverlapSphere(transform.position, UnitStats.AlertRange);
		foreach (Collider _col in _colliders) {
			if (_col.CompareTag(Globals.ENEMY_TAG)) {
				Enemy _unit = _col.GetComponent<Enemy>();
				if (_unit != this) {
					//Debug.Log($"{_unit.UnitStats.name} was alerted by {UnitStats.name} from {UnitStats.AlertRange} radius");
					_unit.AddTarget(LastDamagedBy);
				}
			}
		}
	}
	
	private bool IsTargetableUnit(string _tag) {
		switch (_targetMode) {
			default:
			case TargetMode.FirstAny:
			case TargetMode.LowestHealthAny:
			case TargetMode.ClosestAny:
				return _tag.ContainsInsensitive(Globals.UNIT_TAG) || _tag.ContainsInsensitive(Globals.LIBERATED_TAG);
			case TargetMode.FirstUnit:
			case TargetMode.Damager:
			case TargetMode.LowestHealthUnit:
			case TargetMode.ClosestUnit:
				return _tag.ContainsInsensitive(Globals.UNIT_TAG);
			case TargetMode.FirstLiberated:
				return _tag.ContainsInsensitive(Globals.LIBERATED_TAG);

		}
	}

	private void PrioritizeTargets() {
		if (_targets.Count <= 0) {
			return;
		}
		
		List<Unit> unitsToRemove = new List<Unit>();
		
		foreach (Unit unit in _targets) {
			if (unit.State == UnitState.Dead) {
				unitsToRemove.Add(unit);
			}
		}
		
		foreach (Unit unit in unitsToRemove) {
			RemoveTarget(unit);
		}
		
		switch (_targetMode) {
			default:
			case TargetMode.FirstUnit:
			case TargetMode.FirstLiberated:
			case TargetMode.FirstAny:
				if (_targets.Count > 0) {
					SetTarget(_targets[0]);
				}
				break;
			case TargetMode.Damager:
				if (LastDamagedBy != null) {
					SetTarget(LastDamagedBy);
				}
				break;
			case TargetMode.LowestHealthAny:
			case TargetMode.LowestHealthUnit:
				SetTarget(GetLowestHealthTarget());
				break;
			case TargetMode.ClosestUnit:
			case TargetMode.ClosestAny:
				SetTarget(GetClosestTarget());
				break;
		}
	}
	
	
	
	private Unit GetLowestHealthTarget() {
		Unit lowestHealthUnit = null;
		float lowestHealth = float.MaxValue;

		foreach (Unit unit in _targets) {
			if (unit != null && unit.UnitStats != null) {
				float health = unit.Health;
				if (health < lowestHealth) {
					lowestHealth = health;
					lowestHealthUnit = unit;
				}
			}
		}

		return lowestHealthUnit;
	}
	
	private Unit GetClosestTarget() {
		Unit closestUnit = null;
		float closestDistance = float.MaxValue;

		foreach (Unit unit in _targets) {
			if (unit != null) {
				float distance = Vector3.Distance(transform.position, unit.transform.position);
				if (distance < closestDistance) {
					closestDistance = distance;
					closestUnit = unit;
				}
			}
		}

		return closestUnit;
	}
}
