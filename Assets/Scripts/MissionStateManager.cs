using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MissionStateManager : MonoBehaviour {
    public static MissionStateManager Instance;
    private Canvas _canvas;
    private Animator _anim;
    
    [Header("Complete Mission")]
    [SerializeField] private RectTransform _completePanel;
    
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
        GameManager.Instance.SetIsProcessing(false);
        switch (_condition) {
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
        _canvas.enabled = true;
        AudioManager.Instance.Play(AudioManager.Instance.CompletedJingle, MixerGroups.SFX);
        _anim.Play("Complete");
        _completePanel.gameObject.SetActive(true);
        //move units to set base locations
        
    }

    public void Fail(string _failMessage) {
        _canvas.enabled = true;
        AudioManager.Instance.Play(AudioManager.Instance.FailJingle, MixerGroups.SFX);
        _anim.Play("Fail");
        _failMessageText.text = _failMessage;
        _failPanel.gameObject.SetActive(true);
    }

    public void GoToMissionSelection() {
        TransitionManager.Instance.TransitionToScene("MissionSelect");
    }
}