using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour {

    public static HUD Instance { get; private set; }
    private TickEntity _tickEntity;

    private void Awake() {
        if (Instance) {
            Destroy(this);
        } else {
            Instance = this;
        }

        _tickEntity = GetComponent<TickEntity>();
    }

    [SerializeField] private UI_Character[] _squad;
    [field: SerializeField] public HUDMessage MessageWindow;

    public void Init() {
        for (int i = 0; i < _squad.Length; i++) {
            if (i < GameManager.Instance.PlayerUnits.Count) {
                _squad[i].Init(GameManager.Instance.PlayerUnits[i]);
            } else {
                _squad[i].gameObject.SetActive(false);
            }

        }
        
        TickEventManager.Instance.AddTickEntity(_tickEntity);
    }

    public void UpdateSquad() {
        for (int i = 0; i < _squad.Length; i++) {
            if(!_squad[i].gameObject.activeSelf){ continue; }
            if (i == SquadManager.Instance.UnitIndex) {
                _squad[i].Select();
                _squad[i].UpdateActionVisual(SquadManager.Instance.ActionMode);
            } else {
                _squad[i].Deselect();
                _squad[i].UpdateActionVisual(false);
            }
            _squad[i].UpdateCharacter();

        }

    }

    private void OnDisable() {
        TickEventManager.Instance.RemoveTickEntity(_tickEntity);
    }
}
