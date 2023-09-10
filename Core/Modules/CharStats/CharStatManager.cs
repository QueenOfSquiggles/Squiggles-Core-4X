using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using queen.data;
using queen.error;

public partial class CharStatManager : Node
{

    [Signal] public delegate void OnStatChangeEventHandler(string statName, float value);
    [Signal] public delegate void OnStatModAddedEventHandler(string statName, float value, int modifier);
    [Signal] public delegate void OnStatModRemovedEventHandler(string statName, float value, int modifier);

    [Export] private PackedScene SceneFloatStat;
    [Export] private PackedScene SceneStatMod;

    struct DynStat
    {
        public float Value;
        public float MaxValue;
        public float RegenRate;
        public string ReferenceMaxVal;
        public string ReferenceRegenRate;
    }

    private Dictionary<string, float> Stats = new();
    private Dictionary<string, DynStat> DynamicStats = new();

    public override void _Ready()
    {
        RebuildStatDict();
    }

    public void AddStat(string name, float value, CharStatFloat.Modifier modifier)
    {
        if (SceneFloatStat.Instantiate() is not CharStatFloat node) return;
        node.Name = name;
        node.StoredValue = value;
        node.StatMod = modifier;
        AddChild(node);
        RebuildStatDict();
    }

    public void CreateDynamicStat(string name, float initialValue, float max_value, float regen_rate, string refMax = "", string refRegen = "")
    {
        var dyn = new DynStat()
        {
            Value = initialValue,
            MaxValue = max_value,
            RegenRate = regen_rate,
            ReferenceMaxVal = refMax,
            ReferenceRegenRate = refRegen
        };
        DynamicStats[name] = dyn;
    }

    public void AddStatMod(string targetStat, float value, CharStatFloat.Modifier mod, float duration)
    {
        if (!Stats.ContainsKey(targetStat)) return;
        if (SceneStatMod.Instantiate() is not CharStatFloatMod node) return;
        node.StoredValue = value;
        node.StatMod = mod;
        node.Duration = duration;

        foreach (var c in GetChildren())
        {
            if (c.Name == targetStat)
            {
                c.AddChild(node);
                break;
            }
        }
        node.TreeExiting += () => { EmitSignal(nameof(OnStatModAdded), targetStat, value, (int)mod); };
        node.TreeExiting += DelayedRebuild;
        node.TreeExiting += () => { EmitSignal(nameof(OnStatModRemoved), targetStat, value, (int)mod); };
        RebuildStatDict();
        // SceneStatMod
    }

    public void ModifyStaticStat(string target, float delta_value)
    {
        var n_val = GetStat(target) + delta_value;
        SetStaticStat(target, n_val);
    }
    public void SetStaticStat(string target, float n_value)
    {
        var stat_node = GetNode<CharStatFloat>(target);
        if (stat_node is null) return;
        stat_node.StoredValue = n_value;
        RebuildStatDict();
        EmitSignal(nameof(OnStatChange), target, GetStat(target));
    }


    public override void _Process(double delta)
    {
        var fd = (float)delta;
        foreach (var pair in DynamicStats)
        {
            var d = pair.Value;
            if (HasStat(d.ReferenceMaxVal)) d.MaxValue = GetStat(d.ReferenceMaxVal);
            if (HasStat(d.ReferenceRegenRate)) d.RegenRate = GetStat(d.ReferenceRegenRate);
            d.Value += d.RegenRate * fd;
            if (d.Value > d.MaxValue) d.Value = d.MaxValue;
            DynamicStats[pair.Key] = d;
        }
    }

    private async void DelayedRebuild()
    {
        // When signals are emitted, tree is invalid for rebuild. 5ms should fix?
        await Task.Delay(5);
        RebuildStatDict();
    }

    private void RebuildStatDict()
    {
        Stats.Clear();
        foreach (var n in GetChildren())
        {
            if (n is not CharStatFloat csf) continue;
            Stats[csf.Name] = csf.GetNetValue();
        }
        // DebugPrintStats();
    }

    public bool HasStat(string name)
    {
        return Stats.ContainsKey(name) || DynamicStats.ContainsKey(name);
    }

    public float GetStat(string name)
    {
        return DynamicStats.TryGetValue(name, out DynStat d_val) ? d_val.Value :
        (Stats.TryGetValue(name, out float val) ? val : 0.0f);
    }

    public bool ConsumeDynStat(string statName, float amount)
    {
        if (!DynamicStats.ContainsKey(statName)) return false;
        if (DynamicStats[statName].Value < amount) return false;
        var dyn = DynamicStats[statName];
        dyn.Value -= amount;
        DynamicStats[statName] = dyn;
        return true;
    }

    public bool HasStatMinimum(string statName, float minAmount)
    {
        if (!HasStat(statName)) return false;
        return GetStat(statName) >= minAmount;
    }

    public void DebugPrintStats()
    {
        Print.Debug($"Stats Manager: {Name}");
        Print.Debug("[Dynamic Stats]");
        foreach (var pair in DynamicStats)
        {
            Print.Debug($"(){pair.Key} = {pair.Value.Value}");
        }
        Print.Debug("[Stats]");
        foreach (var pair in Stats)
        {
            Print.Debug($"(){pair.Key} = {pair.Value}");
        }
        Print.Debug($"End Stats Manager: {Name}");
    }

    public void SaveToData(ref SaveDataBuilder build)
    {
        foreach (var stat in Stats)
        {
            build.PutFloat($"StatS_{stat.Key}", stat.Value);
        }
        foreach (var stat in DynamicStats)
        {
            build.PutFloat($"StatS_{stat.Key}_Value", stat.Value.Value);
            build.PutFloat($"StatS_{stat.Key}_Max", stat.Value.MaxValue);
            build.PutFloat($"StatS_{stat.Key}_Regen", stat.Value.RegenRate);
        }
    }

    public void LoadFromData(SaveDataBuilder build)
    {
        foreach (var stat in Stats)
        {
            if (!build.GetFloat($"StatS_{stat.Key}", out float val)) continue;
            if (GetNode(stat.Key) is CharStatFloat csf)
            {
                csf.StoredValue = val;
                EmitSignal(nameof(OnStatChange), stat.Key, val);
            }
        }
        foreach (var stat in DynamicStats)
        {
            if (!build.GetFloat($"StatS_{stat.Key}_Value", out float val)) continue;
            if (!build.GetFloat($"StatS_{stat.Key}_Max", out float max)) continue;
            if (!build.GetFloat($"StatS_{stat.Key}_Regen", out float regen)) continue;
            DynamicStats[stat.Key] = new DynStat
            {
                Value = val,
                MaxValue = max,
                RegenRate = regen,
                ReferenceMaxVal = stat.Value.ReferenceMaxVal,
                ReferenceRegenRate = stat.Value.ReferenceRegenRate
            };
        }
        RebuildStatDict();
    }
}
