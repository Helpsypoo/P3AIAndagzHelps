using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpManager : MonoBehaviour {
    public LevelUp[] LevelUps;

    public void UpdateLevelUps() {
        for (int i = 0; i < LevelUps.Length; i++) {
            if (i < GameManager.Instance.PlayerUnits.Count) {
                LevelUps[i].UpdateDisplay(GameManager.Instance.PlayerUnits[i], this);
                LevelUps[i].gameObject.SetActive(true);
            }
        }
        
        MissionStateManager.Instance.UpdateGoop();
    }
}
