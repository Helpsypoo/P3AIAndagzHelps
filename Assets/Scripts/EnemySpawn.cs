using System;
using UnityEngine;

public class EnemySpawn : MonoBehaviour {
    [SerializeField] private Unit EnemySpawnPrefab;
    private Unit _spawnedUnit;

    private void Start() {
        _spawnedUnit = Instantiate(EnemySpawnPrefab, transform.position, transform.rotation);
        gameObject.SetActive(false);
    }

    private void OnDestroy() {
        if(_spawnedUnit) Destroy(_spawnedUnit.gameObject);
    }
}
