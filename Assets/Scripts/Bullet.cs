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
        float _shooterAndTargetDist = Vector3.Distance(ShotLocation, Target.transform.position);
        float _shooterAndBulletDist = Vector3.Distance(ShotLocation, transform.position);
        if (_shooterAndBulletDist >= _shooterAndTargetDist -.5f) {
            Target.ChangeHealth(Shooter.UnitStats.AttackDamage);
            GameManager.Instance.BulletStash.ReturnBullet(this);
        }
    }
}
