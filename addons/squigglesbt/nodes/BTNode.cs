using System.Collections.Generic;
using Godot;

public abstract class BTNode
{
    public string Label = "";
    public int MaxChildren { get; protected set; }
    public List<BTNode> Children { get; protected set; } = new();
    public Dictionary<string, Variant> Params = new();

    public BTNode() : this("BTNode", -1) { }
    public BTNode(string label, int max_children)
    {
        Label = label;
        MaxChildren = max_children;
        RegisterParams();
    }

    public const int SUCCESS = 0;
    public const int FAILURE = 1;
    public const int RUNNING = 2;
    public const int ERROR = 3;

    protected abstract void RegisterParams();

    public abstract int Tick(Node actor, Blackboard blackboard);

    public virtual void LoadDebuggingValues(Blackboard bb) { }

    public Dictionary<string, Variant> GetKnownParams() => Params;

    protected Variant GetParam(string key, Variant fallback, Blackboard bb)
    {

        if (Params.ContainsKey(key))
        {
            var par = Params[key];
            if (par.VariantType == Variant.Type.String && par.AsString().StartsWith("$"))
            {
                // get blackboard values from passed key
                var ps = par.AsString();
                var bbKey = ps.Substr(1, ps.Length - 1);
                if (bb.HasLocal(bbKey)) return bb.GetLocalOrDefault(bbKey, fallback);
                if (bb.HasGlobal(bbKey)) return bb.GetGlobalOrDefault(bbKey, fallback);
            }

            return par;
        }

        return fallback;

    }


}