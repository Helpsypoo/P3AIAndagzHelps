using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour {

    public static HUD Instance { get; private set; }

    private void Awake() {
        if (Instance) {
            Destroy(this);
        } else {
            Instance = this;
        }
    }

    [SerializeField] private UI_Character[] _squad;

    private void Start() {

        for (int i = 0; i < _squad.Length; i++) {
            _squad[i].Init(SquadManager.Instance.Units[i]);
        }
    }

    private void Update() {
        UpdateSquad();
    }

    public void UpdateSquad() {

        for (int i = 0; i < _squad.Length; i++) {

            if (i == SquadManager.Instance.UnitIndex) {
                _squad[i].Select();
            } else {
                _squad[i].Deselect();
            }
            _squad[i].UpdateCharacter();
        }

    }


}
