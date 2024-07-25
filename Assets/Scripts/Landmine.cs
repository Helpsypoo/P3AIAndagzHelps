using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landmine : MonoBehaviour {

    [SerializeField] private GameObject _lightOff;
    [SerializeField] private GameObject _lightOn;

    float timer;
    private void Update() {

        timer += Time.deltaTime;
        if (timer > 0.5f) {
            _lightOff.SetActive(!_lightOff.activeSelf);
            _lightOn.SetActive(!_lightOn.activeSelf);
            timer = 0f;
        }

    }

}
