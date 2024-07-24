using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {
    public Transform LiberatedSpawnContainer;
    public Transform[] UnitSpawns;
    public float DaylightIntensity;

    IEnumerator Start() {
        yield return new WaitUntil(() => GameManager.Instance);
        GameManager.Instance.SetActiveLevel(this);
    }
}
