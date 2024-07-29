using UnityEngine;

public class Weapon : MonoBehaviour {
    [SerializeField] private ParticleSystem[] _muzzleFlashFX;
    [SerializeField] private AudioClip _fireSound;

    private int _fireCycle;

    private Unit unit;

    private void Awake() {
        unit = GetComponentInParent<Unit>();
    }

    public void Fire() {
        //Debug.Log($"Running fire on weapon");
        if (!_muzzleFlashFX[_fireCycle]) {
            return;
        }
        
        _muzzleFlashFX[_fireCycle].Play();
        //Bullet _bullet = Instantiate(_tracerPrefab, _muzzleFlashFX.transform.position, _muzzleFlashFX.transform.rotation);
        Bullet _bullet = GameManager.Instance.BulletStash.GetBullet(_muzzleFlashFX[_fireCycle].transform.position, _muzzleFlashFX[_fireCycle].transform.rotation);
        //_rb.transform.localScale = _muzzleFlashFX.transform.localScale * 3f;
        if (!unit.AttackTarget) {
            _bullet.gameObject.SetActive(false);
            return;
        }

        _bullet.Shooter = unit;
        _bullet.Target = unit.AttackTarget;
        _bullet.ShotLocation = unit.transform.position;
        _bullet.Damage = unit.UnitStats.AttackDamage;
        Vector3 _attackPos = unit.AttackTarget.transform.position;
        _attackPos.y = 1.25f;
        Vector3 _attackDir = unit.AttackTarget.transform.position - _muzzleFlashFX[_fireCycle].transform.position;
        _attackDir = _attackDir.normalized;

        Vector3 _lookAtLocation = unit.AttackTarget.transform.position;
        _lookAtLocation.y = _muzzleFlashFX[_fireCycle].transform.position.y;
        _bullet.Rb.transform.LookAt(_lookAtLocation);
        
        _bullet.Rb.AddForce(_attackDir * Globals.BULLET_SPEED);

        if (_fireSound != null) {
            AudioManager.Instance.Play(_fireSound, MixerGroups.SFX, Vector2.one, 1f, transform.position);
        }

        _fireCycle = (_fireCycle + 1) % _muzzleFlashFX.Length;
    }
}
