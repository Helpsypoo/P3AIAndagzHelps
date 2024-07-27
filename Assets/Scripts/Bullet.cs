using System;
using UnityEngine;

public class Bullet : MonoBehaviour {
    [HideInInspector] public Unit Shooter;
    [HideInInspector] public Vector3 ShotLocation;
    [HideInInspector] public Unit Target;
    [SerializeField] private float maxLife = 3f;

    [HideInInspector] public Rigidbody Rb;

    private void Awake() {
        Rb = GetComponent<Rigidbody>();
    }

    private void OnEnable() {
        Invoke("ReturnToPool", maxLife);
    }

    private void Update() {
        float _targetAndBulletDist = Vector3.Distance(Target.transform.position, transform.position);
        if (_targetAndBulletDist <= 1f) {
            Target.ChangeHealth(Shooter.UnitStats.AttackDamage, false, Shooter);
            ReturnToPool();
        }
    }

    private void ReturnToPool() {
        CancelInvoke();
        if(GameManager.Instance && GameManager.Instance.BulletStash) {GameManager.Instance.BulletStash.ReturnBullet(this);}
    }
}
