using Godot;
using Godot.Collections;

public class Blackboard
{

    private readonly Dictionary<string, Variant> _Local = new();
    private static readonly Dictionary<string, Variant> _Global = new();

    public Variant GetLocal(string key) => _Local[key];
    public Variant GetGlobal(string key) => _Global[key];

    public void SetLocal(string key, Variant value) => _Local[key] = value;
    public void SetGlobal(string key, Variant value) => _Global[key] = value;

    public Variant GetLocalOrDefault(string key, Variant d_val)
    {
        if (!_Local.ContainsKey(key)) return d_val;
        return _Local[key];
    }
    public Variant GetGlobalOrDefault(string key, Variant d_val)
    {
        if (!_Global.ContainsKey(key)) return d_val;
        return _Global[key];
    }

    public Dictionary<string, Variant> GetValuesLocal() => _Local;
    public Dictionary<string, Variant> GetValuesGlobal() => _Global;

    public bool HasLocal(string key) => _Local.ContainsKey(key);
    public bool HasGlobal(string key) => _Global.ContainsKey(key);

}