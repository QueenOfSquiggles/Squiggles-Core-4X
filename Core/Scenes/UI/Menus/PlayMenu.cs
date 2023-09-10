using System;
using Godot;
using queen.data;
using queen.error;
using queen.events;
using queen.extension;

public partial class PlayMenu : PanelContainer
{
    [Export(PropertyHint.File, "*.tscn")] private string LevelScene;
    [Export] private NodePath PathSaveSlotsRoot;

    private Control _SlotsRoot;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.GetSafe(PathSaveSlotsRoot, out _SlotsRoot);
        LoadSlots();
    }

    private void LoadSlots()
    {
        var slots = Data.GetKnownSaveSlots();
        _SlotsRoot.RemoveAllChildren();
        foreach (var s in slots)
        {
            if (!s.StartsWith("Y")) continue;
            var btn = new Button();
            _SlotsRoot.AddChild(btn);
            var date = Data.ParseSaveSlotName(s);
            btn.Text = $"{date.ToShortDateString()} - {date.ToShortTimeString()}";
            btn.Pressed += () => LoadSlot(s);
        }
    }

    private void OnBtnContinue()
    {
        Events.Data.TriggerSerializeAll();
        Data.LoadMostRecentSaveSlot();
        if (GetTree().CurrentScene.SceneFilePath != LevelScene) Scenes.LoadSceneAsync(LevelScene);
        Events.Data.TriggerReload();
    }
    private void OnBtnNewGame()
    {
        LoadSlot(Data.CreateSaveSlotName()); // makes a new save slot for current time
    }

    private void LoadSlot(string slot_name)
    {
        Events.Data.TriggerSerializeAll();
        Data.SetSaveSlot(slot_name);
        if (GetTree().CurrentScene.SceneFilePath != LevelScene) Scenes.LoadSceneAsync(LevelScene);
        Events.Data.TriggerReload();
    }
}
