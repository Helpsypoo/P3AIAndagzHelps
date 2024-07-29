using System.Collections;
using Cinemachine;
using TMPro;
using UnityEngine;

public class MissionStateManager : MonoBehaviour {
    public static MissionStateManager Instance;
    private Canvas _canvas;
    private Animator _anim;
    private MissionCondition _missionCondition;
    
    [Header("Complete Mission")]
    [SerializeField] private RectTransform _completePanel;
    [SerializeField] private Transform[] _missionCompleteUnitLocation;
    [SerializeField] private CinemachineVirtualCamera _missionCompleteCam;
    [SerializeField] private TextMeshProUGUI _liberatedTitle;
    [SerializeField] private TextMeshProUGUI _liberatedCount;
    [SerializeField] private TextMeshProUGUI _killsTitle;
    [SerializeField] private TextMeshProUGUI _killsCount;    
    [SerializeField] private TextMeshProUGUI _timeTitle;
    [SerializeField] private TextMeshProUGUI _timeCount;    
    [SerializeField] private TextMeshProUGUI _survivalTitle;
    [SerializeField] private TextMeshProUGUI _survivalCount;
    [SerializeField] private TextMeshProUGUI _score;
    [SerializeField] private TextMeshProUGUI _highscore;
    private int _scoreFinal;
    private float _endTime;
    
