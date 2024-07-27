using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Nova : Unit {
	private List<Unit> _unitsToHeal = new List<Unit>();
	[SerializeField] private HealthDisplay _reviveDisplay;
	[SerializeField] private float _reviveTimer;

	private SphereCollider _sphereCollider;
	private DecalProjector _decalProjector;

	private Coroutine reviveCoroutine;

	public override void Awake() {
		base.Awake();
		
		_sphereCollider = GetComponent<SphereCollider>();
		_decalProjector = GetComponentInChildren<DecalProjector>();
	}

	public override void Start() {
		base.Start();
		
		if (!UnitStats) {
			return;
		}

		if(_sphereCollider) {_sphereCollider.radius = UnitStats.ActionRange/2;}
		if(_decalProjector) {_decalProjector.size = new Vector3(UnitStats.ActionRange +.3f, UnitStats.ActionRange +.3f, _decalProjector.size.z);}
	}

	public override void PeriodicUpdate() {
		base.PeriodicUpdate();

		List<Unit> unitsToRemove = new List<Unit>();

		if (Health < UnitStats.MaxHealth && !_unitsToHeal.Contains(this)) {
			_unitsToHeal.Add(this);
		}
		
		foreach (Unit _unit in _unitsToHeal) {
			if (_unit.UnitStats == null || _unit.Health <= 0) {
				unitsToRemove.Add(_unit);
				continue;
			}
			//Debug.Log($"{TimeSinceLastCheck} * {UnitStats.HealthRegenRate}");
			if (_unit.Health < _unit.UnitStats.MaxHealth) {
				_unit.ChangeHealth(TimeSinceLastCheck * UnitStats.HealthRegenRate);
			}
		}
		
		foreach (Unit unit in unitsToRemove) {
			_unitsToHeal.Remove(unit);
		}
	}
	
	public override void PerformAction(Vector3 position, Transform target = null) {
		if (Health <= 0) {
			return;
		}
		
		base.PerformAction(position, target);

		Collider[] _colliders = Physics.OverlapSphere(transform.position, UnitStats.ActionRange);
		List<Unit> _unitsToRes = new List<Unit>();
		foreach (Collider _col in _colliders) {
			if (_col.CompareTag(Globals.UNIT_TAG) || _col.CompareTag(Globals.LIBERATED_TAG)) {
				Unit _unit = _col.GetComponent<Unit>();
				if (_unit.Health <= 0) {
					_unitsToRes.Add(_unit);
				}
			}
		}

		if (reviveCoroutine != null) {
			StopCoroutine(reviveCoroutine);
		}
		//Debug.Log($"Sending Units to revive {_unitsToRes.Count}");
		reviveCoroutine = StartCoroutine(Revive(_unitsToRes));
	}

	IEnumerator Revive(List<Unit> _unitsToRevive) {
		_reviveTimer = 0;
		SetState(UnitState.Locked);
		
		while (_reviveTimer < Globals.REVIVE_TIMER) {
			_reviveDisplay.UpdateHealthDisplay(_reviveTimer, Globals.REVIVE_TIMER);
			_reviveTimer += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		
		//Debug.Log($"Units to revive {_unitsToRevive.Count}");
		foreach (Unit _unit in _unitsToRevive) {
			_unit.Revive();
		}
		
		SetState(UnitState.Idle);
	}

	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag(Globals.ENEMY_TAG)) {
			return;
		}
		
		Unit _unit = other.GetComponent<Unit>();
		if (!_unit) {
			return;
		}

		if (!_unitsToHeal.Contains(_unit)) {
			_unitsToHeal.Add(_unit);
		}
		
		if (_unit.Health < _unit.UnitStats.MaxHealth) {
			if (_unit.HealFX) {_unit.HealFX.gameObject.SetActive(true);}
		}
	}

	private void OnTriggerExit(Collider other) {
		if (other.CompareTag(Globals.ENEMY_TAG)) {
			return;
		}
		
		Unit _unit = other.GetComponent<Unit>();
		if (!_unit) {
			return;
		}

		if (_unitsToHeal.Contains(_unit)) {
			if (_unit.HealFX) {_unit.HealFX.gameObject.SetActive(false);}
			_unitsToHeal.Remove(_unit);
		}
	}
}
