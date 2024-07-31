using UnityEngine;
using System.Collections;

public class Liberated : Unit {

    private Rigidbody _rigidbody;

    public new bool IsLeader;
    public bool IsPrisoner;
    public SphereCollider LiberationRange;

    public override void Awake() {
        base.Awake();
        _rigidbody = GetComponent<Rigidbody>();
    }

    public override void Start() {
        base.Start();
        GameManager.Instance.LiberatedTotal++;
    }

    public void Shackle() {
        IsPrisoner = true;
        LiberationRange.gameObject.SetActive(true);
    }

    public void FreeGroup() {
        Free();
        FreeNearby();
    }
    
    public void Free() {
        IsPrisoner = false;
        LiberationRange.gameObject.SetActive(false);
        GameManager.Instance.JoinLiberated(this);
    }

    private void FreeNearby() {
        Collider[] _colliders = Physics.OverlapSphere(transform.position, 10f);
        foreach (Collider _col in _colliders) {
            Liberated _liberated = _col.transform.GetComponent<Liberated>();
            if (_liberated && _liberated.Health > 0 && _liberated.IsPrisoner) {
                _liberated.Free();
            }
        }
    }

    public override void PeriodicUpdate() {
        base.PeriodicUpdate();

        if (Health <= 0 || IsPrisoner) {
            return;
        }
        
        if (IsLeader) {
            if (GameManager.Instance.IsProcessing) {
                MoveTo(GameManager.Instance.KillZone.position);
                return;
            }
            if (GameManager.Instance.ActiveWaypoints.Count > 0) {
                MoveTo(GameManager.Instance.ActiveWaypoints[0].transform.position);
            }
        } else {
            MoveTo(GameManager.Instance.GetFollowingPosition(this));
        }
    }

    public override void Die() {

        //if (State == UnitState.Dead) return;

        base.Die();
        GameManager.Instance.ProcessLiberatedDeath(this);
    }

    public override void Revive(bool _hiddenHealthbar = false) {
        base.Revive(_hiddenHealthbar);
        GameManager.Instance.JoinLiberated(this);
    }

    public void Launch(float force, Vector3 blastOrigin) {

        _rigidbody.isKinematic = false;

        Vector3 fuzzyFactor = Random.insideUnitSphere * 0.6f;
        Vector3 blastDirection = (transform.position - blastOrigin).normalized;
        blastDirection += fuzzyFactor;
        _rigidbody.AddForce(blastDirection * force, ForceMode.Impulse);
        StartCoroutine(AirTime(4f));

    }

    private void OnCollisionStay(Collision collision) {

        if (_isAirTime) return;

        if (collision.transform.CompareTag(Globals.WALKABLE_TAG)) {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.isKinematic = true;
        }

    }

    private bool _isAirTime;
    private IEnumerator AirTime(float time){

        _isAirTime = true;
        yield return new WaitForSeconds(time);
        _isAirTime = false;

    }

}
