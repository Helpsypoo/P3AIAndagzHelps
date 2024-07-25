using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UI_Character : MonoBehaviour, IPointerClickHandler {

    [SerializeField] private float _deselectedAlpha = 0.65f;

    private Unit _unit;
    [SerializeField] private Image _selectionBorder;
    [SerializeField] private Image _characterImage;
    [SerializeField] private TextMeshProUGUI _characterName;
    [SerializeField] private TextMeshProUGUI _abilityCharge;
    [SerializeField] private GameObject _actionIcon;


    public void Init(Unit unit) {
        _unit = unit;
        _characterName.text = _unit.UnitStats.Name;
        _abilityCharge.text = unit.AbilityCharges.ToString();
    }

    public void UpdateCharacter() {
        _abilityCharge.text = _unit.AbilityCharges.ToString();
    }

    public void Select() {
        _characterImage.color = Utilities.SetAlpha(_characterImage.color, 1f);
    }

    public void Deselect() {
        _characterImage.color = Utilities.SetAlpha(_characterImage.color, _deselectedAlpha);
    }

    public void UpdateActionVisual(bool state) {
        _actionIcon.SetActive(state);
    }

    public void OnPointerClick(PointerEventData eventData) {

        if (eventData.button == PointerEventData.InputButton.Left) {
            SquadManager.Instance.SelectUnit(_unit);
        } else if (eventData.button == PointerEventData.InputButton.Right) {
            SquadManager.Instance.FollowUnit(_unit);
        }
    }
}
