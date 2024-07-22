using UnityEngine;

public class Weapon : MonoBehaviour {
    [SerializeField] private ParticleSystem _muzzleFlashFX;
    [SerializeField] private Bullet _tracerPrefab;

    private Unit unit;

    private void Awake() {
        unit = GetComponentInParent<Unit>();
    }

    public void Fire() {
        //Debug.Log($"Running fire on weapon");
        _muzzleFlashFX?.Play();
        Bullet _bullet = Instantiate(_tracerPrefab, _muzzleFlashFX.transform.position, _muzzleFlashFX.transform.rotation);
        //_rb.transform.localScale = _muzzleFlashFX.transform.localScale * 3f;
        if (!unit.AttackTarget) {
            _bullet.gameObject.SetActive(false);
            return;
        }

        _bullet.Shooter = unit;
        _bullet.Target = unit.AttackTarget;
        _bullet.ShotLocation = unit.transform.position;
        
        Vector3 _attackDir = unit.AttackTarget.transform.position - _muzzleFlashFX.transform.position;
        _attackDir = _attackDir.normalized;

        Vector3 _lookAtLocation = unit.AttackTarget.transform.position;
        _lookAtLocation.y = _muzzleFlashFX.transform.position.y;
        _bullet.Rb.transform.LookAt(_lookAtLocation);
        
        _bullet.Rb.AddForce(_attackDir * 4000f);
    }
}
