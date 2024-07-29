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
        "ShepardKills", "ShepardAbilityModifier1", "ShepardAbilityModifier2", "ShepardAbilityModifier3", "ShepardAbilityModifier4",
        "ChonkKills", "ChonkAbilityModifier1", "ChonkAbilityModifier2", "ChonkAbilityModifier3", "ChonkAbilityModifier4",
        "PercivalKills", "PercivalAbilityModifier1", "PercivalAbilityModifier2", "PercivalAbilityModifier3", "PercivalAbilityModifier4",
        "NovaKills", "NovaAbilityModifier1", "NovaAbilityModifier2", "NovaAbilityModifier3", "NovaAbilityModifier4",
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
    public int ShepardAbilityModifier1;
    public int ShepardAbilityModifier2;
    public int ShepardAbilityModifier3;
    public int ShepardAbilityModifier4;

    public int ChonkKills;
    public int ChonkAbilityModifier1;
    public int ChonkAbilityModifier2;
    public int ChonkAbilityModifier3;
    public int ChonkAbilityModifier4;
    
    public int PercivalKills;
    public int PercivalAbilityModifier1;
    public int PercivalAbilityModifier2;
    public int PercivalAbilityModifier3;
    public int PercivalAbilityModifier4;

    public int NovaKills;
    public int NovaAbilityModifier1;
    public int NovaAbilityModifier2;
    public int NovaAbilityModifier3;
    public int NovaAbilityModifier4;
    
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
        PlayerPrefs.SetInt("ShepardAbilityModifier1", ShepardAbilityModifier1);
        PlayerPrefs.SetInt("ShepardAbilityModifier2", ShepardAbilityModifier2);
        PlayerPrefs.SetInt("ShepardAbilityModifier3", ShepardAbilityModifier3);
        PlayerPrefs.SetInt("ShepardAbilityModifier4", ShepardAbilityModifier4);

        PlayerPrefs.SetInt("ChonkKills", ChonkKills);
        PlayerPrefs.SetInt("ChonkAbilityModifier1", ChonkAbilityModifier1);
        PlayerPrefs.SetInt("ChonkAbilityModifier2", ChonkAbilityModifier2);
        PlayerPrefs.SetInt("ChonkAbilityModifier3", ChonkAbilityModifier3);
        PlayerPrefs.SetInt("ChonkAbilityModifier4", ChonkAbilityModifier4);

        PlayerPrefs.SetInt("PercivalKills", PercivalKills);
        PlayerPrefs.SetInt("PercivalAbilityModifier1", PercivalAbilityModifier1);
        PlayerPrefs.SetInt("PercivalAbilityModifier2", PercivalAbilityModifier2);
        PlayerPrefs.SetInt("PercivalAbilityModifier3", PercivalAbilityModifier3);
        PlayerPrefs.SetInt("PercivalAbilityModifier4", PercivalAbilityModifier4);

        PlayerPrefs.SetInt("NovaKills", NovaKills);
        PlayerPrefs.SetInt("NovaAbilityModifier1", NovaAbilityModifier1);
        PlayerPrefs.SetInt("NovaAbilityModifier2", NovaAbilityModifier2);
        PlayerPrefs.SetInt("NovaAbilityModifier3", NovaAbilityModifier3);
        PlayerPrefs.SetInt("NovaAbilityModifier4", NovaAbilityModifier4);

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
        ShepardAbilityModifier1 = PlayerPrefs.GetInt("ShepardAbilityModifier1", 0);
        ShepardAbilityModifier2 = PlayerPrefs.GetInt("ShepardAbilityModifier2", 0);
        ShepardAbilityModifier3 = PlayerPrefs.GetInt("ShepardAbilityModifier3", 0);
        ShepardAbilityModifier4 = PlayerPrefs.GetInt("ShepardAbilityModifier4", 0);

        ChonkKills = PlayerPrefs.GetInt("ChonkKills", 0);
        ChonkAbilityModifier1 = PlayerPrefs.GetInt("ChonkAbilityModifier1", 0);
        ChonkAbilityModifier2 = PlayerPrefs.GetInt("ChonkAbilityModifier2", 0);
        ChonkAbilityModifier3 = PlayerPrefs.GetInt("ChonkAbilityModifier3", 0);
        ChonkAbilityModifier4 = PlayerPrefs.GetInt("ChonkAbilityModifier4", 0);

        PercivalKills = PlayerPrefs.GetInt("PercivalKills", 0);
        PercivalAbilityModifier1 = PlayerPrefs.GetInt("PercivalAbilityModifier1", 0);
        PercivalAbilityModifier2 = PlayerPrefs.GetInt("PercivalAbilityModifier2", 0);
        PercivalAbilityModifier3 = PlayerPrefs.GetInt("PercivalAbilityModifier3", 0);
        PercivalAbilityModifier4 = PlayerPrefs.GetInt("PercivalAbilityModifier4", 0);

        NovaKills = PlayerPrefs.GetInt("NovaKills", 0);
        NovaAbilityModifier1 = PlayerPrefs.GetInt("NovaAbilityModifier1", 0);
        NovaAbilityModifier2 = PlayerPrefs.GetInt("NovaAbilityModifier2", 0);
        NovaAbilityModifier3 = PlayerPrefs.GetInt("NovaAbilityModifier3", 0);
        NovaAbilityModifier4 = PlayerPrefs.GetInt("NovaAbilityModifier4", 0);

        Level0Status = PlayerPrefs.GetInt("Level0Status", 0);
        Level1Status = PlayerPrefs.GetInt("Level1Status", 0);
        Level2Status = PlayerPrefs.GetInt("Level2Status", 0);
        Level3Status = PlayerPrefs.GetInt("Level3Status", 0);
        Level4Status = PlayerPrefs.GetInt("Level4Status", 0);

        Level0Highscore = PlayerPrefs.GetInt("Level0Highscore", 0);
        Level1Highscore = PlayerPrefs.GetInt("Level1Highscore", 0);
        Level2Highscore = PlayerPrefs.GetInt("Level2Highscore", 0);
        Level3Highscore = PlayerPrefs.GetInt("Level3Highscore", 0);
        Level4Highscore = PlayerPrefs.GetInt("Level4Highscore", 0);
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
