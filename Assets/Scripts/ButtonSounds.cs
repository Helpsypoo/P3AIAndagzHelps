using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSounds : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler {

    public void OnPointerEnter(PointerEventData eventData) {
        AudioManager.Instance.Play(AudioManager.Instance.UIHover, MixerGroups.UI);
    }

    public void OnPointerDown(PointerEventData eventData) {
        AudioManager.Instance.Play(AudioManager.Instance.UIClick, MixerGroups.UI);
    }
}
