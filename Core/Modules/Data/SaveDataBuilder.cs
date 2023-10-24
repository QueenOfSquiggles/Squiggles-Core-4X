namespace Squiggles.Core.Data;

using System;
using System.Linq;
using Godot;
using Godot.Collections;

/// <summary>
/// A utility class for serializing and deserializing structured data in a more custom way. Useful to pass as ref between different components.
/// </summary>
public class SaveDataBuilder : IDisposable {
  private Dictionary _data = new();
  private readonly string _filePath = "";
  private readonly bool _useCurrentSlot;
  private readonly bool _alertErrs;
  private readonly bool _isEmbedded;

  public System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, string>> Iterator => _data
      .Where((entry) => entry.Key.VariantType == Variant.Type.String && entry.Value.VariantType == Variant.Type.String)
      .ToList()
      .ToDictionary((entry) => entry.Key.AsString(), (entry) => entry.Value.AsString());

  public System.Collections.Generic.IEnumerable<string> Keys => _data.ToList().ConvertAll((entry) => entry.Value.AsString());

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

  public SaveDataBuilder(bool isEmbedded = true) {
    _isEmbedded = isEmbedded;
  }

  /// <summary>
  /// Loads the necessary data from file
  /// </summary>
  /// <returns>returns this calling save datat builder. Constructing a new one when failing to load</returns>
  public SaveDataBuilder LoadFromFile() {
    if (_isEmbedded) {
      return null;
    }
    _data = _useCurrentSlot
      ? SaveData.CurrentSaveSlot.LoadDict(_filePath, _alertErrs)
      : SaveData.LoadDict(_filePath, _alertErrs);
    _data ??= new(); // protects from null access
    return this;
  }

  /// <summary>
  /// Writes out all data to disk using the parameters specified at construction.
  /// </summary>
  public void SaveToFile() {
    if (_isEmbedded) {
      return;
    }

    if (_useCurrentSlot) {
      SaveData.CurrentSaveSlot.SaveDict(_data, _filePath);
    }
    else {
      SaveData.SaveDict(_data, _filePath);
    }
  }

  public void Append(SaveDataBuilder childBuilder, string subKey) => _data[subKey] = childBuilder._data;

  public SaveDataBuilder LoadEmbedded(string subKey) =>
    !_data.ContainsKey(subKey)
      ? null
      : new SaveDataBuilder() {
        _data = _data[subKey].AsGodotDictionary()
      };

  public void PutVariant(string key, Variant value) => _data[key] = value;

  // Store
  /// <summary>
  /// Stores a string value
  /// </summary>
  /// <param name="key">the key in the data (must be unique)</param>
  /// <param name="value">the value to assign</param>
  public void PutString(string key, string value) => _data[key] = value;

  /// <summary>
  /// Stores an Int value
  /// </summary>
  /// <param name="key">the key in the data (must be unique)</param>
  /// <param name="value">the value to assign</param>
  public void PutInt(string key, int value) => _data[key] = value;
  /// <summary>
  /// Stores a float value
  /// </summary>
  /// <param name="key">the key in the data (must be unique)</param>
  /// <param name="value">the value to assign</param>
  public void PutFloat(string key, float value) => _data[key] = value;
  /// <summary>
  /// Stores a bool value
  /// </summary>
  /// <param name="key">the key in the data (must be unique)</param>
  /// <param name="value">the value to assign</param>
  public void PutBool(string key, bool value) => _data[key] = value;
  /// <summary>
  /// Stores a Vector2 value
  /// </summary>
  /// <param name="key">the key in the data (must be unique)</param>
  /// <param name="value">the value to assign</param>
  public void PutVector2(string key, Vector2 value) => _data[key] = new float[] { value.X, value.Y };

  /// <summary>
  /// Stores a Vector3 value
  /// </summary>
  /// <param name="key">the key in the data (must be unique)</param>
  /// <param name="value">the value to assign</param>
  public void PutVector3(string key, Vector3 value) => _data[key] = new float[] { value.X, value.Y, value.Z };

  // Collect

  /// <summary>
  /// Parses out a string value from the given key, returning the default value on failure.
  /// </summary>
  /// <param name="key">the key to search for</param>
  /// <returns>the value stored, else the default value</returns>
  public string GetString(string key) => !_data.ContainsKey(key) ? "" : _data[key].AsString();
  /// <summary>
  /// Parses out a int value from the given key, returning the default value on failure.
  /// </summary>
  /// <param name="key">the key to search for</param>
  /// <returns>the value stored, else the default value</returns>
  public int GetInt(string key) => !_data.ContainsKey(key) ? 0 : _data[key].AsInt32();

