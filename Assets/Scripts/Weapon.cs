using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {
    [SerializeField] private ParticleSystem _muzzleFlashFX;
    [SerializeField] private Rigidbody _tracerPrefab;

    public void Fire() {
        //Debug.Log($"Running fire on weapon");
        _muzzleFlashFX?.Play();
        Rigidbody _rb = Instantiate(_tracerPrefab, _muzzleFlashFX.transform.position, _muzzleFlashFX.transform.rotation);
        //_rb.transform.localScale = _muzzleFlashFX.transform.localScale * 3f;
        _rb.AddForce(_rb.transform.forward * 4000f);
    }
}
