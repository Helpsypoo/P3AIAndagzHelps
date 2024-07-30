using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class UnitUpgrade : ScriptableObject {
    public int Cost;
    public int Health;
    public int Speed;
    public int Damage;
    public int Ability;
    public string Description;
}
