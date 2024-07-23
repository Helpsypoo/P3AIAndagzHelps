using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

public static class Utils {
	public static bool IntToBool(int _value) {
		if (_value != 0 && _value != 1) {
			Debug.LogError("Attempting to get bool from int for value other than 1 or 0.  Defaulting bool to false.");
			return false;
		}

		return _value == 1 ? true : false;
	}

	public static int BoolToInt(bool _value) {
		return _value ? 1 : 0;
	}

	public static string GetChars(string _string, int _length) {
		if (!string.IsNullOrEmpty(_string)) {
			_length = Mathf.Clamp(_length, 0, _string.Length);
				return _string.Substring(0, _length);
		}

		return string.Empty;
	}

	public static bool IsPointOnNavMesh(Vector3 point) {
        NavMeshHit hit;
        bool found = NavMesh.SamplePosition(point, out hit, 0.3f, NavMesh.AllAreas);
        return found;
    }

 
    
    public static void PauseGame() {
	    if(Settings.Instance) Settings.Instance.SetTimeScale(0f, 1.5f);
    }

    public static void UnpauseGame() {
	    if(Settings.Instance) Settings.Instance.SetTimeScale(1f, .5f);
    }

    public static T ParseEnumOrDefault<T>(string value) where T : struct, Enum
    {
	    // Check if the string matches any enum value
	    if (Enum.TryParse<T>(value, true, out var result) && Enum.IsDefined(typeof(T), result))
	    {
		    return result;
	    }

	    // If no match is found, return the first enum value
	    return Enum.GetValues(typeof(T)).Cast<T>().First();
    }

    public static void AssignValueAtIndex<T>(List<T> list, int index, T value) {
	    if (index < 0) {
		    throw new ArgumentOutOfRangeException("Index cannot be negative.");
	    }

	    // Expand the list if the index is greater than or equal to the list's count
	    while (index >= list.Count) {
		    list.Add(default(T));
	    }

	    // Assign the value to the index
	    list[index] = value;
    }
    
    public static List<Transform> FindChildrenWithNameContains(Transform parent, string nameSubstring) {
	    List<Transform> foundChildren = new List<Transform>();

	    // Check each child of the parent object
	    foreach (Transform child in parent) {
		    // If the child's name contains the specified substring, add it to the list
		    if (child.name.Contains(nameSubstring)) {
			    foundChildren.Add(child);
		    }

		    // Check all descendants,
		    foundChildren.AddRange(FindChildrenWithNameContains(child, nameSubstring));
	    }

	    return foundChildren;
    }
    
    public static T GetOrAddComponent<T>(this Component component) where T : Component
    {
	    T a = component.GetComponent<T>();
	    if (a != null)
	    {
		    return a;
	    }

	    return component.gameObject.AddComponent<T>();
    }

    public static Transform FindChild(Transform parent, string childName) {
	    foreach (Transform child in parent) {
		    if (child.name == childName) {
			    return child;
		    }

		    Transform found = FindChild(child, childName);
		    if (found != null) return found;
	    }
	    return null;
    }
    
    public static Color HexToColor(string hex) {

	    if (hex.StartsWith("#")) {
		    hex = hex.Substring(1);
	    }
	    
	    byte _r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
	    byte _g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
	    byte _b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
	    byte _a = 255; // Default alpha value is 255 (fully opaque).
	    
	    if (hex.Length == 8) {
		    _a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
	    }
	    
	    return new Color(_r / 255f, _g / 255f, _b / 255f, _a / 255f);
    }
}