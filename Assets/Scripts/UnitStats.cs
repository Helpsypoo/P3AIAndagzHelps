using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class UnitStats : ScriptableObject {
	public string Name;
	public Color Colour;
	public float MaxHealth;
	public float HealthRegenRate;
	public float HealthRegenDelay;
	public float Speed;
	public int AttackDamage;
	public float AttackRate;
	public float AttackRange;
	public bool IsEnemy;
	public bool InvunerableToSun;
	public float ActionRange; // How close unit needs to be to target to perform their skill action thingie.
	public AudioClip[] Footsteps;
}
