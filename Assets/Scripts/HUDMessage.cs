using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUDMessage : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _content;
    [SerializeField] private GameObject _closeButton;
    [SerializeField] private Vector2 _typeDelayRange;
    [SerializeField] private AudioClip _typeSound;

    public bool IsTyping { get; private set; }

    private Coroutine _typeRoutine;

    public void ShowMessage(string title, string message, bool hideButton = true) {
        Activate();
        if (_typeRoutine != null) {
            StopCoroutine(_typeRoutine);
            _typeRoutine = null;
        }
        _title.text = title;
        _closeButton.SetActive(hideButton);
        _typeRoutine = StartCoroutine(TypeMessage(message));
    }

    private IEnumerator TypeMessage(string message) {

        IsTyping = true;

        string output = "";
        Debug.Log("Message Length: " + message.Length);
        foreach (char c in message) {

            output += c;
            _content.text = output;
            if (c.ToString() == " ") {
                AudioManager.Instance.Play(_typeSound, MixerGroups.UI, default, 0.2f);
            }
            yield return new WaitForSeconds(Random.Range(_typeDelayRange.x, _typeDelayRange.y));

        }

        IsTyping = false;

    }

    public void CompleteTutorialTask() {
        GameManager.Instance.ActiveLevel.Tutorial.CompleteCurrentTask();
    }

    public void Activate() {
        gameObject.SetActive(true);
    }

    public void Deactivate() {
        gameObject.SetActive(false);
    }

}
