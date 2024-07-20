using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.AI.Navigation;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;

    public float SunDamagePerSecond = 2.5f;
    
    public LayerMask ShadeLayerMask;

    public Camera MainCamera { get; private set; }
    [field: SerializeField] public SelectionCursor SelectionMarker { get; private set; }

    public Level ActiveLevel;
    public Level[] Levels;

    [SerializeField] private Liberated _liberatedPrefab;
    [SerializeField] private Waypoint _waypointPrefab;
    private List<Liberated> _liberatedPool = new List<Liberated>();
    [HideInInspector] public List<Liberated> ActiveLiberated = new List<Liberated>();
    public List<Waypoint> ActiveWaypoints = new List<Waypoint>();

    private void Awake() {
        if (Instance) {
            Destroy(this);
        } else {
            Instance = this;
        }

        MainCamera = Camera.main;
    }

    private void Start() {
        CreateLevel(Levels[0]);
    }

    public void CreateLevel(Level _level) {
        StartCoroutine(CreateLevelCoroutine(_level));
    }

    public void SetActiveLevel(Level _level) {
        ActiveLevel = _level;
    }
    

    IEnumerator CreateLevelCoroutine(Level _level) {
        if (ActiveLevel != _level) {
            if(ActiveLevel){Destroy(ActiveLevel.gameObject);}
            yield return StartCoroutine(InstantiateAsync(_level));
        }
        
        yield return new WaitUntil(() => ActiveLevel);

        SpawnOrRelocatedLiberated(ActiveLevel.LiberatedSpawns);
        //TODO: spawn/move units to ActiveLevel.UnitSpawns
        
        yield return new WaitForSeconds(2f);
        //TODO: transition out of a loading screen
    }


    private void SpawnOrRelocatedLiberated(Transform[] _spawnPoints) {
        ActiveLiberated.Clear();
        
        for (int i = 0; i < _spawnPoints.Length; i++) {
            Liberated _liberated;
            if (i > _liberatedPool.Count) {
                _liberated = _liberatedPool[i];
                _liberated.transform.position = _spawnPoints[i].position;
                _liberated.transform.rotation = _spawnPoints[i].rotation;
                _liberated.Revive();
            } else {
                _liberated = Instantiate(_liberatedPrefab, _spawnPoints[i].position, _spawnPoints[i].rotation);
                _liberatedPool.Add(_liberated);
            }

            _liberated.IsLeader = ActiveLiberated.Count == 0;
            _liberated.SetStopDistance(_liberated.IsLeader ? 0 : 1.5f);
            //Debug.Log($"Set liberated to: {_liberated.IsLeader}. Count {ActiveLiberated.Count}");
            ActiveLiberated.Add(_liberated);
        }
    }

    public void ProcessLiberatedDeath(Liberated _liberated) {
        ActiveLiberated.Remove(_liberated);
        if (ActiveLiberated.Count == 0) {
            return;
        }
        ActiveLiberated[0].IsLeader = true;
        ActiveLiberated[0].SetStopDistance( 0);
    }

    public Vector3 GetFollowingPosition(Liberated _liberated) {
        int index = ActiveLiberated.IndexOf(_liberated);
        if (index > 0) {
            if (ActiveLiberated[index - 1].State == UnitState.Moving) {
                return ActiveLiberated[index - 1].transform.position + new Vector3(Random.Range(-2f, 2f), 0f, Random.Range(-2f, 2f));
            } else {
                return ActiveLiberated[index - 1].transform.position;
            }

        } else {
            return _liberated.transform.position;
        }
    }

    public void RemoveWaypoint(Waypoint _waypoint) {
        ActiveWaypoints.Remove(_waypoint);
        Destroy(_waypoint.gameObject);
    }

    public void AddWaypoint(Waypoint _waypoint) {
        ActiveWaypoints.Add(_waypoint);
    }
}
