using System;
using Godot;
using queen.extension;
using static Godot.Tween;

public partial class ItemSlotDisplay : PanelContainer
{
    [ExportGroup("Selected Settings", "Select")]
    [Export] private float SelectOffset = -16.0f;
    [Export] private float UnselectedOffset = 32.0f;
    [Export] private float SelectScale = 1.25f;

    [ExportGroup("Node Paths", "Path")]
    [Export] private NodePath PathIcon;
    [Export] private NodePath PathLabel;

    private Label _Label;
    private TextureRect _Icon;
    private string _Item;
    private int _Qty = 0;
    private Tween _ActiveTween;
    private float _ReturnScale = 1.0f;
    private float _SelectY = 0.0f;
    private float _UnselectY = 0.0f;

    public override void _Ready()
    {
        this.GetSafe(PathIcon, out _Icon);
        this.GetSafe(PathLabel, out _Label);
        PivotOffset = new Vector2(Size.X / 2.0f, Size.Y);
        _SelectY = Position.Y + SelectOffset;
        _UnselectY = Position.Y + UnselectedOffset;
        Position += new Vector2(0, Size.Y);
        _Label.Text = "";
        UpdateItem("", 0);

        _ActiveTween = GetDefaultTween();
        _ActiveTween.TweenInterval(0.5f);
        _ActiveTween.TweenProperty(this, "position:y", _UnselectY, 0.3f);
    }

    public void UpdateItem(string item, int qty)
    {
        var pos = Position;
        var scale = Scale;
        if (item == "")
        {
            _Item = "";
            _Qty = 0;
            _Icon.Texture = null;
            _Label.Text = "";
        }
        if (item != _Item)
        {
            _Item = item;
            var data = RegistrationManager.GetResource<WorldEntity>(_Item);
            _Icon.Texture = data?.InventoryIcon as Texture2D;
        }

        if (_Qty != qty)
        {
            _Qty = qty;
            _Label.Text = (_Qty <= 1) ? "" : $"{_Qty}";
        }
        Position = pos;
        Scale = scale;
    }

    public void OnSelect()
    {
        _ActiveTween?.Kill();
        _ActiveTween = GetDefaultTween();
        _ActiveTween.TweenProperty(this, "position:y", _SelectY, 0.3f);
        _ActiveTween.Parallel().TweenProperty(this, "scale", Vector2.One * SelectScale, 0.3f);
        _ActiveTween.Parallel().TweenProperty(this, "z_index", 3, 0.3f);
    }

    public void OnDeselect()
    {
        _ActiveTween?.Kill();
        _ActiveTween = GetDefaultTween();
        _ActiveTween.TweenProperty(this, "position:y", _UnselectY, 0.3f);
        _ActiveTween.Parallel().TweenProperty(this, "scale", Vector2.One * _ReturnScale, 0.3f);
        _ActiveTween.Parallel().TweenProperty(this, "z_index", 0, 0.3f);
    }

    private Tween GetDefaultTween()
    {
        return CreateTween().SetTrans(TransitionType.Cubic).SetEase(EaseType.InOut);
    }


}
