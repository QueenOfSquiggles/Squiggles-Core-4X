namespace Squiggles.Core.Data;

using System.Collections.Generic;
using Godot;
using Squiggles.Core.Extension;

/// <summary>
/// A utility class for serializing and deserializing structured data in a more custom way. Useful to pass as ref between different components.
/// </summary>
public class SaveDataBuilder {
  private Dictionary<string, string> _data = new();
  private readonly string _filePath = "";
  private readonly bool _useCurrentSlot;
  private readonly bool _alertErrs;

  public IEnumerable<KeyValuePair<string, string>> Iterator => _data;
  public IEnumerable<string> Keys => _data.Keys;

  /// <summary>
  /// Constructs a new SaveDataBuilder
  /// </summary>
  /// <param name="filePath">the file path to use</param>
  /// <param name="alertErrs">Whether or not to print out errors on failure. Defaults to false</param>
  /// <param name="useCurrentSaveSlot">Whether or not to use the root save slot ("user://")</param>
  public SaveDataBuilder(string filePath, bool alertErrs = false, bool useCurrentSaveSlot = true) {
    _filePath = filePath;
    _useCurrentSlot = useCurrentSaveSlot;
    _alertErrs = alertErrs;
  }

  /// <summary>
  /// Loads the necessary data from file
  /// </summary>
  /// <returns>returns this calling save datat builder. Constructing a new one when failing to load</returns>
  public SaveDataBuilder LoadFromFile() {
    _data = _useCurrentSlot
      ? SaveData.CurrentSaveSlot.Load<Dictionary<string, string>>(_filePath, _alertErrs)
      : SaveData.Load<Dictionary<string, string>>(_filePath, _alertErrs);
    _data ??= new(); // protects from null access
    return this;
  }

  /// <summary>
  /// Writes out all data to disk using the parameters specified at construction.
  /// </summary>
  public void SaveToFile() {
    if (_data is null) {
      return;
    }

    if (_useCurrentSlot) {
      SaveData.CurrentSaveSlot.Save(_data, _filePath);
    }
    else {
      SaveData.Save(_data, _filePath);
    }
  }

  // Store
  /// <summary>
  /// Stores a string value
  /// </summary>
  /// <param name="key">the key in the data (must be unique)</param>
  /// <param name="value">the value to assign</param>
  public void PutString(string key, string value) {
    if (_data is null) {
      return;
    }

    _data[key] = value;
  }

  /// <summary>
  /// Stores an Int value
  /// </summary>
  /// <param name="key">the key in the data (must be unique)</param>
  /// <param name="value">the value to assign</param>
  public void PutInt(string key, int value) {
    if (_data is null) {
      return;
    }

    _data[key] = value.ToString("G0");
  }
  /// <summary>
  /// Stores a float value
  /// </summary>
  /// <param name="key">the key in the data (must be unique)</param>
  /// <param name="value">the value to assign</param>
  public void PutFloat(string key, float value) {
    if (_data is null) {
      return;
    }

    _data[key] = value.ToString("G");
  }
  /// <summary>
  /// Stores a bool value
  /// </summary>
  /// <param name="key">the key in the data (must be unique)</param>
  /// <param name="value">the value to assign</param>
  public void PutBool(string key, bool value) {
    if (_data is null) {
      return;
    }

    _data[key] = value.ToString();
  }
  /// <summary>
  /// Stores a Vector2 value
  /// </summary>
  /// <param name="key">the key in the data (must be unique)</param>
  /// <param name="value">the value to assign</param>
  public void PutVector2(string key, Vector2 value) {
    if (_data is null) {
      return;
    }

    var s = $"vec({value.X:G},{value.Y:G})";
    _data[key] = s;
  }

  /// <summary>
  /// Stores a Vector3 value
  /// </summary>
  /// <param name="key">the key in the data (must be unique)</param>
  /// <param name="value">the value to assign</param>
  public void PutVector3(string key, Vector3 value) {
    if (_data is null) {
      return;
    }

    var s = $"vec({value.X:G},{value.Y:G},{value.Z:G})";
    _data[key] = s;
  }

  // Collect

  /// <summary>
  /// Parses out a string value from the given key, returning the default value on failure.
  /// </summary>
  /// <param name="key">the key to search for</param>
  /// <returns>the value stored, else the default value</returns>
  public string GetString(string key) {
    if (_data is null) {
      return "";
    }

    return !_data.ContainsKey(key) ? "" : _data[key];
  }
  /// <summary>
  /// Parses out a int value from the given key, returning the default value on failure.
  /// </summary>
  /// <param name="key">the key to search for</param>
  /// <returns>the value stored, else the default value</returns>
  public int GetInt(string key) {
    if (_data is null) {
      return 0;
    }

    if (!_data.ContainsKey(key)) {
      return 0;
    }

    return int.TryParse(_data[key], out var val) ? val : 0;
  }
  /// <summary>
  /// Parses out a float value from the given key, returning the default value on failure.
  /// </summary>
  /// <param name="key">the key to search for</param>
  /// <returns>the value stored, else the default value</returns>
  public float GetFloat(string key) {
    if (_data is null) {
      return 0f;
    }

    if (!_data.ContainsKey(key)) {
      return 0f;
    }

    return float.TryParse(_data[key], out var val) ? val : 0f;
  }
  /// <summary>
  /// Parses out a bool value from the given key, returning the default value on failure.
  /// </summary>
  /// <param name="key">the key to search for</param>
  /// <returns>the value stored, else the default value</returns>
  public bool GetBool(string key) {
    if (_data is null) {
      return false;
    }

    if (!_data.ContainsKey(key)) {
      return false;
    }

    return bool.TryParse(_data[key], out var val) && val;
  }
  /// <summary>
  /// Parses out a Vector2 value from the given key, returning the default value on failure.
  /// </summary>
  /// <param name="key">the key to search for</param>
  /// <returns>the value stored, else the default value</returns>
  public Vector2 GetVector2(string key) {
    var s = GetString(key);
    if (!s.StartsWith("vec")) {
      return Vector2.Zero;
    }

    var floats = s.ParseFloatsSquiggles();
    return floats.Length != 2 ? Vector2.Zero : new Vector2(floats[0], floats[1]);
  }

