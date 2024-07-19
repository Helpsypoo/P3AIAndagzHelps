using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TickEntity : MonoBehaviour {
    [SerializeField] private UnityEvent updateEvent;

    public void InvokeUpdateEvent() {
        //Debug.Log($"Updating tick event for {gameObject.name}");
        updateEvent.Invoke();
    }

    public void AddToTickEventManager() {
        if (TickEventManager.Instance != null) {
            TickEventManager.Instance.AddTickEntity(this);
        }
    }

    public void RemoveFromTickEventManager() {
        if (TickEventManager.Instance != null) {
            TickEventManager.Instance.RemoveTickEntity(this);
        }
    }
}
