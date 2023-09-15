namespace Squiggles.Core.Data;

using System.Collections.Generic;
using Godot;
using Squiggles.Core.Extension;

public class SaveDataBuilder {
  private Dictionary<string, string> _data = new();
  private readonly string _filePath = "";
  private readonly bool _useSlot;
  private readonly bool _alertErrs;

  public SaveDataBuilder(string filePath, bool alertErrs = false, bool useSaveSlot = true) {
    _filePath = filePath;
    _useSlot = useSaveSlot;
    _alertErrs = alertErrs;
  }

  public SaveDataBuilder LoadFromFile() {
    _data = _useSlot
      ? SaveData.CurrentSaveSlot.Load<Dictionary<string, string>>(_filePath, _alertErrs)
      : SaveData.Load<Dictionary<string, string>>(_filePath, _alertErrs);
    _data ??= new(); // protects from null access
    return this;
  }

  public void SaveToFile() {
    if (_data is null) {
      return;
    }

    if (_useSlot) {
      SaveData.CurrentSaveSlot.Save(_data, _filePath);
    }
    else {
      SaveData.Save(_data, _filePath);
    }
  }

  // Store
  public void PutString(string key, string value) {
    if (_data is null) {
      return;
    }

    _data[key] = value;
  }

  public void PutInt(string key, int value) {
    if (_data is null) {
      return;
    }

    _data[key] = value.ToString("G0");
  }
  public void PutFloat(string key, float value) {
    if (_data is null) {
      return;
    }

    _data[key] = value.ToString("G");
  }
  public void PutBool(string key, bool value) {
    if (_data is null) {
      return;
    }

    _data[key] = value.ToString();
  }
  public void PutVector2(string key, Vector2 value) {
    if (_data is null) {
      return;
    }

    var s = $"vec({value.X:G},{value.Y:G})";
    _data[key] = s;
  }

  public void PutVector3(string key, Vector3 value) {
    if (_data is null) {
      return;
    }

    var s = $"vec({value.X:G},{value.Y:G},{value.Z:G})";
    _data[key] = s;
  }

  // Collect
  public string GetString(string key) {
    if (_data is null) {
      return "";
    }

    return !_data.ContainsKey(key) ? "" : _data[key];
  }
  public int GetInt(string key) {
    if (_data is null) {
      return 0;
    }

    if (!_data.ContainsKey(key)) {
      return 0;
    }

    return int.TryParse(_data[key], out var val) ? val : 0;
  }
  public float GetFloat(string key) {
    if (_data is null) {
      return 0f;
    }

    if (!_data.ContainsKey(key)) {
      return 0f;
    }

    return float.TryParse(_data[key], out var val) ? val : 0f;
  }
  public bool GetBool(string key) {
    if (_data is null) {
      return false;
    }

    if (!_data.ContainsKey(key)) {
      return false;
    }

    return bool.TryParse(_data[key], out var val) && val;
  }
  public Vector2 GetVector2(string key) {
    var s = GetString(key);
    if (!s.StartsWith("vec")) {
      return Vector2.Zero;
    }

    var floats = s.ParseFloatsSquiggles();
    return floats.Length != 2 ? Vector2.Zero : new Vector2(floats[0], floats[1]);
  }

  public Vector3 GetVector3(string key) {
    var s = GetString(key);
    if (!s.StartsWith("vec")) {
      return Vector3.Zero;
    }

    var floats = s.ParseFloatsSquiggles();
    return floats.Length != 3 ? Vector3.Zero : new Vector3(floats[0], floats[1], floats[2]);
  }
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
  public bool GetInt(string key, out int value) {
    value = 0;
    if (_data is null) {
      return false;
    }

    return _data.ContainsKey(key) && int.TryParse(_data[key], out value);
  }
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
