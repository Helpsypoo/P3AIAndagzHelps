using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class UnitUpgrade : ScriptableObject {
    public int Cost;
    [FormerlySerializedAs("Health")] public int MaxHealth;
    public float Speed;
    public int Damage;
    public int Ability;
    public string Description;
}
