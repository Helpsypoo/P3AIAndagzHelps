using System;
using System.Collections.Generic;
using UnityEngine;

public class TickEventManager : MonoBehaviour {
    public static TickEventManager Instance;
    private List<TickEntity> tickEntities = new List<TickEntity>();
    private int currentEntityIndex;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
        }
    }

    private void Update() {
        if (tickEntities.Count == 0) return;
        
       tickEntities[currentEntityIndex].InvokeUpdateEvent();
       currentEntityIndex++;
        
        if (currentEntityIndex >= tickEntities.Count) {
            currentEntityIndex = 0;
        }
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
