using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimListener : MonoBehaviour {

    private Unit unit;

    private void Awake() {
        unit = GetComponentInParent<Unit>();
    }

    public void Fire() {
        unit?.Fire();
    }
}
