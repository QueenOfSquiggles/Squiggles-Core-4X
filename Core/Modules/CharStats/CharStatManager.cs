namespace Squiggles.Core.CharStats;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Squiggles.CheeseFruitGroves.Data;
using Squiggles.Core.Data;
using Squiggles.Core.Error;

/// <summary>
/// The root node for CharStat Management. It contains the stats and  ensures everything is as expected. Use this to offload managing certain resource bars in your characters.
/// </summary>
[GlobalClass]
public partial class CharStatManager : Node, IHasSaveData {

  /// <summary>
  /// A signal delegate for <see cref="OnStatChange"/> which notifies when a stat changes
  /// </summary>
  /// <param name="statName">The name of the stat that changed</param>
  /// <param name="value">The value that it is now currently</param>
  [Signal] public delegate void OnStatChangeEventHandler(string statName, float value);

  /// <summary>
  /// A signal delegate for <see cref="OnStatModAdded"/> which notifies when a stat mod (buff/debuff) is added
  /// </summary>
  /// <param name="statName">the name fo the stat</param>
  /// <param name="value">the value which the stat currently is</param>
  /// <param name="modifier">The StatMod enum value, cast to int for easy transfer through Godot</param>
  [Signal] public delegate void OnStatModAddedEventHandler(string statName, float value, int modifier);
  /// <summary>
  /// A signal delegate for <see cref="OnStatModRemoved"/> which notifies when a stat mod (buff/debuff) is removed
  /// </summary>
  /// <param name="statName">the name fo the stat</param>
  /// <param name="value">the value which the stat currently is</param>
  /// <param name="modifier">The StatMod enum value, cast to int for easy transfer through Godot</param>
  [Signal] public delegate void OnStatModRemovedEventHandler(string statName, float value, int modifier);

  /// <summary>
  /// A structure for handling Dynamic Stats, which are stats that generally have an active regeneration rate which can reference another char stat if desired.
  /// DynamicStats are instantiated entrirely through code, but are incredibly useful for things like stamina and cooldowns. Anything that can be quickly exhausted and replenished.
  /// </summary>
  private struct DynStat {
    /// <summary>
    /// The current value of the DynStat
    /// </summary>
    public float Value;
    /// <summary>
    /// The maximum value
    /// </summary>
    public float MaxValue;
    /// <summary>
    /// The amount which this stat regenerates every second. This is calculated using frametime to ensure the regenration is not frame rate dependant.
    /// </summary>
    public float RegenRate;
    /// <summary>
    /// the name of a stat to reference instead of using a flat rate for the max value
    /// </summary>
    public string ReferenceMaxVal;
    /// <summary>
    /// the name of a stat to reference instead of using a flat regen rate
    /// </summary>
    public string ReferenceRegenRate;
  }

  private readonly Dictionary<string, float> _stats = new();
  private readonly Dictionary<string, DynStat> _dynamicStats = new();

  public override void _Ready() => RebuildStatDict();

  /// <summary>
  /// Creates a new <see cref="CharStatFloat"/> with the given values
  /// </summary>
  /// <param name="name">the name of the stat</param>
  /// <param name="value">the initial value of the stat</param>
  /// <param name="modifier">the <see cref="CharStatFloat.Modifier"/> of the stat</param>
  public void AddStat(string name, float value, CharStatFloat.Modifier modifier) {
    var node = new CharStatFloat {
      Name = name,
      StoredValue = value,
      StatMod = modifier
    };
    AddChild(node);
    RebuildStatDict();
  }

  /// <summary>
  /// Creates a <see cref="DynStat"/> for this manager with the given values.
  /// </summary>
  /// <param name="name">the name of the stat</param>
  /// <param name="initialValue">the initial value</param>
  /// <param name="max_value">the maximum value</param>
  /// <param name="regen_rate">the regeneration rate</param>
  /// <param name="refMax">the referenced stat to use instead for the max_value (optional)</param>
  /// <param name="refRegen">the referenced stat to use instead for the regen_rate (optional)</param>
  public void CreateDynamicStat(string name, float initialValue, float max_value, float regen_rate, string refMax = "", string refRegen = "") {
    var dyn = new DynStat() {
      Value = initialValue,
      MaxValue = max_value,
      RegenRate = regen_rate,
      ReferenceMaxVal = refMax,
      ReferenceRegenRate = refRegen
    };
    _dynamicStats[name] = dyn;
  }

