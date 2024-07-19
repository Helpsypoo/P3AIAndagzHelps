using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour {

    [SerializeField] private UnitStats _unitStats;

    [SerializeField] private HealthDisplay _healthDisplay;

    [SerializeField] GameObject _selectionIndicator;
    [SerializeField] GameObject _mesh;
    
    private UnitState _unitState;
    private NavMeshAgent _navAgent;
    private TickEntity _tickEntity;
    
    private float _health;
    private float _timeAtLastCheck;
    private float _timeInShade;

    private Coroutine healthRegen;
    
    private void Awake() {
        
        _navAgent = GetComponent<NavMeshAgent>();
        if (_navAgent == null) {
            throw new System.Exception($"Cannot find NavMeshAgent on unit {transform.name}.");
        }

        _tickEntity = GetComponent<TickEntity>();
    }

    private void Start() {
        _tickEntity.AddToTickEventManager();
        ChangeHealth(_unitStats.MaxHealth - _health, true);
        _mesh.SetActive(true);
    }

    private void Update() {
        LookWhereYoureGoing();
    }

    /// <summary>
    /// If we have path, turn the unit to face the direction they are going.
    /// </summary>
    private void LookWhereYoureGoing() {
        if (!_navAgent.hasPath || _unitState == UnitState.Dead) {
            return;
        }

        Vector3 lookTarget = _navAgent.nextPosition;
        lookTarget.y = transform.position.y;
        transform.LookAt(lookTarget);
    }

    /// <summary>
    /// Sends unit to a new destination.
    /// </summary>
    /// <param name="destination">The location the unit will attempt to move to.</param>
    public void MoveTo(Vector3 destination) {
        if(_unitState == UnitState.Dead) {
            return;
        }
        _navAgent.SetDestination(destination);
    }

    /// <summary>
    /// Called when this unit is selected by the player.
    /// </summary>
    public void Select() {
        _selectionIndicator.SetActive(true);
    }

    /// <summary>
    /// Called when this unit becomes deselected.
    /// </summary>
    public void Deselect() {
        _selectionIndicator?.SetActive(false);
    }

    /// <summary>
    /// Called via the TickEntity system. A periodic check
    /// </summary>
    public void PeriodicUpdate() {
        float _timeSinceLastCheck = Time.time - _timeAtLastCheck;
        CheckLightingStatus(_timeSinceLastCheck);
        _timeAtLastCheck = Time.time;
    }
    
    /// <summary>
    /// Checks if the object is in shade or not
    /// </summary>
    private void CheckLightingStatus(float _timeSinceLastCheck) {
        Light mainLight = RenderSettings.sun;
        Vector3 lightDir = -mainLight.transform.forward;
        //Debug.DrawLine(transform.position, transform.position + lightDir * 100f, Color.red, 1f);
        bool isInShadow = Physics.Raycast(transform.position, lightDir, out RaycastHit _hitInfo, 100f, GameManager.Instance.ShadeLayerMask);
        if (!isInShadow) {
            //Debug.Log(gameObject.name + " is NOT in shadow.");
            ChangeHealth(GameManager.Instance.SunDamagePerSecond * _timeSinceLastCheck);
        } else {
            //Debug.Log(gameObject.name + $" is shaded by {_hitInfo.transform.gameObject.name}.");
        }
    }
    

    /// <summary>
    /// Sets the state of the current unit
    /// </summary>
    public void SetState(UnitState _newState) {
        if (_newState == _unitState) {
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
        
        _unitState = _newState;
    }

    public void ChangeHealth(float _amount, bool _hiddenDisplayUpdate = false) {
        _health = Mathf.Clamp(_health + _amount, 0, _unitStats.MaxHealth);
        
        if (_hiddenDisplayUpdate) {
            _healthDisplay.HiddenHealthDisplayUpdate(_health, _unitStats.MaxHealth);
        } else {
            _healthDisplay.UpdateHealthDisplay(_health, _unitStats.MaxHealth);
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
        yield return new WaitForSeconds(_unitStats.HealthRegenDelay);
        
        while (_health < _unitStats.MaxHealth) {
            ChangeHealth(_unitStats.HealthRegenRate * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
    
    public void Revive() {
        ChangeHealth(_unitStats.MaxHealth - _health);
        SetState(UnitState.Idle);
        _mesh.SetActive(true); //TODO: replace this with a revive anim
        _tickEntity.AddToTickEventManager();
    }

    private void Die() {
        //Stop any current health regen
        if (healthRegen != null) {
            StopCoroutine(healthRegen);
        }
        
        _mesh.SetActive(false); //TODO: replace this with a death anim
        _tickEntity.RemoveFromTickEventManager();
    }
}


