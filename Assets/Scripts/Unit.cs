using System;
using System.Collections;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Unit : MonoBehaviour {

    public UnitStats UnitStats;

    [SerializeField] private HealthDisplay _healthDisplay;

    [SerializeField] MeshRenderer _selectionIndicator;
    [SerializeField] Renderer[] _renderers;
    [SerializeField] Weapon _weapon;
    [SerializeField] Rigidbody _weaponPrefab;

    public Unit FollowTarget { get; private set; }
    

    Rigidbody _looseGun;

    private Animator _anim;
  
    public UnitState State { get; private set; }
    protected NavMeshAgent _navAgent;
    private TickEntity _tickEntity;
    
    public float _health { get; private set; }
    private float _timeAtLastCheck;
    private float _timeInShade;
    private bool _attacking;

    [field: SerializeField] public bool AtDestination { get; private set; }

    private Coroutine healthRegen;

    /// <summary>
    /// The index in the SquadManager._units array.
    /// </summary>
    private int _squadNumber = 0;

    private static readonly int speed = Animator.StringToHash("Speed");
    private static readonly int hasAttackTargetInRange = Animator.StringToHash("HasAttackTargetInRange");
    public int SquadNumber => _squadNumber;
    public void UpdateSquadNumber(int number) {
        _squadNumber = number;
    }
    
    public void Awake() {
        _navAgent = GetComponent<NavMeshAgent>();
        if (_navAgent == null) {
            throw new System.Exception($"Cannot find NavMeshAgent on unit {transform.name}.");
        }

        _tickEntity = GetComponent<TickEntity>();
        _anim = GetComponentInChildren<Animator>();
    }

    public void Start() {
        _tickEntity?.AddToTickEventManager();
        ChangeHealth(UnitStats.MaxHealth - _health, true);
        SetColors();
        _navAgent.speed = UnitStats.Speed;
    }

    public virtual void Update() {
        LookWhereYoureGoing();
        if(_anim){ _anim.SetFloat(speed, _navAgent.velocity.magnitude);}
    }

    private void SetColors() {
        foreach (Renderer _rend in _renderers) {
            foreach (Material material in _rend.materials) {
                material.color = UnitStats.Colour;
            }
        }
    }

    /// <summary>
    /// If we have path, turn the unit to face the direction they are going.
    /// </summary>
    public void LookWhereYoureGoing() {
        if (!FollowTarget && (!_navAgent.hasPath || State == UnitState.Dead)) {
            return;
        }

        Vector3 lookTarget = FollowTarget ? FollowTarget.transform.position : _navAgent.nextPosition;
        lookTarget.y = transform.position.y;
        transform.LookAt(lookTarget);
    }

    public void UpdateDestinationStatus() {

        bool prevDestination = AtDestination;
        // If we don't currently have a path we are at our destination (or we can't get there).
        if (!_navAgent.hasPath) {
            AtDestination = true;
        } else {
            float sqrThreshold = Globals.MIN_ACTION_DIST * Globals.MIN_ACTION_DIST;
            AtDestination = (_navAgent.destination - transform.position).sqrMagnitude <= sqrThreshold;
        }

        // If we are now at our destination and we weren't before, throw a call to SelectionCursor to see if it needs to deactivate.
        if (AtDestination && prevDestination != AtDestination) {
            GameManager.Instance.SelectionMarker.CheckAction(transform.position);
        }

    }

    /// <summary>
    /// Sends unit to a new destination.
    /// </summary>
    /// <param name="destination">The location the unit will attempt to move to.</param>
    public void MoveTo(Vector3 destination) {
        _navAgent.SetDestination(destination);
    }

    /// <summary>
    /// Called whenever this unit is called upon to perform an action (whatever their action is).
    /// This function should contain any code that needs to be run for ANY unit regardless of type.
    /// Code specific to an individual unit type should be run in an inherited override.
    /// </summary>
    /// <param name="position">The position that the action should be performed at (where the click happened)</param>
    /// <param name="target">The thing that was clicked on. Can be null.</param>
    public virtual void PerformAction(Vector3 position, Transform target = null) {
        Debug.Log($"{UnitStats.Name}'s action has been called.");
    }

    /// <summary>
    /// Called when this unit is selected by the player.
    /// </summary>
    public void Select() {
        _selectionIndicator.gameObject.SetActive(true);
    }

    /// <summary>
    /// Called when this unit becomes deselected.
    /// </summary>
    public void Deselect() {
        _selectionIndicator.gameObject.SetActive(false);
    }

    /// <summary>
    /// Called via the TickEntity system. A periodic check
    /// </summary>
    public void PeriodicUpdate() {
        float _timeSinceLastCheck = Time.time - _timeAtLastCheck;
        CheckLightingStatus(_timeSinceLastCheck);
        UpdateDestinationStatus();
        if (FollowTarget){
            ProcessAttack();
            ProcessFriendlyFollow();
        }
        
        _timeAtLastCheck = Time.time;
    }
    
    /// <summary>
    /// Checks if the object is in shade or not
    /// </summary>
    public void CheckLightingStatus(float _timeSinceLastCheck) {
        Light mainLight = RenderSettings.sun;
        Vector3 lightDir = -mainLight.transform.forward;
        //Debug.DrawLine(transform.position, transform.position + lightDir * 100f, Color.red, 1f);
        bool isInShadow = Physics.Raycast(transform.position, lightDir, out RaycastHit _hitInfo, 100f, GameManager.Instance.ShadeLayerMask);
       
        if (isInShadow) {
            //Debug.Log(gameObject.name + $" is shaded by {_hitInfo.transform.gameObject.name}.");
            return;
        }

        //Debug.Log(gameObject.name + " is NOT in shadow.");
        ChangeHealth(GameManager.Instance.SunDamagePerSecond * _timeSinceLastCheck);
    }
    

    /// <summary>
    /// Sets the state of the current unit
    /// </summary>
    public void SetState(UnitState _newState) {
        if (_newState == State) {
            return;
        }

        switch (_newState) {
            default:
            case UnitState.Idle:
                break;
            case UnitState.Moving:
                break;
            case UnitState.Attacking:
                break;
            case UnitState.Dead:
                Die();
                break;
        }
        
        State = _newState;
    }

    public void ChangeHealth(float _amount, bool _hiddenDisplayUpdate = false) {
        _health = Mathf.Clamp(_health + _amount, 0, UnitStats.MaxHealth);
        
        if (_hiddenDisplayUpdate) {
            _healthDisplay.HiddenHealthDisplayUpdate(_health, UnitStats.MaxHealth);
        } else {
            _healthDisplay.UpdateHealthDisplay(_health, UnitStats.MaxHealth);
        }
        
        if (_health <= 0) {
            SetState(UnitState.Dead);
            _healthDisplay.ForceHealthDisplay(false);
            return;
        }
    
        if (_amount > 0) {
            //TODO: Play heal effect
        } else {
            //TODO: Play damage effect
            
            //Reset any health regen delay
            if (healthRegen != null) {
                StopCoroutine(healthRegen);
            }

            healthRegen = StartCoroutine(StartRegen());
        }
    }

    IEnumerator StartRegen() {
        yield return new WaitForSeconds(UnitStats.HealthRegenDelay);
        
        while (_health < UnitStats.MaxHealth) {
            ChangeHealth(UnitStats.HealthRegenRate * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
    
    [ContextMenu("Revive")]
    public void Revive() {
        ChangeHealth(UnitStats.MaxHealth - _health);
        SetState(UnitState.Idle);
        _navAgent.isStopped = false;
        transform.tag = Globals.UNIT_TAG;
        _tickEntity?.AddToTickEventManager();
        
        _looseGun?.gameObject.SetActive(false);
        _weapon?.gameObject.SetActive(true);
        
        if (_anim) {
            _anim.Play("Idle", 0);
            _anim.transform.localPosition = Vector3.zero;
            _anim.transform.rotation = Quaternion.identity;
        }
    }

    public void SetFollowTarget(Unit followTarget) {
        Debug.Log($"Set follow target to {followTarget.UnitStats.Name}");
        this.FollowTarget = followTarget;
        SetStopDistance(FollowTarget.UnitStats.IsEnemy ? UnitStats.AttackRange : Globals.FOLLOW_DIST);
    }

    public void ClearFollowTarget() {
        FollowTarget = null;
        StandDown();
    }

    [ContextMenu("Toggle Shoot")]
    public void ToggleShoot() {
        _anim?.SetBool(hasAttackTargetInRange, !_anim.GetBool(hasAttackTargetInRange)); //this calls the Fire() function at a specific frame of the anim
    }

    public void Fire() {
        _weapon.Fire();
    }

    private void Die() {
        //Stop any current health regen
        if (healthRegen != null) {
            StopCoroutine(healthRegen);
        }

        _navAgent.isStopped = true;
        
        if(_anim){_anim.Play("Death", 0);}
        _weapon?.gameObject.SetActive(false);
        if (!_looseGun && _weaponPrefab) {
            _looseGun = Instantiate(_weaponPrefab, _weapon.transform.position, _weapon.transform.rotation);
        } else if (_looseGun) {
            _looseGun.transform.position = _weapon.transform.position;
            _looseGun.transform.rotation = _weapon.transform.rotation;
            _looseGun.gameObject.SetActive(true);
        }
        
        Vector3 _randomUpwardForce = new Vector3(Random.Range(-1f, 1f), Random.Range(3f, 6f), Random.Range(-1f, 1f));
        _looseGun?.AddForce(_randomUpwardForce);
        _looseGun?.AddTorque(Random.rotation.eulerAngles * Random.Range(-2f, 2f));

        transform.tag = Globals.DOWNED_UNIT_TAG;
        _tickEntity?.RemoveFromTickEventManager();

        // If we are the currently selected unit, tell the squadmanager to select another unit.
        if (SquadManager.Instance.SelectedUnit == this) {
            Debug.Log("Asking Squadmanager to select a new unit");
            SquadManager.Instance.SelectNextAvailableUnit();
        }

    }

    public void SetStopDistance(float _value) {
        _navAgent.stoppingDistance = _value;
    }

    private void ProcessAttack() {
        if (!FollowTarget.UnitStats || !FollowTarget.UnitStats.IsEnemy || FollowTarget._health <= 0 || !UnitStats) {
            _anim?.SetBool(hasAttackTargetInRange, false);
            return;
        }
        
        bool _isInAttackRange = Vector3.Distance(FollowTarget.transform.position, transform.position) <= UnitStats.AttackRange;
        _anim?.SetBool(hasAttackTargetInRange, _isInAttackRange);
        if (!_isInAttackRange) {
            MoveTo(FollowTarget.transform.position);
        }
    }
    
    private void ProcessFriendlyFollow() {
        if (FollowTarget.UnitStats.IsEnemy || FollowTarget._health <= 0) {
            return;
        }

        //Debug.Log($"Friendly following {FollowTarget.UnitStats.Name}");
        MoveTo(FollowTarget.FollowPosition);
    }

    public Vector3 FollowPosition => transform.position - (transform.forward * Globals.FOLLOW_DIST);

    /// <summary>
    /// Sets this unit to attack whenever it is in range of the target but only if this unit has a weapon to attack with.
    /// </summary>
    public void Attack(Unit unit) {
        if (_weapon != null) {
            _attacking = true;
            SetFollowTarget(unit);
        }
    }

    /// <summary>
    /// Makes this unit chill (stop attacking things).
    /// </summary>
    public void StandDown() {
        _attacking = false;
        FollowTarget = null;
        SetStopDistance(0);
    }

}


