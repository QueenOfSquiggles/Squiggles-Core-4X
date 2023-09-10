using System;
using System.Threading.Tasks;
using Godot;
using queen.error;
using queen.events;
using queen.extension;

public partial class OptionsMenu : Control
{
    [Export(PropertyHint.File, "*.tscn")] private string main_menu_path;
    [Export] private NodePath path_sliding_scene_root = "SlidingRoot";

    [ExportGroup("Panel Scene References")]
    [Export(PropertyHint.File, "*.tscn")] private string path_panel_gameplay;
    [Export(PropertyHint.File, "*.tscn")] private string path_panel_graphics;
    [Export(PropertyHint.File, "*.tscn")] private string path_panel_access;
    [Export(PropertyHint.File, "*.tscn")] private string path_panel_controls;
    [Export(PropertyHint.File, "*.tscn")] private string path_panel_audio;


    private Node CurrentPopup = null;
    private bool IsBusy = false;
    private Control SlidingSceneRoot;

    public override void _Ready()
    {
        this.GetSafe(path_sliding_scene_root, out SlidingSceneRoot);
    }

    private void OnBtnGameplay() { } // => DoPanelThing(path_panel_gameplay);
    private void OnBtnGraphics() => DoPanelThing(path_panel_graphics);
    private void OnBtnAccess() => DoPanelThing(path_panel_access);
    private void OnBtnAudio() => DoPanelThing(path_panel_audio);
    private void OnBtnControls() => DoPanelThing(path_panel_controls);

    private async void DoPanelThing(string file_path)
    {
        if (IsBusy) return;
        Events.Data.TriggerSerializeAll();
        // Print.Debug($"OptionsMenu: attempting sliding in {file_path}");
        IsBusy = true;
        var result = await ClearOldSlidingScene(file_path);
        if (result) CreateNewSlidingScene(file_path);
        IsBusy = false;
    }

    private async Task<bool> ClearOldSlidingScene(string scene_file)
    {
        if (CurrentPopup is null || !IsInstanceValid(CurrentPopup)) return true;
        if (CurrentPopup.SceneFilePath == scene_file) return false;
        var sliding_comp = CurrentPopup.GetComponent<SlidingPanelComponent>();
        if (sliding_comp is not null)
        {
            sliding_comp.RemoveScene();
            await ToSignal(CurrentPopup, "tree_exited");
        }
        return true;
    }

    private void CreateNewSlidingScene(string scene_file)
    {
        var packed = GD.Load<PackedScene>(scene_file);
        var scene = packed?.Instantiate<Control>();
        if (scene is null) return;
        scene.GlobalPosition = new Vector2(Size.X, 0);
        SlidingSceneRoot.AddChild(scene);
        CurrentPopup = scene;
    }

    public override void _ExitTree()
    {
        Events.Data.TriggerSerializeAll();
    }

}
