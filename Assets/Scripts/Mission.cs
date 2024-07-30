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

    public MissionCondition Condition
    {
        get
        {
            switch (Level) {
                case 1:
                    return (MissionCondition)SessionManager.Instance.Level1Status;
                case 2:
                    return (MissionCondition)SessionManager.Instance.Level2Status;
                case 3:
                    return (MissionCondition)SessionManager.Instance.Level3Status;
                case 4:
                    return (MissionCondition)SessionManager.Instance.Level4Status;
                default:
                    return (MissionCondition)SessionManager.Instance.Level0Status;
            }
        }
    }
}
