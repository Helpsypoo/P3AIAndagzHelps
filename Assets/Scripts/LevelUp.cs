using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUp : MonoBehaviour {
	public TextMeshProUGUI Name;
	public Image HealthValue;
	public Image HealthUpgradeValue;
	public Image SpeedValue;
	public Image SpeedUpgradeValue;	
	public Image DamageValue;
	public Image DamageUpgradeValue;	
	public Image AbilityValue;
	public Image AbilityUpgradeValue;
	public TextMeshProUGUI Description;
	public TextMeshProUGUI Cost;
	public Button Upgrade;

	private Unit _unit;
	private LevelUpManager _levelUpManager;
	[SerializeField] private ButtonGraphicEffects _buttonEffects;

	private void Start() {
		HideUpgrade();
	}

	public void UpdateDisplay(Unit _unitValue, LevelUpManager _manager) {
		_unit = _unitValue;
		_levelUpManager = _manager;
		
		Name.text = _unit.UnitStats.Name;

		UnitUpgrade _currentLevel = _unit.UnitStats.UnitUpgrades[SessionManager.Instance.GetLevel(_unit)];


		HealthValue.fillAmount = _currentLevel.MaxHealth/500f;
		SpeedValue.fillAmount = _currentLevel.Speed/10f;
		DamageValue.fillAmount = Mathf.Abs(_currentLevel.Damage)/10f;
		AbilityValue.fillAmount = _currentLevel.Ability/10f;

		UpdateAvailability();
	}

	public void ShowUpgrade() {
		if (SessionManager.Instance.GetLevel(_unit) + 1 > _unit.UnitStats.UnitUpgrades.Length) {
			Upgrade.interactable = false;
			Cost.text = "";
			Description.text = "No more upgrades";
			return;
		}
		
		UnitUpgrade _nextLevel = _unit.UnitStats.UnitUpgrades[SessionManager.Instance.GetLevel(_unit) + 1];
		HealthUpgradeValue.fillAmount = _nextLevel.MaxHealth/500f;
		SpeedUpgradeValue.fillAmount = _nextLevel.Speed/10f;
		DamageUpgradeValue.fillAmount = Mathf.Abs(_nextLevel.Damage)/10f;
		AbilityUpgradeValue.fillAmount = _nextLevel.Ability/10f;
		Description.text = _nextLevel.Description;
	}
	
	public void HideUpgrade() {
		HealthUpgradeValue.fillAmount = 0;
		SpeedUpgradeValue.fillAmount = 0;
		DamageUpgradeValue.fillAmount = 0;
		AbilityUpgradeValue.fillAmount = 0;
		Description.text = "";
	}

	public void UpdateAvailability() {
		if (SessionManager.Instance.GetLevel(_unit) + 1 > _unit.UnitStats.UnitUpgrades.Length) {
			Upgrade.interactable = false;
			_buttonEffects.CheckDisabled();
			Cost.text = "";
			return;
		}
		
		if (SessionManager.Instance.BlueGoop < _unit.UnitStats.UnitUpgrades[SessionManager.Instance.GetLevel(_unit) + 1].Cost) {
			Upgrade.interactable = false;
			_buttonEffects.CheckDisabled();
			Cost.text = $"({_unit.UnitStats.UnitUpgrades[SessionManager.Instance.GetLevel(_unit) + 1].Cost})";
			Cost.color = Color.gray;
			return;
		}
		
		_buttonEffects.CheckDisabled();
		Cost.text = $"({_unit.UnitStats.UnitUpgrades[SessionManager.Instance.GetLevel(_unit) + 1].Cost})";
		Cost.color = HealthValue.color;
	}

	public void BuyUpgrade() {
		if (SessionManager.Instance.BlueGoop < _unit.UnitStats.UnitUpgrades[SessionManager.Instance.GetLevel(_unit) + 1].Cost) {
			Upgrade.interactable = false;
			_buttonEffects.CheckDisabled();
			return;
		}

		AudioManager.Instance.Play(AudioManager.Instance.PurchaseUpgrade, MixerGroups.UI);

		SessionManager.Instance.BlueGoop -= _unit.UnitStats.UnitUpgrades[SessionManager.Instance.GetLevel(_unit) + 1].Cost;
		SessionManager.Instance.AddLevel(_unit);
		_levelUpManager.UpdateLevelUps();
	}
}
