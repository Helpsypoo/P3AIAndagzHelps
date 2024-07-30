using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.AI.Navigation;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;
    public int TestLevel = -1;
    public float SunDamagePerSecond = 2.5f;
    
    public LayerMask ShadeLayerMask;

    public Camera MainCamera { get; private set; }
    [field: SerializeField] public SelectionCursor SelectionMarker { get; private set; }

    public Level ActiveLevel;
    public Level[] Levels;

    [SerializeField] private Liberated _liberatedPrefab;
    [SerializeField] private Waypoint _waypointPrefab;
    [HideInInspector] public List<Liberated> ActiveLiberated = new List<Liberated>();
    [HideInInspector] public List<Liberated> DeadLiberated = new List<Liberated>();
    public List<Waypoint> ActiveWaypoints = new List<Waypoint>();
    public MoveArrow MoveArrow;

    public Transform KillZone;
    public List<Unit> PlayerUnits { get; private set; } = new List<Unit>();
    [SerializeField] private List<Unit> _playerUnitPrefabs = new List<Unit>();
    public bool IsProcessing { get; private set; }
    private AudioSource _meatGrinder;
    [SerializeField] private Animator _enhancementBuildingAnim;


    [SerializeField] private Transform _blueLiquid;
    [SerializeField] private Transform _orangeLiquid;

    [field: SerializeField] public BulletPool BulletStash { get; private set; }

    [Header("Level Stats")] 
    public int LiberatedProcessed;
    public int LiberatedPercent;
    public int LiberatedTotal;
    public int Kills;
    public int EnemyTotal;
    public float StartTime;
    public int Score;
    public int Survival;
    public int SurvivalTotal;

    private void Awake() {
        if (Instance) {
            Destroy(this);
        } else {
            Instance = this;
        }

        MainCamera = Camera.main;
    }

    private void Start() {
        if (TestLevel != -1) {
            CreateLevel(Levels[TestLevel]);
        } else {
            if (!SessionManager.Instance) {
                SceneManager.LoadScene("Menu");
                return;
            }
            
            CreateLevel(Levels[SessionManager.Instance.Level]);
        }
        
        AudioManager.Instance.PlayAmbiance(AudioManager.Instance.MissionAmbiance, 2f, .4f);
        UpdateGoop();
    }

    public void CreateLevel(Level _level) {
        StartCoroutine(CreateLevelCoroutine(_level));
    }

    public void SetActiveLevel(Level _level) {
        ActiveLevel = _level;
    }

    public int GetLiberatedPct() {
        float _pct = (float)LiberatedProcessed / LiberatedTotal;
        return Mathf.CeilToInt(_pct * 100);
    }

    IEnumerator CreateLevelCoroutine(Level _level) {
        if (ActiveLevel != _level) {
            if(ActiveLevel) { Destroy(ActiveLevel.gameObject); }
            yield return StartCoroutine(InstantiateAsync(_level));
        }
        
        yield return new WaitUntil(() => ActiveLevel);
        
        LiberatedTotal = 0;
        EnemyTotal = 0;

        SpawnOrRelocateLiberated(ActiveLevel.LiberatedSpawnContainer);
        SpawnOrRelocateUnits(ActiveLevel.UnitSpawnsContainer);
        SquadManager.Instance.Init();
        HUD.Instance.Init();
        StartTime = Time.time;
    }


    private void SpawnOrRelocateLiberated(Transform _spawnPointContainer) {
        ActiveLiberated.Clear();
        
        for (int i = 0; i < _spawnPointContainer.childCount; i++) {
            LiberatedSpawn _spawn = _spawnPointContainer.GetChild(i).GetComponent<LiberatedSpawn>();
            if (!_spawn) {
                _spawnPointContainer.GetChild(i).gameObject.SetActive(false);
                continue;
            }
            
            Liberated _liberated;
            _liberated = Instantiate(_liberatedPrefab, _spawnPointContainer.GetChild(i).position, _spawnPointContainer.GetChild(i).rotation);
            _liberated.IsPrisoner = _spawn.IsPrisoner;
            _liberated.gameObject.name = $"Liberated({i})";
            if (!_liberated.IsPrisoner) {
                _liberated.Free();
            } else {
                _liberated.Shackle();
            }
            
            _spawn.gameObject.SetActive(false);
        }
    }
    
    public void JoinLiberated(Liberated _liberated) {
        //TODO: play join sound
        //Debug.Log($"Liberated joined squad");
        _liberated.IsLeader = ActiveLiberated.Count == 0;
        _liberated.SetStopDistance(_liberated.IsLeader ? 0 : 0.8f);
        ActiveLiberated.Add(_liberated);
    }
    
    private void SpawnOrRelocateUnits(Transform _spawnPointContainer) {
        Survival = _spawnPointContainer.childCount;
        SurvivalTotal = Survival;
        for (int i = 0; i < _spawnPointContainer.childCount; i++) {

            if (i >= _playerUnitPrefabs.Count) {
                return;
            }
            
            Unit _unit;
            _unit = Instantiate(_playerUnitPrefabs[i], _spawnPointContainer.GetChild(i).position, _spawnPointContainer.GetChild(i).rotation); 
            PlayerUnits.Add(_unit);
            _unit.ApplyUpgrades(i);
        }
    }

    public void ProcessLiberatedDeath(Liberated _liberated) {
        ActiveLiberated.Remove(_liberated);
        DeadLiberated.Add(_liberated);

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
            return ActiveLiberated[index - 1].transform.position;
        } else {
            return _liberated.transform.position;
        }
    }

    public void RemoveWaypoint(Waypoint _waypoint) {
        ActiveWaypoints.Remove(_waypoint);
        SquadManager.Instance.IncrementWaypoints();
        Destroy(_waypoint.gameObject);
        UpdateLines();
    }

    public void AddWaypoint(Waypoint _waypoint) {
        ActiveWaypoints.Add(_waypoint);
        UpdateLines();
    }

    private void UpdateLines() {

        for (int i = 0; i < ActiveWaypoints.Count; i++) {
            ActiveWaypoints[i].UpdateLine(i);
        }

    }

    public void SetIsProcessing(bool _enabled) {
        if (_enabled == IsProcessing) {
            return;
        }
        if(_meatGrinder) {_meatGrinder.Stop();}
        
        if (_enabled) {
            _meatGrinder = AudioManager.Instance.Play(AudioManager.Instance.MeatGrinder, MixerGroups.SFX, default, 1f, transform.position, .95f, default, true);
        }
        
        IsProcessing = _enabled;

    }

    private float maxFull = 2.5f;

    public void ProcessLiberatedScore(Liberated _liberated) {
        _liberated.gameObject.SetActive(false);
        
        LiberatedProcessed++;
        
        if (SessionManager.Instance) {
            SessionManager.Instance.BlueGoop += 5;
            SessionManager.Instance.OrangeGoop += 1;
            UpdateGoop();
        }
        
        
        AudioManager.Instance.Play(AudioManager.Instance.Liquid, MixerGroups.SFX, new Vector2(.8f, 1.1f), 1f, _enhancementBuildingAnim.transform.position);

        _enhancementBuildingAnim.Play("Process", 0, 0f);
        AudioManager.Instance.Play(AudioManager.Instance.Saw, MixerGroups.SFX, new Vector2(.8f, 1.1f), 1f, _enhancementBuildingAnim.transform.position);
    }

    private void UpdateGoop() {
        if (!SessionManager.Instance) {
            return;
        }
        float _blueLerp =
            Mathf.Lerp(0, maxFull, SessionManager.Instance.BlueGoop / (float)SessionManager.Instance.MaxBlueGoop);
        float _orangeLerp =
            Mathf.Lerp(0, maxFull, SessionManager.Instance.OrangeGoop / (float)SessionManager.Instance.MaxOrangeGoop);
        _blueLiquid.localScale =
            new Vector3(_blueLiquid.localScale.x, _blueLerp, _blueLiquid.localScale.z);
        _orangeLiquid.localScale =
            new Vector3(_orangeLiquid.localScale.x, _orangeLerp, _orangeLiquid.localScale.z);
    }
}