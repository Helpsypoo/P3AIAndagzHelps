public struct SettingsValue {
	public SettingsValue(string _reference, string _value, string _defaultValue) {
		Reference = _reference;
		Value = _value;
		DefaultValue = _defaultValue;
	}

	public string Reference { get; }
	public string Value { get; }
	public string DefaultValue { get; }

	public float ToFloat() {
		float.TryParse(Value, out float _parsedValue);
		return _parsedValue;
	}
	
	public int ToInt() {
		int.TryParse(Value, out int _parsedValue);
		return _parsedValue;
	}
	
	public bool ToBool() {
		if(Value.ToLower() == "true" ||
		   Value == "1") {
			return true;
		}
			
		return false;
	}
	
	public override string ToString() {
		return Value;
	}
}