  /// <summary>
  /// Applies a stat mod (<see cref="CharStatFloatMod"/>) to the target stat with given values
  /// </summary>
  /// <param name="targetStat">the stat to apply the mod to</param>
  /// <param name="value">the value of the mod</param>
  /// <param name="mod">the <see cref="CharStatFloat.Modifier"/> type.</param>
  /// <param name="duration">the length of time for which this should be active/</param>
  public void AddStatMod(string targetStat, float value, CharStatFloat.Modifier mod, float duration) {
    if (!_stats.ContainsKey(targetStat)) {
      return;
    }

    var node = new CharStatFloatMod {
      StoredValue = value,
      StatMod = mod,
      Duration = duration
    };

    foreach (var c in GetChildren()) {
      if (c.Name == targetStat) {
        c.AddChild(node);
        break;
      }
    }
    node.TreeEntered += () => EmitSignal(nameof(OnStatModAdded), targetStat, value, (int)mod);
    node.TreeExiting += DelayedRebuild;
    node.TreeExiting += () => EmitSignal(nameof(OnStatModRemoved), targetStat, value, (int)mod);
    RebuildStatDict();
    // SceneStatMod
  }

  /// <summary>
  /// Modifies an existing stat (<see cref="CharStatFloat"/> not <see cref="DynStat"/>) with the given delta value. For those unfamiliar, in mathematics, "delta" refers to the value by which something changes. That's why it's commonly associated with the frame time, because it's how much time has changed since the last frame, i.e. the "delta time"
  ///
  /// </summary>
  /// <param name="target">the stat to apply the change to</param>
  /// <param name="delta_value">the scalar value to change it by (addition)</param>
  public void ModifyStaticStat(string target, float delta_value) {
    var n_val = GetStat(target) + delta_value;
    SetStaticStat(target, n_val);
  }

  /// <summary>
  /// Assigns a new value to an existing stat.
  /// </summary>
  /// <param name="target">the stat to target</param>
  /// <param name="n_value">the new value to assign</param>
  public void SetStaticStat(string target, float n_value) {
    var stat_node = GetNode<CharStatFloat>(target);
    if (stat_node is null) {
      return;
    }

    stat_node.StoredValue = n_value;
    RebuildStatDict();
    EmitSignal(nameof(OnStatChange), target, GetStat(target));
  }


  public override void _Process(double delta) {
    var fd = (float)delta;
    foreach (var pair in _dynamicStats) {
      var d = pair.Value;
      if (HasStat(d.ReferenceMaxVal)) {
        d.MaxValue = GetStat(d.ReferenceMaxVal);
      }

      if (HasStat(d.ReferenceRegenRate)) {
        d.RegenRate = GetStat(d.ReferenceRegenRate);
      }

      d.Value += d.RegenRate * fd;
      if (d.Value > d.MaxValue) {
        d.Value = d.MaxValue;
      }

      _dynamicStats[pair.Key] = d;
    }
  }

  private async void DelayedRebuild() {
    // When signals are emitted, tree is invalid for rebuild. 5ms should fix?
    await Task.Delay(5);
    RebuildStatDict();
  }

  private void RebuildStatDict() {
    _stats.Clear();
    foreach (var n in GetChildren()) {
      if (n is not CharStatFloat csf) {
        continue;
      }

      _stats[csf.Name] = csf.GetNetValue();
    }
    // DebugPrintStats();
  }

  /// <summary>
  /// Determine whether a given stat exists
  /// </summary>
  /// <param name="name">the name of the stat</param>
  /// <returns>Whether a <see cref="CharStatFloat"/> or a <see cref="DynStat"/> is currently registred with this manager</returns>
  public bool HasStat(string name) => _stats.ContainsKey(name) || _dynamicStats.ContainsKey(name);

  /// <summary>
  /// Gets the value of a particular stat, preferencing DynStat first for better performance since they are most likely to not be cached.
  /// </summary>
  /// <param name="name">the target stat</param>
  /// <returns>the value of the stat, or zero if none exists</returns>
  public float GetStat(string name) => _dynamicStats.TryGetValue(name, out var d_val) ? d_val.Value :
    (_stats.TryGetValue(name, out var val) ? val : 0.0f);

