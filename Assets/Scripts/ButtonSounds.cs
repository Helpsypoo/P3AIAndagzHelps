using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSounds : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler {
    private Button _button;

    private void Awake() {
        _button = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (!_button || !_button.interactable) {
            return;
        }
        AudioManager.Instance.Play(AudioManager.Instance.UIHover, MixerGroups.UI);
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (!_button || !_button.interactable) {
            return;
        }
        AudioManager.Instance.Play(AudioManager.Instance.UIClick, MixerGroups.UI);
    }
}
