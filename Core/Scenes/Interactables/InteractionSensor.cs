using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using interaction;
using Modules.Interaction;
using queen.error;
using queen.extension;

public partial class InteractionSensor : Area3D
{
    [Signal] public delegate void OnCurrentInteractionChangeEventHandler();

    [Export] private NodePath PathDerivePositionFrom = "..";
    [Export] private bool AutoSelectObjects = false;

    private Node3D DerivedPosition;


    public Node3D? CurrentInteraction { get; private set; } = null;

    public override void _Ready()
    {
        this.GetSafe(PathDerivePositionFrom, out DerivedPosition);
    }

    private void OnAreaEnter(Area3D _) => RefreshCurrent();
    private void OnAreaExit(Area3D _) => RefreshCurrent();

    private void OnBodyEnter(Node3D _) => RefreshCurrent();
    private void OnBodyExit(Node3D _) => RefreshCurrent();

    private void RefreshCurrent()
    {
        // any time something valid enters/exits we check all overlapping. Using Linq to filter out false positives
        var options = new List<Node3D>();
        options.AddRange(GetOverlappingBodies());
        options.AddRange(GetOverlappingAreas());
        options = options.FindAll((Node3D n) => n is IInteractable inter && inter.IsActive()).ToList();

        if (options.Count <= 0)
        {
            if (CurrentInteraction == null) return;
            if (AutoSelectObjects && CurrentInteraction is ISelectable sel) sel.OnDeselect();
            CurrentInteraction = null;
            EmitSignal(nameof(OnCurrentInteractionChange));
        }
        else
        {
            var n_current = options[0];
            var dist = float.MaxValue;
            foreach (var n in options)
            {
                var d = (DerivedPosition.GlobalPosition - n.GlobalPosition).LengthSquared();
                if (d > dist) continue;
                dist = d;
                n_current = n;
            }
            if (CurrentInteraction == n_current) return;
            if (AutoSelectObjects && CurrentInteraction is ISelectable sel1) sel1.OnDeselect();
            CurrentInteraction = n_current;
            if (AutoSelectObjects && CurrentInteraction is ISelectable sel2) sel2.OnSelect();
            EmitSignal(nameof(OnCurrentInteractionChange));
        }

    }
}
