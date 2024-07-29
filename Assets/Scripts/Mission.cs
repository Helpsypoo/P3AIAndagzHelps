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

}
