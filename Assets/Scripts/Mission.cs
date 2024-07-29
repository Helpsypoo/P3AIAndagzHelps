using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Mission : ScriptableObject {

    public string Name;
    [TextArea(5, 8)]
    public string Description;
    public string SceneName;
    public int Level;

    public bool Available => MissionStatus() == 0;

    public int MissionStatus() {
        switch (Level) {
            case 1:
                return SessionManager.Instance.Level1Status;
            case 2:
                return SessionManager.Instance.Level2Status;
            case 3:
                return SessionManager.Instance.Level3Status;
            case 4:
                return SessionManager.Instance.Level4Status;
            default:
                return SessionManager.Instance.Level0Status;
        }
    }
}
