namespace SquigglesBT;

using Godot;
using Godot.Collections;
public class Blackboard {

  private readonly Dictionary<string, Variant> _local = new();
  private static readonly Dictionary<string, Variant> _global = new();

  public Variant GetLocal(string key) => _local[key];
  public Variant GetGlobal(string key) => _global[key];

  public void SetLocal(string key, Variant value) => _local[key] = value;
  public void SetGlobal(string key, Variant value) => _global[key] = value;

  public Variant GetLocalOrDefault(string key, Variant d_val) => !_local.ContainsKey(key) ? d_val : _local[key];
  public Variant GetGlobalOrDefault(string key, Variant d_val) => !_global.ContainsKey(key) ? d_val : _global[key];

  public Dictionary<string, Variant> GetValuesLocal() => _local;
  public Dictionary<string, Variant> GetValuesGlobal() => _global;

  public bool HasLocal(string key) => _local.ContainsKey(key);
  public bool HasGlobal(string key) => _global.ContainsKey(key);

}
