using System;
using System.Threading.Tasks;
using Godot;
using queen.events;
using queen.extension;

public partial class PauseMenu : Control
{

    [Export(PropertyHint.File, "*.tscn")] private string _MainMenuFile = "";
    [Export(PropertyHint.File, "*.tscn")] private string _OptionsMenuFile = "";

    [Export] private Control _MenuPanel;
    private Control _CurrentPopup = null;

    public override void _Ready()
    {
        GetTree().Paused = true;
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }

    public override void _UnhandledInput(InputEvent e)
    {
        if (e.IsActionPressed("ui_cancel"))
        {

            ReturnToPlay();
            this.HandleInput();
        }
    }

    private async void ReturnToPlay()
    {
        Events.Data.TriggerSerializeAll();
        await Task.Delay(10);
        Input.MouseMode = Input.MouseModeEnum.Captured;
        GetTree().Paused = false;
        QueueFree();
    }

    private async void ExitToMainMenu()
    {
        Events.Data.TriggerSerializeAll();
        await Task.Delay(10);
        GetTree().Paused = false;
        Scenes.LoadSceneAsync(_MainMenuFile);
    }

    private void OnBtnOptions()
    {
        if (_CurrentPopup is null || !IsInstanceValid(_CurrentPopup))
        {
            CreateNewSlidingScene(_OptionsMenuFile);
        }
        else
        {
            _CurrentPopup?.GetComponent<SlidingPanelComponent>()?.RemoveScene();
        }

    }
    private void OnBtnSave()
    {
        Events.Data.TriggerSerializeAll();
    }

    private void OnBtnReloadLastSave()
    {
        Events.Data.TriggerReload();
    }

    private void CreateNewSlidingScene(string scene_file)
    {
        var packed = GD.Load<PackedScene>(scene_file);
        var scene = packed?.Instantiate<Control>();
        if (scene is null) return;
        scene.GlobalPosition = new Vector2(_MenuPanel?.Size.X ?? 0, 0);
        AddChild(scene);
        _CurrentPopup = scene;
    }
}
