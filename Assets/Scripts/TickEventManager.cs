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
        if (tickEntities.Count == 0) return;

        // Calculate the end index of the loop
        int endIndex = currentEntityIndex + _amountPerUpdate;
        if (endIndex > tickEntities.Count) {
            endIndex -= tickEntities.Count;
        }

        // Loop through entities and invoke update event
        for (int i = currentEntityIndex; i < endIndex; i++) {
            int index = i % tickEntities.Count;
            tickEntities[index].InvokeUpdateEvent();
        }

        // Update currentEntityIndex for the next frame
        currentEntityIndex = endIndex % tickEntities.Count;
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
