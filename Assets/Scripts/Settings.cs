using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;


public class Settings : MonoBehaviour
{
	public static Settings Instance;
	private Camera mainCamera;
    public SettingsValue[] SettingsValues { get; private set; }

	private InputAction cancel;

	private string controlScheme;

	private Coroutine timescaleCoroutine;
	private Coroutine timeCoroutine;
	
	private void Awake() {
		if(Instance != null && Instance != this) {
			Destroy(gameObject);
			return;
		} else {
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}
	
	public void SetTimeScale(float _value, float _duration = 0) {
		if (timescaleCoroutine != null) {
			StopCoroutine(timescaleCoroutine);
			timescaleCoroutine = null;
		}
		timescaleCoroutine = StartCoroutine(SetTimescaleAfterFrame(_value, _duration));
	}
	IEnumerator SetTimescaleAfterFrame(float _value, float _duration = 0) {
		float _time = 0;
		float _startingValue = Time.timeScale;
		while (_time < _duration) {
			_time += Time.unscaledDeltaTime;
			float _newValue = Mathf.Lerp(_startingValue, _value, _time / _duration);
			Time.timeScale = _newValue;
			yield return new WaitForEndOfFrame();
		}
		Time.timeScale = _value;
	}

	public void SetFrameRate(int value) {
		//savedFramerateLimit = Application.targetFrameRate;
		Application.targetFrameRate = value;
	}

	public void RevertFrameRate() {
		//Application.targetFrameRate = savedFramerateLimit;
	}
	
	private void InitSettings() {
		SettingsValues = new[]{
			//General
			new SettingsValue("RunInBackground", PlayerPrefs.GetString("RunInBackground", "0"), "0"),
			//Video
			new SettingsValue("ShowFPS", PlayerPrefs.GetString("ShowFPS", "0"), "0"),
			new SettingsValue("VSync", PlayerPrefs.GetString("VSync", "1"), "1"),
			//Audio
			new SettingsValue("MasterVolume", PlayerPrefs.GetString("MasterVolume", "100"), "100"),
			new SettingsValue("AmbianceVolume", PlayerPrefs.GetString("AmbianceVolume", "100"), "100"),
			new SettingsValue("MusicVolume", PlayerPrefs.GetString("MusicVolume", "100"), "100"),
			new SettingsValue("SFXVolume", PlayerPrefs.GetString("SFXVolume", "100"), "100"),
			new SettingsValue("UIVolume", PlayerPrefs.GetString("UIVolume", "100"), "100")
			//Controls
		};
	}

	public SettingsValue? GetSetting(string _ref) {
		foreach (SettingsValue _setting in SettingsValues) {
			if (_ref == _setting.Reference) {
				return _setting;
			}
		}
		
		Debug.LogError($"No reference found in settings for {_ref}. Returning empty string");
		return null;
	}
	
	public bool SetSetting(string _ref, string _value) {
		for (int i = 0; i < SettingsValues.Length; i++) {
			if (_ref == SettingsValues[i].Reference) {
				SettingsValues[i] = new SettingsValue(SettingsValues[i].Reference, _value, SettingsValues[i].DefaultValue);
				PlayerPrefs.SetString(_ref, _value);
				return true;
			}
		}

		return false;
	}

	private void SetSettingToDefault(string _ref) {
		SettingsValue _setting = GetSetting(_ref).Value;
		for (int i = 0; i < SettingsValues.Length; i++) {
			if (_ref == SettingsValues[i].Reference) {
				SettingsValues[i] = new SettingsValue(SettingsValues[i].Reference, SettingsValues[i].DefaultValue, SettingsValues[i].DefaultValue);
				PlayerPrefs.SetString(_setting.Reference, _setting.DefaultValue);
			}
		}
	}

	public void UpdateRunInBackground(bool _value) {
		Application.runInBackground = _value;
		SetSetting("RunInBackground", _value.ToString());
	}
	
	public void UpdateVSync(bool _value) {
		Application.targetFrameRate = _value ? -1 : 60; //TODO replace 60 with custom targetFrameRate
		QualitySettings.vSyncCount = _value ? 1 : 0;
		//Debug.Log($"VSync set to {_value}. Framerate: {Application.targetFrameRate} | VsyncCount: {QualitySettings.vSyncCount}");
		SetSetting("VSync", _value.ToString());
	}

	private void SetSavedSettings() {
		//General
		UpdateRunInBackground(GetSetting("RunInBackground").Value.ToBool());
		//Video
		UpdateVSync(GetSetting("VSync").Value.ToBool());
		//Audio
		AudioManager.Instance.UpdateVolume("MasterVolume", GetSetting("MasterVolume").Value.Value);
		AudioManager.Instance.UpdateVolume("MusicVolume", GetSetting("MusicVolume").Value.Value);
		AudioManager.Instance.UpdateVolume("SFXVolume", GetSetting("SFXVolume").Value.Value);
		AudioManager.Instance.UpdateVolume("AmbianceVolume", GetSetting("AmbianceVolume").Value.Value);
		AudioManager.Instance.UpdateVolume("UIVolume", GetSetting("UIVolume").Value.Value);
		//Controls
	}
	
	public void ResetGeneralSettings() {
		SetSettingToDefault("RunInBackground");
	}
	
	public void ResetVideoSettings() {
		SetSettingToDefault("ShowFPS");
		SetSettingToDefault("VSync");
	}

	public void ResetAudioSettings() {
		SetSettingToDefault("MasterVolume");
		SetSettingToDefault("MusicVolume");
		SetSettingToDefault("SFXVolume");
		SetSettingToDefault("AmbianceVolume");
		SetSettingToDefault("UIVolume");
		SetSettingToDefault("SpeechVolume");
	}
}
