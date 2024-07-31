using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimListener2 : MonoBehaviour {

    [SerializeField] private UnityEvent _event;

    public void Trigger() {

        _event.Invoke();

    }

}
