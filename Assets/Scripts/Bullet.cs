using UnityEngine;

public class Bullet : MonoBehaviour {
    [HideInInspector] public Unit Shooter;
    [HideInInspector] public Vector3 ShotLocation;
    [HideInInspector] public Unit Target;

    [HideInInspector] public Rigidbody Rb;

    private void Awake() {
        Rb = GetComponent<Rigidbody>();
    }

    private void Update() {
        float _targetAndBulletDist = Vector3.Distance(Target.transform.position, transform.position);
        if (_targetAndBulletDist <= 1f) {
            Target.ChangeHealth(Shooter.UnitStats.AttackDamage);
            GameManager.Instance.BulletStash.ReturnBullet(this);
        }
    }
}
