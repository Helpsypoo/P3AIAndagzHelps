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

    [SerializeField] private TextMeshProUGUI _squadList;

    private void Update() {
        UpdateSquad();
    }

    public void UpdateSquad() {

        string text = "";

        foreach (Unit unit in SquadManager.Instance.Units) {
            if (unit == SquadManager.Instance.SelectedUnit) {
                text += $"<color=black>{unit.UnitStats.Name}</color>";
            } else {
                text += unit.UnitStats.Name;
            }

            if (unit.FollowTarget != null) {
                text += $"(following {unit.FollowTarget.UnitStats.Name})";
            }


            text += "\n";
        }
        _squadList.text = text;
    }


}
