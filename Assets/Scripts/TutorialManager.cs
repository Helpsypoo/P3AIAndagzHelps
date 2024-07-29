using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialManager : MonoBehaviour {

    [SerializeField] private TutorialStage[] _stages;
    private int index = 0;

    private void Start() {
        SquadManager.Instance.DisablePlayerInput = true;
    }

    private void Update() {
        if (index == 0 && !TransitionManager.Instance.Transitioning) {
            ShowNextMessage();
        }
    }

    public void CompleteCurrentTask() {
        HUD.Instance.MessageWindow.Deactivate();
        SquadManager.Instance.DisablePlayerInput = false;
    }

    public void ShowNextMessage() {

        if (index >= _stages.Length) {
            return;
        }
        HUD.Instance.MessageWindow.ShowMessage(_stages[index].Title, _stages[index].Message, _stages[index].ShowCloseButton);
        index++;
    }

}

[System.Serializable]
public class TutorialStage {

    public string Title;
    public string Message;
    public bool ShowCloseButton;

}