    [Header("Fail Mission")]
    [SerializeField] private RectTransform _failPanel;
    [SerializeField] private TextMeshProUGUI _failMessageText;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }

        _canvas = GetComponent<Canvas>();
        _anim = GetComponent<Animator>();
    }

    public void MissionEvent(MissionCondition _condition) {
        _missionCondition = _condition;
        GameManager.Instance.SetIsProcessing(false);
        UpdateLevelInformation(SessionManager.Instance.Level, _condition);
        switch (_missionCondition) {
            case MissionCondition.Complete:
                Complete();
                break;
            case MissionCondition.FailMininumLiberated:
                Fail("<size=8.2>failed to liberate enough citizens");
                break;
            case MissionCondition.FailUnitsLost:
                Fail("all units were lost in battle");
                break;
        }
        
    }
    
    public void Complete() {
        _endTime = Time.time;
        for (int i = 0; i < GameManager.Instance.PlayerUnits.Count; i++) {
            GameManager.Instance.PlayerUnits[i].Revive(true);
            GameManager.Instance.PlayerUnits[i].SetState(UnitState.Locked);
            if(i < _missionCompleteUnitLocation.Length){GameManager.Instance.PlayerUnits[i].SetTransform(_missionCompleteUnitLocation[i]);}
        }
        
        _missionCompleteCam.Priority = 10;
        
        _canvas.enabled = true;
        AudioManager.Instance.Play(AudioManager.Instance.CompletedJingle, MixerGroups.SFX);
        _highscore.gameObject.SetActive(false);
        _anim.Play("Complete");
        _completePanel.gameObject.SetActive(true);
        CalculateScore();
    }

    public void Fail(string _failMessage) {
        _canvas.enabled = true;
        AudioManager.Instance.Play(AudioManager.Instance.FailJingle, MixerGroups.SFX);
        _anim.Play("Fail");
        _failMessageText.text = _failMessage;
        _failPanel.gameObject.SetActive(true);
    }

    public void SubtotalSound() {
        AudioManager.Instance.Play(AudioManager.Instance.ScoreSubtotal, MixerGroups.UI);
    }

    private void CalculateScore() {
        int _liberated = CalcLiberated();
        int _kills = CalcKills();
        int _time = CalcTime();
        int _surivival = CalcSurvival();
        _scoreFinal = _liberated + _kills + _time + _surivival;
    }

    public void AnimateScore() {
        StartCoroutine(AnimateScoreCoroutine());
    }
    
    private IEnumerator AnimateScoreCoroutine() {
        float _clipDuration = AudioManager.Instance.ScoreCalc.length;
        float elapsedTime = 0f;
        int startScore = 0;

        while (elapsedTime < _clipDuration) {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / _clipDuration);
            int currentScore = (int)Mathf.Lerp(startScore, _scoreFinal, t);
            _score.text = currentScore.ToString();
            yield return null;
        }
        
        _score.text = _scoreFinal.ToString();
        //TODO: check if highscore
    }
    
    #region Caluations
    private int CalcLiberated() {
        int _pctInt = GameManager.Instance.GetLiberatedPct();
        _liberatedTitle.text = $"Liberated  <size=14>({_pctInt}%)";
        int _liberatedScore = Mathf.CeilToInt(Mathf.Lerp(0, 1000, _pctInt / 100f));
        _liberatedCount.text = $"{_liberatedScore}";
        return _liberatedScore;
    }
    
    private int CalcKills() {
        if (GameManager.Instance.EnemyTotal == 0) {
            _killsTitle.text = $"Kills  <size=14>({GameManager.Instance.Kills})";
            _killsCount.text = $"{1000}";
            return 1000;
        }
        float _pct = (float)GameManager.Instance.Kills / GameManager.Instance.EnemyTotal;
        _killsTitle.text = $"Kills  <size=14>({GameManager.Instance.Kills})";
        int _killScore = Mathf.CeilToInt(Mathf.Lerp(0, 1000, _pct));
        _killsCount.text = $"{_killScore}";
        return _killScore;
    }

    
    private int CalcTime() {
        float _timeDelta = _endTime - GameManager.Instance.StartTime;
        int minutes = (int)(_timeDelta / 60);
        int seconds = (int)(_timeDelta % 60);
        
        string _formattedTime = string.Format("{0}:{1:D2}", minutes, seconds);
        _timeTitle.text = $"Time  <size=14>({_formattedTime})";
        int _timeScore = Mathf.CeilToInt(Mathf.Lerp(0, 1000, _timeDelta / (5 * 60)));
        _timeCount.text = $"{_timeScore}";
        return _timeScore;
    }
    
    private int CalcSurvival() {
        float _pct = (float)GameManager.Instance.Survival / GameManager.Instance.SurvivalTotal;
        _survivalTitle.text = $"Survival  <size=14>({Mathf.CeilToInt(_pct*100)}%)";
        int _survivalScore = Mathf.CeilToInt(Mathf.Lerp(0, 1000, _pct));
        _survivalCount.text = $"{_survivalScore}";
        return _survivalScore;
    }

    #endregion

    public void GoToMissionSelection() {
        if (SessionManager.Instance.Level == 0 && 
            (_missionCondition == MissionCondition.FailMininumLiberated || _missionCondition == MissionCondition.FailUnitsLost)) {
            AudioManager.Instance.ClearGameSounds();
            AudioManager.Instance.PlayAmbiance(AudioManager.Instance.MenuAmbiance, 2f, .4f);
            TransitionManager.Instance.TransitionToScene("Game");
            return;
        }

        UpdateLevelInformation(SessionManager.Instance.Level, _missionCondition);
        
        SessionManager.Instance.Save();
        AudioManager.Instance.ClearGameSounds();
        AudioManager.Instance.PlayAmbiance(AudioManager.Instance.MenuAmbiance, 2f, .4f);
        TransitionManager.Instance.TransitionToScene("MissionSelect");
    }

    private void UpdateLevelInformation(int _level, MissionCondition _status) {
        int _statusToInt = ConvertMissionConditionToInt(_status);
        switch (_level) {
            case 0:
                SessionManager.Instance.Level0Status = _statusToInt;
                break;
            case 1:
                SessionManager.Instance.Level1Status = _statusToInt;
                break;
            case 2:
                SessionManager.Instance.Level2Status = _statusToInt;
                break;
            case 3:
                SessionManager.Instance.Level3Status = _statusToInt;
                break;
            case 4:
                SessionManager.Instance.Level4Status = _statusToInt;
                break;
            
        }
    }

    // Status || 0 = incomplete | 1 = complete | 2 = failed
    private int ConvertMissionConditionToInt(MissionCondition _status) {
        switch (_status) {
            default:
                return 0;
            case MissionCondition.Complete:
                return 1;
            case MissionCondition.FailMininumLiberated:
            case MissionCondition.FailUnitsLost:
                return 2;
        }
    }
}