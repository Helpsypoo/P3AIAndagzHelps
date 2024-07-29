using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {
    public Transform LiberatedSpawnContainer;
    public Transform UnitSpawnsContainer;
    public float DaylightIntensity;
    public int MinimumLiberatedPct;
    [field: SerializeField] public TutorialManager Tutorial { get; private set; }

    IEnumerator Start() {
        yield return new WaitUntil(() => GameManager.Instance);
        GameManager.Instance.SetActiveLevel(this);
    }
}
