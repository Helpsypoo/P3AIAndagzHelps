using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonGraphicEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, ISelectHandler, IDeselectHandler
{
	[SerializeField] private Button _button;
	[SerializeField] private Graphic[] _graphics;

	[SerializeField] private Color _normalColor;
	[SerializeField] private Color _highlightedColor;
	[SerializeField] private Color _pressedColor;
	[SerializeField] private Color _selectedColor;
	[SerializeField] private Color _disabledColor;

	private void Start() {
		UpdateGraphics(_button.interactable ? _normalColor : _disabledColor);
	}

	public void OnPointerEnter(PointerEventData eventData) {
		UpdateGraphics(_highlightedColor);
	}

	public void OnPointerExit(PointerEventData eventData) {
		UpdateGraphics(_button.interactable ? _normalColor : _disabledColor);
	}

	public void OnPointerDown(PointerEventData eventData) {
		UpdateGraphics(_pressedColor);
	}

	public void OnPointerUp(PointerEventData eventData) {
		UpdateGraphics(_button.interactable ? _highlightedColor : _disabledColor);
	}

	public void OnSelect(BaseEventData eventData) {
		UpdateGraphics(_selectedColor);
	}

	public void OnDeselect(BaseEventData eventData) {
		UpdateGraphics(_normalColor);
	}

	private void OnEnable() {
		UpdateGraphics(_button.interactable ? _normalColor : _disabledColor);
	}

	private void OnDisable() {
		UpdateGraphics(_disabledColor);
	}

	private void UpdateGraphics(Color color) {
		foreach (var graphic in _graphics) {
			graphic.color = color;
		}
	}

	private void OnValidate() {
		if (_button != null) {
			UpdateGraphics(_button.interactable ? _normalColor : _disabledColor);
		}
	}
}