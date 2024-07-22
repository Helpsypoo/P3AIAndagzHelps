using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Character : MonoBehaviour {

    private Unit _unit;
    [SerializeField] private Image _selectionBorder;
    [SerializeField] private Image _characterImage;
    [SerializeField] private TextMeshProUGUI _characterName;
    [SerializeField] private TextMeshProUGUI _abilityCharge;

    public void Init(Unit unit) {
        _unit = unit;
        _selectionBorder.color = Utilities.MakeTransparent(unit.UnitStats.Colour);
        _characterName.text = _unit.UnitStats.Name;
        _abilityCharge.text = unit.AbilityCharges.ToString();
    }

    public void UpdateCharacter() {
        _abilityCharge.text = _unit.AbilityCharges.ToString();
    }

    public void Select() {
        _selectionBorder.color = Utilities.MakeOpaque(_unit.UnitStats.Colour);
    }

    public void Deselect() {
        _selectionBorder.color = Utilities.MakeTransparent(_unit.UnitStats.Colour);
    }

}
