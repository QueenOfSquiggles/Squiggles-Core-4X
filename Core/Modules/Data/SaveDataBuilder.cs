using System.Collections.Generic;
using Godot;
using queen.error;
using SquiggleZoneGameBase.Modules.Extensions;

namespace queen.data;

public class SaveDataBuilder
{
    private Dictionary<string, string> _Data = new();
    private string _FilePath = "";
    private bool _UseSlot = false;
    private bool _AlertErrs = false;

    public SaveDataBuilder(string filePath, bool alertErrs = false, bool useSaveSlot = true)
    {
        _FilePath = filePath;
        _UseSlot = useSaveSlot;
        _AlertErrs = alertErrs;
    }

    public SaveDataBuilder LoadFromFile()
    {
        if (_UseSlot) _Data = Data.CurrentSaveSlot.Load<Dictionary<string, string>>(_FilePath, _AlertErrs);
        else _Data = Data.Load<Dictionary<string, string>>(_FilePath, _AlertErrs);
        _Data ??= new(); // protects from null access
        return this;
    }

    public void SaveToFile()
    {
        if (_UseSlot) Data.CurrentSaveSlot.Save(_Data, _FilePath);
        else Data.Save(_Data, _FilePath);
    }

    // Store
    public void PutString(string key, string value) => _Data[key] = value;

    public void PutInt(string key, int value) => _Data[key] = value.ToString("G0");
    public void PutFloat(string key, float value) => _Data[key] = value.ToString("G");
    public void PutBool(string key, bool value) => _Data[key] = value.ToString();
    public void PutVector2(string key, Vector2 value)
    {
        var s = $"vec({value.X:G},{value.Y:G})";
        _Data[key] = s;
    }

    public void PutVector3(string key, Vector3 value)
    {
        var s = $"vec({value.X:G},{value.Y:G},{value.Z:G})";
        _Data[key] = s;
    }

    // Collect
    public string GetString(string key)
    {
        if (!_Data.ContainsKey(key)) return "";
        return _Data[key];
    }
    public int GetInt(string key)
    {
        if (!_Data.ContainsKey(key)) return 0;
        if (int.TryParse(_Data[key], out int val)) return val;
        return 0;
    }
    public float GetFloat(string key)
    {
        if (!_Data.ContainsKey(key)) return 0f;
        if (float.TryParse(_Data[key], out float val)) return val;
        return 0f;
    }
    public bool GetBool(string key)
    {
        if (!_Data.ContainsKey(key)) return false;
        if (bool.TryParse(_Data[key], out bool val)) return val;
        return false;
    }
    public Vector2 GetVector2(string key)
    {
        var s = GetString(key);
        if (!s.StartsWith("vec")) return Vector2.Zero;
        var floats = s.ParseFloatsSquiggles();
        if (floats.Length != 2) return Vector2.Zero;
        return new Vector2(floats[0], floats[1]);
    }

    public Vector3 GetVector3(string key)
    {
        var s = GetString(key);
        if (!s.StartsWith("vec")) return Vector3.Zero;
        var floats = s.ParseFloatsSquiggles();
        if (floats.Length != 3) return Vector3.Zero;
        return new Vector3(floats[0], floats[1], floats[2]);
    }
    public bool GetString(string key, out string value)
    {
        value = "";
        if (!_Data.ContainsKey(key)) return false;
        value = _Data[key];
        return true;
    }
    public bool GetInt(string key, out int value)
    {
        value = 0;
        if (!_Data.ContainsKey(key)) return false;
        return int.TryParse(_Data[key], out value);
    }
    public bool GetFloat(string key, out float value)
    {
        value = 0f;
        if (!_Data.ContainsKey(key)) return false;
        bool passed = float.TryParse(_Data[key], out float v);
        value = v;
        return passed;
    }

    public bool GetBool(string key, out bool value)
    {
        value = false;
        if (!_Data.ContainsKey(key)) return false;
        bool passed = bool.TryParse(_Data[key], out bool v);
        value = v;
        return passed;
    }
    public bool GetVector2(string key, out Vector2 value)
    {
        value = Vector2.Zero;
        var s = GetString(key);
        if (!s.StartsWith("vec2")) return false;
        var floats = s.Replace("vec2(", "").Replace(")", "").SplitFloats(",");
        if (floats.Length != 2) return false;
        value = new Vector2(floats[0], floats[1]);
        return true;
    }

    public bool GetVector3(string key, out Vector3 value)
    {
        value = Vector3.Zero;
        var s = GetString(key);
        if (!s.StartsWith("vec3")) return false;
        var floats = s.Replace("vec3(", "").Replace(")", "").SplitFloats(",");
        if (floats.Length != 3) return false;
        value = new Vector3(floats[0], floats[1], floats[2]);
        return true;
    }


}