  /// <summary>
  /// Parses out a float value from the given key, returning the default value on failure.
  /// </summary>
  /// <param name="key">the key to search for</param>
  /// <returns>the value stored, else the default value</returns>
  public float GetFloat(string key) => !_data.ContainsKey(key) ? 0f : _data[key].AsSingle();
  /// <summary>
  /// Parses out a bool value from the given key, returning the default value on failure.
  /// </summary>
  /// <param name="key">the key to search for</param>
  /// <returns>the value stored, else the default value</returns>
  public bool GetBool(string key) {


    if (!_data.ContainsKey(key)) {
      return false;
    }

    return _data[key].AsBool();
  }
  /// <summary>
  /// Parses out a Vector2 value from the given key, returning the default value on failure.
  /// </summary>
  /// <param name="key">the key to search for</param>
  /// <returns>the value stored, else the default value</returns>
  public Vector2 GetVector2(string key) {
    if (!_data.ContainsKey(key)) { return Vector2.Zero; }
    var floats = _data[key].AsFloat32Array();
    return floats.Length != 2 ? Vector2.Zero : new Vector2(floats[0], floats[1]);
  }

  /// <summary>
  /// Parses out a Vector3 value from the given key, returning the default value on failure.
  /// </summary>
  /// <param name="key">the key to search for</param>
  /// <returns>the value stored, else the default value</returns>
  public Vector3 GetVector3(string key) {
    if (!_data.ContainsKey(key)) { return Vector3.Zero; }
    var floats = _data[key].AsFloat32Array();
    return floats.Length != 3 ? Vector3.Zero : new Vector3(floats[0], floats[1], floats[2]);
  }

  /// <summary>
  /// Parses out a string value from the key and outputs the value, returning whether or not parsing was successful.
  /// </summary>
  /// <param name="key">the key to search for</param>
  /// <param name="value">the value passed out as an out variable</param>
  /// <returns>true if parsing was successful, false if not</returns>
  public bool GetString(string key, out string value) {

    if (!_data.ContainsKey(key)) {
      value = "";
      return false;
    }

    value = _data[key].AsString();
    return true;
  }

  /// <summary>
  /// Parses out a int value from the key and outputs the value, returning whether or not parsing was successful.
  /// </summary>
  /// <param name="key">the key to search for</param>
  /// <param name="value">the value passed out as an out variable</param>
  /// <returns>true if parsing was successful, false if not</returns>
  public bool GetInt(string key, out int value) {
    if (_data.ContainsKey(key)) {
      value = _data[key].AsInt32();
      return true;
    }
    value = 0;
    return false;
  }
  /// <summary>
  /// Parses out a float value from the key and outputs the value, returning whether or not parsing was successful.
  /// </summary>
  /// <param name="key">the key to search for</param>
  /// <param name="value">the value passed out as an out variable</param>
  /// <returns>true if parsing was successful, false if not</returns>
  public bool GetFloat(string key, out float value) {
    if (!_data.ContainsKey(key)) {
      value = 0;
      return false;
    }
    value = _data[key].AsSingle();
    return true;
  }

  /// <summary>
  /// Parses out a bool value from the key and outputs the value, returning whether or not parsing was successful.
  /// </summary>
  /// <param name="key">the key to search for</param>
  /// <param name="value">the value passed out as an out variable</param>
  /// <returns>true if parsing was successful, false if not</returns>
  public bool GetBool(string key, out bool value) {

    if (!_data.ContainsKey(key)) {
      value = false;
      return false;
    }
    value = _data[key].AsBool();
    return true;
  }
  /// <summary>
  /// Parses out a Vector2 value from the key and outputs the value, returning whether or not parsing was successful.
  /// </summary>
  /// <param name="key">the key to search for</param>
  /// <param name="value">the value passed out as an out variable</param>
  /// <returns>true if parsing was successful, false if not</returns>

  public bool GetVector2(string key, out Vector2 value) {

    value = Vector2.Zero;
    if (!_data.ContainsKey(key)) {
      return false;
    }

    var floats = _data[key].AsFloat32Array();
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
    if (!_data.ContainsKey(key)) { return false; }
    var floats = _data[key].AsFloat32Array();
    if (floats.Length != 3) {
      return false;
    }

    value = new Vector3(floats[0], floats[1], floats[2]);
    return true;
  }

  void IDisposable.Dispose() {
    GC.SuppressFinalize(this);
    _data.Dispose();
  }
}
