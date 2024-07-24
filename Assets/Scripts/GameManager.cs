using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.AI.Navigation;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

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
    [HideInInspector] public List<Liberated> DeadLiberated = new List<Liberated>();
    public List<Waypoint> ActiveWaypoints = new List<Waypoint>();
    public int LiberatedScore { get; private set; }
    public Transform KillZone;
    public List<Unit> PlayerUnits { get; private set; } = new List<Unit>();
    public bool IsProcessing { get; private set; }
    [SerializeField] private Animator _enhancementBuildingAnim;

    [field: SerializeField] public BulletPool BulletStash { get; private set; }

    private void Awake() {
        if (AudioManager.Instance == null) {
            SceneManager.LoadScene("Menu");
        }
        
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

        SpawnOrRelocatedLiberated(ActiveLevel.LiberatedSpawnContainer);
        //TODO: spawn/move units to ActiveLevel.UnitSpawns
        
        yield return new WaitForSeconds(2f);
        //TODO: transition out of a loading screen
    }


    private void SpawnOrRelocatedLiberated(Transform _spawnPointContainer) {
        ActiveLiberated.Clear();
        
        for (int i = 0; i < _spawnPointContainer.childCount; i++) {
            Liberated _liberated;
            if (i > _liberatedPool.Count) {
                _liberated = _liberatedPool[i];
                _liberated.transform.position = _spawnPointContainer.GetChild(i).position;
                _liberated.transform.rotation = _spawnPointContainer.GetChild(i).rotation;
                _liberated.Revive();
            } else {
                _liberated = Instantiate(_liberatedPrefab, _spawnPointContainer.GetChild(i).position, _spawnPointContainer.GetChild(i).rotation);
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
        DeadLiberated.Add(_liberated);
        
        if (IsProcessing) {
            _liberated.gameObject.SetActive(false);
        }

        if (ActiveLiberated.Count == 0) {
            if (IsProcessing) {
                MissionStateManager.Instance.MissionEvent(MissionCondition.Complete);
            } else {
                MissionStateManager.Instance.MissionEvent(MissionCondition.FailMininumLiberated);
            }
            return;
        }
        ActiveLiberated[0].IsLeader = true;
        ActiveLiberated[0].SetStopDistance( 0);
    }
    
    public void ProcessUnitLife(Unit _unit) {
        if (!PlayerUnits.Contains(_unit)) {
            PlayerUnits.Add(_unit);
        }

        int _deadUnits = 0;
        for (int i = 0; i < PlayerUnits.Count; i++) {
            if (PlayerUnits[i].Health <= 0) {
                _deadUnits++;
            }
        }
        
        if (_deadUnits >= PlayerUnits.Count) {
            MissionStateManager.Instance.MissionEvent(MissionCondition.FailUnitsLost);
        }
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
        SquadManager.Instance.IncrementWaypoints();
        Destroy(_waypoint.gameObject);
    }

    public void AddWaypoint(Waypoint _waypoint) {
        ActiveWaypoints.Add(_waypoint);
    }

    public void SetIsProcessing(bool _enabled) {
        if (_enabled == IsProcessing) {
            return;
        }

        IsProcessing = _enabled;

    }

    public void ProcessLiberatedScore() {
        LiberatedScore++;
        _enhancementBuildingAnim.Play("Process", 0, 0f);
    }
}
