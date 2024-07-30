using UnityEngine;
using System.Collections.Generic;

public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance;
    public int Level;

    // Define your PlayerPrefs keys here for easy management
    private static List<string> allKeys = new List<string>
    {
        "BlueGoop", "OrangeGoop",
        "ShepardKills", "ShepardLevel",
        "ChonkKills", "ChonkLevel",
        "PercivalKills", "PercivalLevel",
        "NovaKills", "NovaLevel",
        "Level0Status", "Level1Status", "Level2Status", "Level3Status", "Level4Status",
        "Level0Highscore", "Level1Highscore", "Level2Highscore", "Level3Highscore", "Level4Highscore"
    };

    // List of high score keys to preserve
    private static List<string> highScoreKeys = new List<string>
    {
        "Level0Highscore", "Level1Highscore", "Level2Highscore", "Level3Highscore", "Level4Highscore"
    };

    public int BlueGoop;
    public int MaxBlueGoop = 1000;
    public int OrangeGoop;
    public int MaxOrangeGoop = 1000;

    public int ShepardKills;
    public int ShepardLevel;

    public int ChonkKills;
    public int ChonkLevel;

    public int PercivalKills;
    public int PercivalLevel;

    public int NovaKills;
    public int NovaLevel;

    // Status || 0 = incomplete | 1 = complete | 2 = failed
    public int Level0Status;
    public int Level1Status;
    public int Level2Status;
    public int Level3Status;
    public int Level4Status;

    public int Level0Highscore;
    public int Level1Highscore;
    public int Level2Highscore;
    public int Level3Highscore;
    public int Level4Highscore;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        Load();
    }

    public void NewGame() {
        if (Level0Status > 0) {
            Menu _menu = FindObjectOfType<Menu>();
            if(_menu){_menu.WarningMessage.SetActive(true);}
            return;
        }
        TransitionManager.Instance.TransitionToScene("Game");
    }

    public void ResumeGame() {
        TransitionManager.Instance.TransitionToScene("MissionSelect");
    }

    public void Save() {
        PlayerPrefs.SetInt("BlueGoop", BlueGoop);
        PlayerPrefs.SetInt("OrangeGoop", OrangeGoop);

        PlayerPrefs.SetInt("ShepardKills", ShepardKills);
        PlayerPrefs.SetInt("ShepardLevel", ShepardLevel);

        PlayerPrefs.SetInt("ChonkKills", ChonkKills);
        PlayerPrefs.SetInt("ChonkLevel", ChonkLevel);

        PlayerPrefs.SetInt("PercivalKills", PercivalKills);
        PlayerPrefs.SetInt("PercivalLevel", PercivalLevel);

        PlayerPrefs.SetInt("NovaKills", NovaKills);
        PlayerPrefs.SetInt("NovaLevel", NovaLevel);

        PlayerPrefs.SetInt("Level0Status", Level0Status);
        PlayerPrefs.SetInt("Level1Status", Level1Status);
        PlayerPrefs.SetInt("Level2Status", Level2Status);
        PlayerPrefs.SetInt("Level3Status", Level3Status);
        PlayerPrefs.SetInt("Level4Status", Level4Status);

        PlayerPrefs.SetInt("Level0Highscore", Level0Highscore);
        PlayerPrefs.SetInt("Level1Highscore", Level1Highscore);
        PlayerPrefs.SetInt("Level2Highscore", Level2Highscore);
        PlayerPrefs.SetInt("Level3Highscore", Level3Highscore);
        PlayerPrefs.SetInt("Level4Highscore", Level4Highscore);

        PlayerPrefs.Save();
    }

    public void Load() {
        BlueGoop = PlayerPrefs.GetInt("BlueGoop", 0);
        OrangeGoop = PlayerPrefs.GetInt("OrangeGoop", 0);

        ShepardKills = PlayerPrefs.GetInt("ShepardKills", 0);
        ShepardLevel = PlayerPrefs.GetInt("ShepardLevel", 0);

        ChonkKills = PlayerPrefs.GetInt("ChonkKills", 0);
        ChonkLevel = PlayerPrefs.GetInt("ChonkLevel", 0);

        PercivalKills = PlayerPrefs.GetInt("PercivalKills", 0);
        PercivalLevel = PlayerPrefs.GetInt("PercivalLevel", 0);

        NovaKills = PlayerPrefs.GetInt("NovaKills", 0);
        PercivalLevel = PlayerPrefs.GetInt("NovaLevel", 0);

        Level0Status = PlayerPrefs.GetInt("Level0Status", (int)MissionCondition.Available);
        Level1Status = PlayerPrefs.GetInt("Level1Status", (int)MissionCondition.Locked);
        Level2Status = PlayerPrefs.GetInt("Level2Status", (int)MissionCondition.Locked);
        Level3Status = PlayerPrefs.GetInt("Level3Status", (int)MissionCondition.Locked);
        Level4Status = PlayerPrefs.GetInt("Level4Status", (int)MissionCondition.Locked);

        Level0Highscore = PlayerPrefs.GetInt("Level0Highscore", 0);
        Level1Highscore = PlayerPrefs.GetInt("Level1Highscore", 0);
        Level2Highscore = PlayerPrefs.GetInt("Level2Highscore", 0);
        Level3Highscore = PlayerPrefs.GetInt("Level3Highscore", 0);
        Level4Highscore = PlayerPrefs.GetInt("Level4Highscore", 0);
    }

    public int GetLevel(Unit _unit) {
        string _name = _unit.UnitStats.Name;
        switch (_name) {
            case "Shepard":
                return ShepardLevel;
            case "Chonk":
                return ChonkLevel;
            case "Percival":
                return PercivalLevel;
            case "Nova":
                return NovaLevel;
            default:
                return -1;
        }
    }

    public void AddLevel(Unit _unit) {
        string _name = _unit.UnitStats.Name;
        switch (_name) {
            case "Shepard":
                ShepardLevel++;
                break;
            case "Chonk":
                ChonkLevel++;
                break;
            case "Percival":
                PercivalLevel++;
                break;
            case "Nova":
                NovaLevel++;
                break;
        }
    }

    public void DeleteSaveData() {
        foreach (string key in allKeys) {
            if (!highScoreKeys.Contains(key)) {
                PlayerPrefs.DeleteKey(key);
            }
        }

        PlayerPrefs.Save();
        Load();
        NewGame();
    }
}