  /// <summary>
  /// Parses out a Vector3 value from the given key, returning the default value on failure.
  /// </summary>
  /// <param name="key">the key to search for</param>
  /// <returns>the value stored, else the default value</returns>
  public Vector3 GetVector3(string key) {
    var s = GetString(key);
    if (!s.StartsWith("vec")) {
      return Vector3.Zero;
    }

    var floats = s.ParseFloatsSquiggles();
    return floats.Length != 3 ? Vector3.Zero : new Vector3(floats[0], floats[1], floats[2]);
  }

  /// <summary>
  /// Parses out a string value from the key and outputs the value, returning whether or not parsing was successful.
  /// </summary>
  /// <param name="key">the key to search for</param>
  /// <param name="value">the value passed out as an out variable</param>
  /// <returns>true if parsing was successful, false if not</returns>
  public bool GetString(string key, out string value) {
    value = "";
    if (_data is null) {
      return false;
    }

    if (!_data.ContainsKey(key)) {
      return false;
    }

    value = _data[key];
    return true;
  }

  /// <summary>
  /// Parses out a int value from the key and outputs the value, returning whether or not parsing was successful.
  /// </summary>
  /// <param name="key">the key to search for</param>
  /// <param name="value">the value passed out as an out variable</param>
  /// <returns>true if parsing was successful, false if not</returns>
  public bool GetInt(string key, out int value) {
    value = 0;
    if (_data is null) {
      return false;
    }

    return _data.ContainsKey(key) && int.TryParse(_data[key], out value);
  }
  /// <summary>
  /// Parses out a float value from the key and outputs the value, returning whether or not parsing was successful.
  /// </summary>
  /// <param name="key">the key to search for</param>
  /// <param name="value">the value passed out as an out variable</param>
  /// <returns>true if parsing was successful, false if not</returns>
  public bool GetFloat(string key, out float value) {
    value = 0f;
    if (_data is null) {
      return false;
    }

    if (!_data.ContainsKey(key)) {
      return false;
    }

    var passed = float.TryParse(_data[key], out var v);
    value = v;
    return passed;
  }

  /// <summary>
  /// Parses out a bool value from the key and outputs the value, returning whether or not parsing was successful.
  /// </summary>
  /// <param name="key">the key to search for</param>
  /// <param name="value">the value passed out as an out variable</param>
  /// <returns>true if parsing was successful, false if not</returns>
  public bool GetBool(string key, out bool value) {
    value = false;
    if (_data is null) {
      return false;
    }

    if (!_data.ContainsKey(key)) {
      return false;
    }

    var passed = bool.TryParse(_data[key], out var v);
    value = v;
    return passed;
  }
  /// <summary>
  /// Parses out a Vector2 value from the key and outputs the value, returning whether or not parsing was successful.
  /// </summary>
  /// <param name="key">the key to search for</param>
  /// <param name="value">the value passed out as an out variable</param>
  /// <returns>true if parsing was successful, false if not</returns>

  public bool GetVector2(string key, out Vector2 value) {
    value = Vector2.Zero;
    if (_data is null) {
      return false;
    }

    var s = GetString(key);
    if (!s.StartsWith("vec2")) {
      return false;
    }

    var floats = s.Replace("vec2(", "").Replace(")", "").SplitFloats(",");
    if (floats.Length != 2) {
      return false;
    }

    value = new Vector2(floats[0], floats[1]);
    return true;
  }

  /// <summary>
  /// Parses out a Vector3 value from the key and outputs the value, returning whether or not parsing was successful.
  /// </summary>
  /// <param name="key">the key to search for</param>
  /// <param name="value">the value passed out as an out variable</param>
  /// <returns>true if parsing was successful, false if not</returns>
  public bool GetVector3(string key, out Vector3 value) {
    value = Vector3.Zero;
    if (_data is null) {
      return false;
    }

    var s = GetString(key);
    if (!s.StartsWith("vec3")) {
      return false;
    }

    var floats = s.Replace("vec3(", "").Replace(")", "").SplitFloats(",");
    if (floats.Length != 3) {
      return false;
    }

    value = new Vector3(floats[0], floats[1], floats[2]);
    return true;
  }


}
