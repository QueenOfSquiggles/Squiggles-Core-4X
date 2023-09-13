using Godot;
using System;

public partial class DemoEndScene : Control
{
    [Export(PropertyHint.File, "*.tscn")] private string _MainMenuScene = "";

    public override void _Ready()
    {
        // extra safe guard since we are likely to be coming from the play space, which has the mouse captured
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }

    public void ReturnMainMenu() => Scenes.LoadSceneAsync(_MainMenuScene);
}
