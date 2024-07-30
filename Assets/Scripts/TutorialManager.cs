using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialManager : MonoBehaviour {

    [SerializeField] private TutorialStage[] _stages;
    public TutorialStage CurrentStage => _stages[index];
    private int index = -1;

    // Quick bool to 
    private bool IsActive => HUD.Instance.MessageWindow.gameObject.activeSelf;

    private void Start() {

        if (_stages.Length < 1) {
            Destroy(this);
            return;
        }
        SquadManager.Instance.DisablePlayerInput = true;
    }

    private void Update() {

        // If we're set to -1 (meaning we've just loaded in) and the transition has finished, run the first mission.
        if (index == -1 && !TransitionManager.Instance.Transitioning) {
            ShowNextMessage();
            return;
        }

        // If we are currently in a tutorial stage and that tutorial stage is not ended by clicking the button,
        // check to see if whatever associated object has been removed.
        if (index > -1 && index < _stages.Length && !CurrentStage.ShowCloseButton) {

            if (CurrentStage.Target == null) {
                ShowNextMessage();
            }

        }

    }

    public void CompleteCurrentTask() {

        if (index <= _stages.Length && CurrentStage.ShowNextMessage) {
            ShowNextMessage();
        } else {
            HUD.Instance.MessageWindow.Deactivate();
            SquadManager.Instance.DisablePlayerInput = false;
        }
    }

    public void ShowNextMessage() {

        index++;

        // If we've done all the tutorials, destroy the tutorial object, we don't need it anymore.
        if (index >= _stages.Length) {
            HUD.Instance.MessageWindow.Deactivate();
            Destroy(this);
            return;
        }

        SquadManager.Instance.DisablePlayerInput = _stages[index].ShowCloseButton;

        // All tutorial targets are disabled by default to stop them being triggered out of order. We only activate them
        // when we are at that stage.
        if (CurrentStage.Target != null) {
            CurrentStage.Target.SetActive(true);
        }

        HUD.Instance.MessageWindow.ShowMessage(_stages[index].Title, _stages[index].Message, _stages[index].ShowCloseButton);
        
    }

}

[System.Serializable]
public class TutorialStage {

    /// <summary>
    /// Shown at the top of the message window.
    /// </summary>
    public string Title;

    /// <summary>
    /// The content of the message.
    /// </summary>
    [TextArea(5, 5)]
    public string Message;

    /// <summary>
    /// If true, message will lock player movement and only unlock when player clicks the close button.
    /// </summary>
    public bool ShowCloseButton;

    /// <summary>
    /// If true, next message in tutorial will automatically show when current message is resolved.
    /// </summary>
    public bool ShowNextMessage;

    /// <summary>
    /// The object we are monitoring for this tutorial stage. Once it is no longer in the scene, we consider this stage complete.
    /// </summary>
    public GameObject Target;

}
