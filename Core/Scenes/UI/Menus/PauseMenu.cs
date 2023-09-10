using System;
using System.Threading.Tasks;
using Godot;
using queen.events;
using queen.extension;

public partial class PauseMenu : Control
{

    [Export(PropertyHint.File, "*.tscn")] private string main_menu_file;
    [Export(PropertyHint.File, "*.tscn")] private string options_menu_file;
    [Export] private NodePath PathMenuPanel;

    private Control CurrentPopup = null;
    private Control MenuPanel;

    public override void _Ready()
    {
        this.GetSafe(PathMenuPanel, out MenuPanel);
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
        Scenes.LoadSceneAsync(main_menu_file);
    }

    private void OnBtnOptions()
    {
        if (CurrentPopup is null || !IsInstanceValid(CurrentPopup))
        {
            CreateNewSlidingScene(options_menu_file);
        }
        else
        {
            CurrentPopup?.GetComponent<SlidingPanelComponent>()?.RemoveScene();
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
        scene.GlobalPosition = new Vector2(MenuPanel.Size.X, 0);
        AddChild(scene);
        CurrentPopup = scene;
    }
}
