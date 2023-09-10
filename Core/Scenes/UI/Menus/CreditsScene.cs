using System;
using Godot;

public partial class CreditsScene : Control
{

	[Export(PropertyHint.File, "*.tscn")] private string main_menu_path;
	[Export] private Control _CreditsLinesRoot;
	[Export] private LabelSettings _LabelStyling;

	public override void _Ready()
	{
		var lines = ThisIsYourMainScene.Config.CreditsLines;
		foreach (var line in lines)
		{
			_CreditsLinesRoot.AddChild(new Label()
			{
				Text = line,
				LabelSettings = _LabelStyling
			});
		}
	}
	private void OnMenuButtonPressed() => Scenes.LoadSceneAsync(main_menu_path);
}
