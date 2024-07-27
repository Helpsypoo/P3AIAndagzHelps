using System;
using System.Collections;
using System.ComponentModel;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Unit : MonoBehaviour {

    public UnitStats UnitStats;

    [SerializeField] private HealthDisplay _healthDisplay;

    [SerializeField] GameObject _selectionIndicator;
    [SerializeField] GameObject _reviveSelectionIndicator;
    [SerializeField] GameObject _leaderSelectionIndicator;
    [SerializeField] Renderer[] _renderers;
    [SerializeField] Weapon _weapon;
    [SerializeField] Rigidbody _weaponPrefab;
    public Transform LeftFoot;
    public Transform RightFoot;

    private CapsuleCollider _hitbox;
    private float _healthTickCooldownDuration = .4f;
    private bool _canHealthTick;

    public Unit FollowTarget { get; private set; }
    public Unit AttackTarget { get; private set; }

    protected int _abilityCharges = 0;     // The remaining of times this unit can perform their special action.
    public int AbilityCharges => _abilityCharges;

    // Is true if this unit is the currently selected/active unit.
    public bool IsLeader => SquadNumber == SquadManager.Instance.UnitIndex;
    

    Rigidbody _looseGun;

    [SerializeField] private Animator _anim;
  
    [field: SerializeField] public UnitState State { get; private set; }
    protected NavMeshAgent _navAgent;
    private TickEntity _tickEntity;
    
    public float Health { get; private set; }
    [HideInInspector] public float TimeAtLastCheck;
    [HideInInspector] public float TimeSinceLastCheck;
    private float _timeInShade;
    private bool _attacking;
    private float _attackCooldown;

    private bool _isInShadow;

    private Light _mainLight;
    //private bool _isReviving => _reviveTimer > 0f;

    [field: SerializeField] public bool AtDestination { get; private set; }

    private Coroutine healthRegen;
    [SerializeField] private LineRenderer _sunBeam;
    public ParticleSystem HealFX;
    [SerializeField] private ParticleSystem _deathFX;

    /// <summary>
    /// The index in the SquadManager._units array.
    /// </summary>
    private int _squadNumber = 0;

    private static readonly int speed = Animator.StringToHash("Speed");
    private static readonly int hasAttackTargetInRange = Animator.StringToHash("HasAttackTargetInRange");
    private static readonly int attackTrigger = Animator.StringToHash("Attack");
    public int SquadNumber => _squadNumber;
    public void UpdateSquadNumber(int number) {
        _squadNumber = number;
    }
    
    public virtual void Awake() {
        _navAgent = GetComponent<NavMeshAgent>();
        if (_navAgent == null) {
            Debug.LogWarning($"Cannot find NavMeshAgent on unit {transform.name}.");
        }

        _tickEntity = GetComponent<TickEntity>();
        _hitbox = GetComponent<CapsuleCollider>();
        _mainLight = RenderSettings.sun;
    }

    public virtual void Start() {
        _canHealthTick = true;
        _tickEntity?.AddToTickEventManager();
        ChangeHealth(UnitStats.MaxHealth - Health, true);
        SetColors();
        if (_navAgent) { _navAgent.speed = UnitStats.Speed;}

        if (CompareTag(Globals.UNIT_TAG)) {
            GameManager.Instance.ProcessUnitLife(this);
        }
    }

    public virtual void Update() {
        if (Health <= 0) {
            _sunBeam.gameObject.SetActive(false);
            return;
        }

        SetSunLaserDisplay();
        
        if (State == UnitState.Locked) {
            return;
        }
        
        LookWhereYoureGoing();
        if(_anim && _navAgent){ _anim.SetFloat(speed, _navAgent.velocity.magnitude);}

        if (_attackCooldown > 0f) {
            _attackCooldown -= Time.deltaTime;
        }

        /*if (AtDestination && _reviveTimer > 0f) {
            _reviveTimer -= Time.deltaTime;
            if (_reviveTimer <= 0f) {
                FollowTarget.Revive();
                _reviveDisplay.ForceHealthDisplay(false);
            }
        }

        if (_reviveTimer > 0f && _reviveTimer < Globals.REVIVE_TIMER) {
            _reviveDisplay.UpdateHealthDisplay(Globals.REVIVE_TIMER - _reviveTimer, Globals.REVIVE_TIMER);
        }*/

    }

    private void SetColors() {
        foreach (Renderer _rend in _renderers) {
            _rend.material.color = UnitStats.Colour;
        }
    }

    /// <summary>
    /// If we have path, turn the unit to face the direction they are going.
    /// </summary>
    public void LookWhereYoureGoing() {
        if (!_navAgent || (!FollowTarget && !AttackTarget && (!_navAgent.hasPath || State == UnitState.Dead))) {
            return;
        }

        Vector3 lookTarget = _navAgent.nextPosition;
        if (AttackTarget) {
            lookTarget = AttackTarget.transform.position;
        } else if (FollowTarget) {
            lookTarget = FollowTarget.transform.position;
        }

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

            //if (FollowTarget != null && FollowTarget.State == UnitState.Dead) {
            //    FollowTarget.Revive();
            //}

        }

    }

    /// <summary>
    /// Sends unit to a new destination.
    /// </summary>
    /// <param name="destination">The location the unit will attempt to move to.</param>
    public void MoveTo(Vector3 destination) {
        if (!_navAgent || !_navAgent.enabled || !_navAgent.isOnNavMesh) {
            return;
        }
        _navAgent.SetDestination(destination);
        _navAgent.isStopped = false;
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

        if (transform.CompareTag(Globals.UNIT_TAG) && State == UnitState.Dead) {
            _reviveSelectionIndicator.SetActive(true);
        } else {
            if (transform.CompareTag(Globals.UNIT_TAG) && IsLeader) {
                _leaderSelectionIndicator.SetActive(true);
            } else {
                _selectionIndicator.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Called when this unit becomes deselected.
    /// </summary>
    public void Deselect() {
        // CHECK IF PLAYER UNIT FIRST
        if (_selectionIndicator != null) {
            _selectionIndicator.SetActive(false);
        }
        if (_leaderSelectionIndicator != null) {
            _leaderSelectionIndicator.SetActive(false);
        }
        if (transform.CompareTag(Globals.UNIT_TAG)) {
            _reviveSelectionIndicator.SetActive(false);
        }
    }

    /// <summary>
    /// Toggles selection from its current state (whatever that is).
    /// </summary>
    public void ToggleSelect() {
        if(_selectionIndicator){_selectionIndicator.SetActive(!_selectionIndicator.activeSelf);}
    }

    /// <summary>
    /// Called via the TickEntity system. A periodic check
    /// </summary>
    public virtual void PeriodicUpdate() {
        if (_anim) { _anim.transform.localPosition = Vector3.zero; } //stop the animator drifting from the collider
        TimeSinceLastCheck = Time.time - TimeAtLastCheck;
        CheckLightingStatus(TimeSinceLastCheck);
        UpdateDestinationStatus();

        if (AttackTarget) {
            ProcessAttack();
        } else if (FollowTarget){ //Mirror the attack target of the unit we're following
            if (FollowTarget.AttackTarget) {
                SetTarget(FollowTarget.AttackTarget);
            } else {
                FollowTarget.AttackTarget = null;
                SetTarget(FollowTarget);
            }
            ProcessFriendlyFollow();
        }

        if (Health >= UnitStats.MaxHealth) {
            if(HealFX) HealFX.gameObject.SetActive(false);
        }
        
        TimeAtLastCheck = Time.time;
    }
    
    /// <summary>
    /// Checks if the object is in shade or not
    /// </summary>
    public void CheckLightingStatus(float _timeSinceLastCheck) {
        Vector3 lightDir = -_mainLight.transform.forward;
        //Debug.DrawLine(transform.position, transform.position + lightDir * 100f, Color.red, 1f);
        _isInShadow = Physics.Raycast(transform.position, lightDir, out RaycastHit _hitInfo, 2000f, GameManager.Instance.ShadeLayerMask);
       
        if (_isInShadow) {
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
                _navAgent.enabled = true;
                break;
            case UnitState.Moving:
                break;
            case UnitState.Attacking:
                break;
            case UnitState.Dead:
                Die();
                break;
            case UnitState.Locked:
                _navAgent.enabled = false;
                break;
        }
        
        State = _newState;
    }

    public void SetTransform(Transform _target) {
        gameObject.transform.position = _target.position;
        gameObject.transform.rotation = _target.rotation;
        _anim.transform.localRotation = Quaternion.identity;
    }

    public void ChangeHealth(float _amount, bool _hiddenDisplayUpdate = false) {
        Health = Mathf.Clamp(Health + _amount, 0, UnitStats.MaxHealth);

        if (_hiddenDisplayUpdate) {
            _healthDisplay.HiddenHealthDisplayUpdate(Health, UnitStats.MaxHealth);
        } else {
            _healthDisplay.UpdateHealthDisplay(Health, UnitStats.MaxHealth);
        }
        
        if (Health <= 0) {
            SetState(UnitState.Dead);
            _healthDisplay.ForceHealthDisplay(false);
            return;
        }
    
        if (_amount > 0) {
            if (_canHealthTick) {
                _canHealthTick = false;
                AudioManager.Instance.Play(
                    AudioManager.Instance.HealthTick,
                    MixerGroups.SFX,
                    new Vector2(Health / UnitStats.MaxHealth + .2f, Health / UnitStats.MaxHealth + .2f),
                    1f,
                    transform.position,
                    .9f);
                Invoke("ResetCanHealthTick", _healthTickCooldownDuration);
            }
        } else {

            //Reset any health regen delay
           /* if (healthRegen != null) {
                StopCoroutine(healthRegen);
            }

            healthRegen = StartCoroutine(StartRegen());*/
        }
    }

    private void ResetCanHealthTick() {
        _canHealthTick = true;
    }

    IEnumerator StartRegen() {
        yield return new WaitForSeconds(UnitStats.HealthRegenDelay);
        
        while (Health < UnitStats.MaxHealth) {
            ChangeHealth(UnitStats.HealthRegenRate * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
    
    [ContextMenu("Revive")]
    public void Revive(bool _hiddenHealthbar = false) {
        _hitbox.enabled = false; //Toggle collider to allow enemies to retarget
        TimeAtLastCheck = Time.time;
        ChangeHealth(UnitStats.MaxHealth - Health, _hiddenHealthbar);
        SetState(UnitState.Idle);
        
        if (_navAgent) {
            _navAgent.enabled = true;
            _navAgent.isStopped = false;
        }

        transform.tag = Globals.UNIT_TAG;
        _tickEntity?.AddToTickEventManager();
        
        _looseGun?.gameObject.SetActive(false);
        _weapon?.gameObject.SetActive(true);
        
        if (_anim) {
            _anim.Play("Idle", 0);
            _anim.transform.localPosition = Vector3.zero;
            _anim.transform.rotation = Quaternion.identity;
        }
        _reviveSelectionIndicator.SetActive(false);
        
        if (CompareTag(Globals.UNIT_TAG)) {
            GameManager.Instance.ProcessUnitLife(this);
        }
        
        _hitbox.enabled = true;
        _anim.transform.localRotation = Quaternion.identity;
    }

    public void SetTarget(Unit target) {
        if (target.UnitStats && !AreAllyUnits(target)) {
            this.AttackTarget = target;
            SetStopDistance(UnitStats.AttackRange);
            //Debug.Log($"{UnitStats.Name} attack target set to {target.UnitStats.Name}");
        } else {
            this.FollowTarget = target;
            //Debug.Log($"Set follow target to {target.UnitStats.Name}");
            SetStopDistance(Globals.FOLLOW_DIST);
        }
    }

    public void SetReviveTarget(Unit target) {

        //_reviveTimer = Globals.REVIVE_TIMER;
        FollowTarget = target;
        //_reviveDisplay.HiddenHealthDisplayUpdate(Globals.REVIVE_TIMER - _reviveTimer, Globals.REVIVE_TIMER);
        SetStopDistance(Globals.FOLLOW_DIST);


    }

    public void ClearTarget() {
        AttackTarget = null;
        ClearFollowTarget();
        if(_navAgent && _navAgent.enabled &&  _navAgent.isOnNavMesh) {_navAgent.isStopped = true;}
    }

    public void ClearFollowTarget() {
        FollowTarget = null;
        StandDown();
    }

    [ContextMenu("Toggle Shoot")]
    public void ToggleShoot() {
        _anim?.SetBool(hasAttackTargetInRange, !_anim.GetBool(hasAttackTargetInRange)); //this calls the Fire() function at a specific frame of the anim
    }

    public virtual void TakeAim() {
        if (_attackCooldown > 0f) return;
        _anim?.SetTrigger(attackTrigger);
        _attackCooldown = UnitStats.AttackRate;
    }

    public Vector2 Cooldown => new Vector2(_attackCooldown, UnitStats.AttackRate);

    public void Fire() {
        _weapon.Fire();
    }

    public virtual void Die() {

        // If we're already dead, we don't need to die again.
        if (State == UnitState.Dead) return;
        if(_deathFX) {_deathFX.Play();}
        _tickEntity?.RemoveFromTickEventManager();
        //Stop any current health regen
        if (healthRegen != null) {
            StopCoroutine(healthRegen);
        }

        FollowTarget = null;
        StandDown();
        Deselect();

        if (_navAgent & _navAgent.enabled && _navAgent.isOnNavMesh) {
            _navAgent.isStopped = true;
            _navAgent.enabled = false;
        }

        AudioClip[] _deathSFX;
        if (CompareTag(Globals.UNIT_TAG)) {
            GameManager.Instance.ProcessUnitLife(this);
            _deathSFX = AudioManager.Instance.UnitDeath;
        } else if(CompareTag(Globals.ENEMY_TAG)){
            _deathSFX = AudioManager.Instance.EnemyDeath;
            _hitbox.enabled = false;
        } else {
            _deathSFX = AudioManager.Instance.LiberatedDeath;
        }
        
        AudioManager.Instance.Play(_deathSFX, MixerGroups.SFX, new Vector2(.9f,1.1f), 2f, transform.position);

        if (_anim) {
            _anim.SetBool(hasAttackTargetInRange, false);
            _anim.Play("Death", 0);
        }
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
        
        if (SquadManager.Instance.UnitIndex >= GameManager.Instance.PlayerUnits.Count) {
            return;
        }
        
        // If we are the currently selected unit, tell the squadmanager to select another unit.
        if (SquadManager.Instance.SelectedUnit == this) {
            Debug.Log("Asking Squadmanager to select a new unit");
            SquadManager.Instance.SelectNextAvailableUnit();
        }

    }

    public void SetStopDistance(float _value) {
        if(_navAgent){_navAgent.stoppingDistance = _value;}
    }

    protected virtual void ProcessAttack() {
        //Debug.Log($"$Processing attack for {UnitStats.name}");
        if (AttackTarget.Health <= 0 || Health <= 0) { 
            //If the thing we're following cannot be attacked exit
            AttackTarget = null;
            _anim?.SetBool(hasAttackTargetInRange, false);
            
            if (FollowTarget) {
                SetTarget(FollowTarget);
            }
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
    
    private void ProcessFriendlyFollow() {

        if (FollowTarget.Health <= 0/* && !_isReviving*/) {
            FollowTarget = null;
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
            SetTarget(unit);
        }
    }

    public void SetSunLaserDisplay() {
        if (!_sunBeam) {
            return;
        }
        if (!_isInShadow) {
            Vector3[] _positions = new Vector3[] {
                _sunBeam.transform.GetChild(0).position,
                _mainLight.transform.position
            };
            _sunBeam.SetPositions(_positions);
        }
        
        _sunBeam.gameObject.SetActive(!_isInShadow);

    }

    /// <summary>
    /// Makes this unit chill (stop attacking things).
    /// </summary>
    public void StandDown() {
        _attacking = false;
        if (AttackTarget != null) {
            AttackTarget.Deselect();
            AttackTarget = null;
        }
        SetStopDistance(0);
    }

    private bool AreAllyUnits(Unit _otherUnit) {
        return UnitStats.IsEnemy == _otherUnit.UnitStats.IsEnemy;
    }

    private void OnDestroy() {
        if(_looseGun) Destroy(_looseGun.gameObject);
    }
}