  /// <summary>
  /// Used to reduce the value of the <see cref="DynStat"/>. They are considered to be a "resource" so consumption is the best way to think of them.
  /// </summary>
  /// <param name="statName">that DynStat to consume</param>
  /// <param name="amount">the amount to consume</param>
  /// <returns>True if the stat was able to be consumed (had enough). And False if the value was insufficient to consume the desired amount.</returns>
  public bool ConsumeDynStat(string statName, float amount) {
    if (!_dynamicStats.ContainsKey(statName)) {
      return false;
    }

    if (_dynamicStats[statName].Value < amount) {
      return false;
    }

    var dyn = _dynamicStats[statName];
    dyn.Value -= amount;
    _dynamicStats[statName] = dyn;
    return true;
  }

  /// <summary>
  /// Determines if a partiular stat is above a certain value without applying any modifications. Useful for gating certain abilites or areas behind stat improvements.
  /// </summary>
  /// <param name="statName">the name of the stat</param>
  /// <param name="minAmount">the required amount (stat >= min)</param>
  /// <returns>true if the stat has the minimum expected value</returns>
  public bool HasStatMinimum(string statName, float minAmount) => HasStat(statName) && GetStat(statName) >= minAmount;

  /// <summary>
  /// Utility for printing out all stats and their values. Useful when debugging the less observable components of the CharStatManager
  /// </summary>
  public void DebugPrintStats() {
    Print.Debug($"Stats Manager: {Name}");
    Print.Debug("[Dynamic Stats]");
    foreach (var pair in _dynamicStats) {
      Print.Debug($"(){pair.Key} = {pair.Value.Value}");
    }
    Print.Debug("[Stats]");
    foreach (var pair in _stats) {
      Print.Debug($"(){pair.Key} = {pair.Value}");
    }
    Print.Debug($"End Stats Manager: {Name}");
  }

  private const string EMBED_KEY = "__stats";
  /// <summary>
  /// Saves the stats out to a <see cref="SaveDataBuilder"/>. This should be called by the parent scene node. Such as the character controller.
  /// </summary>
  /// <param name="build">the builder to use.</param>
  public void SaveToData(ref SaveDataBuilder parentBuilder) {
    var build = new SaveDataBuilder();
    foreach (var stat in _stats) {
      build.PutFloat(stat.Key, stat.Value);
    }
    foreach (var stat in _dynamicStats) {
      build.PutFloat($"{stat.Key}_Value", stat.Value.Value);
      build.PutFloat($"{stat.Key}_Max", stat.Value.MaxValue);
      build.PutFloat($"{stat.Key}_Regen", stat.Value.RegenRate);
    }
    parentBuilder.Append(build, EMBED_KEY);
  }

  /// <summary>
  /// Loads in the properties of the stats from the given <see cref="SaveDataBuilder"/>.  This should be called by the parent scene node. Such as the character controller.
  /// </summary>
  /// <param name="build">the builder to use</param>
  public void LoadFromData(SaveDataBuilder parentBuilder) {
    var build = parentBuilder.LoadEmbedded(EMBED_KEY);
    if (build is null) { return; }
    foreach (var stat in _stats) {
      if (!build.GetFloat(stat.Key, out var val)) {
        continue;
      }

      if (GetNode(stat.Key) is CharStatFloat csf) {
        csf.StoredValue = val;
        EmitSignal(nameof(OnStatChange), stat.Key, val);
      }
    }
    foreach (var stat in _dynamicStats) {
      if (!build.GetFloat($"{stat.Key}_Value", out var val)) {
        continue;
      }

      if (!build.GetFloat($"{stat.Key}_Max", out var max)) {
        continue;
      }

      if (!build.GetFloat($"{stat.Key}_Regen", out var regen)) {
        continue;
      }

      _dynamicStats[stat.Key] = new DynStat {
        Value = val,
        MaxValue = max,
        RegenRate = regen,
        ReferenceMaxVal = stat.Value.ReferenceMaxVal,
        ReferenceRegenRate = stat.Value.ReferenceRegenRate
      };
    }
    RebuildStatDict();
  }

  public void Serialize(SaveDataBuilder builder) => SaveToData(ref builder);
  public void Deserialize(SaveDataBuilder builder) => LoadFromData(builder);
}
