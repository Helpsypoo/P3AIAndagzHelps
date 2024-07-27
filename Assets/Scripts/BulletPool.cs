using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour {

    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private int _poolSize;
    private Queue<Bullet> _pool = new();

    private void Awake() {
        
        // Populate our pool with bullets.
        for (int i = 0; i < _poolSize; i++) {

            CreateBullet();

        }

    }

    public Bullet GetBullet(Vector3 position, Quaternion rotation) {

        if (_pool.Count <= 0 ) {
            Debug.LogWarning($"Ran out of bullets in pool, instantiating new bullet. Consider increasing size of pool. (Current size: {_poolSize}");
            CreateBullet();
        }

        Bullet bullet = _pool.Dequeue();
        bullet.transform.SetParent(null);
        bullet.transform.SetLocalPositionAndRotation(position, rotation);
        bullet.gameObject.SetActive(true);
        return bullet;

    }

    public void ReturnBullet(Bullet bullet) {
        bullet.gameObject.SetActive(false);
        bullet.transform.SetParent(transform);
        bullet.Rb.velocity = Vector3.zero;
        _pool.Enqueue(bullet);
    }

    private void CreateBullet() {
        Bullet bullet = Instantiate(_bulletPrefab, transform);
        bullet.gameObject.SetActive(false);
        _pool.Enqueue(bullet);
    }

}
