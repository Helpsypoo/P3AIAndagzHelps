using System;
using System.Collections.Generic;
using UnityEngine;

public class TickEventManager : MonoBehaviour {
    public static TickEventManager Instance;
    private List<TickEntity> tickEntities = new List<TickEntity>();
    private int currentEntityIndex;

    [SerializeField] private int _amountPerUpdate = 1;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
        }
    }

    private void Update() {
        if (tickEntities.Count == 0) {return;}

        //Update all if the amount is less or equal to our list
        if (_amountPerUpdate >= tickEntities.Count) {
            for (int i = 0; i < tickEntities.Count; i++) {
                tickEntities[i].InvokeUpdateEvent();
            }
            return;
        }

        //Debug.Log($"Current Index {currentEntityIndex}");
        // Loop through _amountPerUpdate entities starting at currentEntityIndex and wrapping around the tickEntities list
        for (int i = currentEntityIndex; i < currentEntityIndex + _amountPerUpdate; i++) {
            int index = i % tickEntities.Count;
            tickEntities[index].InvokeUpdateEvent();
        }

        // Update currentEntityIndex for the next frame

        currentEntityIndex = (currentEntityIndex + _amountPerUpdate) % tickEntities.Count;
    }

    public void AddTickEntity(TickEntity _tickEntity) {
        if (!_tickEntity) return; 
        if (tickEntities.Contains(_tickEntity)) return;
        tickEntities.Add(_tickEntity);
    }
    
    public void RemoveTickEntity(TickEntity _tickEntity) {
        if (tickEntities.Contains(_tickEntity)) {
            tickEntities.Remove(_tickEntity);
        }
        if (tickEntities.Count > 0) {
            currentEntityIndex %= tickEntities.Count;
        } else {
            currentEntityIndex = 0;
        }
    }
}
