namespace menus;

using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using queen.data;
using queen.error;
using queen.events;
using queen.extension;

public partial class MainMenu : Control
{
	[Export(PropertyHint.File, "*.tscn")] private string PlayMenuScene;
	[Export(PropertyHint.File, "*.tscn")] private string Options;
	[Export(PropertyHint.File, "*.tscn")] private string CreditsScene;

	[ExportGroup("Node Paths")]
	[Export] private Control _ButtonsPanel;
	[Export] private TextureRect _GameLogo;

	private Node _CurrentPopup = null;

	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Visible;
		_GameLogo.Texture = ThisIsYourMainScene.Config.GameLogo;
	}

	private async void OnBtnPlay()
	{
		if (Data.HasSaveData())
		{
			if (_CurrentPopup is PlayMenu) return;
			await ClearOldSlidingScene();
			CreateNewSlidingScene(PlayMenuScene);
		}
		else
		{
			Data.SetSaveSlot(Data.CreateSaveSlotName());
			Events.Data.TriggerSerializeAll(); // guarantees any open options menus save their data
			Scenes.LoadSceneAsync(ThisIsYourMainScene.Config.PlayScene);
		}
	}

	private void OnBtnContinue()
	{
		Data.LoadMostRecentSaveSlot();
		Events.Data.TriggerSerializeAll(); // guarantees any open options menus save their data
		Scenes.LoadSceneAsync(ThisIsYourMainScene.Config.PlayScene);
	}

	private async void OnBtnOptions()
	{
		if (_CurrentPopup is OptionsMenu) return;
		await ClearOldSlidingScene();
		CreateNewSlidingScene(Options);
	}

	private async void OnBtnCredits()
	{
		if (_CurrentPopup is CreditsScene) return;
		await ClearOldSlidingScene();
		CreateNewSlidingScene(CreditsScene);
	}


	private async Task ClearOldSlidingScene()
	{
		if (_CurrentPopup is null || !IsInstanceValid(_CurrentPopup)) return;
		var sliding_comp = _CurrentPopup.GetComponent<SlidingPanelComponent>();
		if (sliding_comp is not null)
		{
			sliding_comp.RemoveScene();
			await ToSignal(_CurrentPopup, "tree_exited");
		}
	}

	private void CreateNewSlidingScene(string scene_file)
	{
		var packed = GD.Load<PackedScene>(scene_file);
		var scene = packed?.Instantiate<Control>();
		if (scene is null) return;
		scene.GlobalPosition = new Vector2(_ButtonsPanel.Size.X, 0);
		AddChild(scene);
		_CurrentPopup = scene;
	}

	private void OnBtnQuit()
	{
		Print.Debug("Quitting game");
		GetTree().Quit();
	}


}
