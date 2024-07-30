using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelUpButton : MonoBehaviour {
    [SerializeField] private LevelUp _levelUp;

    private EventTrigger _eventTrigger;

    private void Awake() {
        _eventTrigger = gameObject.AddComponent<EventTrigger>(); // Add EventTrigger component
        
        // Set up the pointer enter event
        EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        pointerEnterEntry.callback.AddListener((data) => { _levelUp.ShowUpgrade(); });
        _eventTrigger.triggers.Add(pointerEnterEntry);

        // Set up the pointer exit event
        EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerExit
        };
        pointerExitEntry.callback.AddListener((data) => { _levelUp.HideUpgrade(); });
        _eventTrigger.triggers.Add(pointerExitEntry);
    }
}
