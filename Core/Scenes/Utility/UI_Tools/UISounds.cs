namespace Squiggles.Core.Scenes.Utility;

using System.Threading.Tasks;
using Godot;
using Squiggles.Core.Attributes;
using Squiggles.Core.Data;
using Squiggles.Core.Error;
using Squiggles.Core.Extension;

/// <summary>
/// A generic tool for adding some juice to UI elements.
/// </summary>
/// <remarks>
/// There is functionality for popping up the ui element, sfx for mouse over and click. But there is also some partial feature development for TTS (Text To Speech) for the text contained on the parent node. More testing and development are required to get that feature working but the bones are there. In fact it techincally works, so long as you only want to select one GUI element every 5-10 seconds
/// </remarks>
[MarkForRefactor("rename/specify", "this class needs to be renamed in order to better specify its purpose")]
public partial class UISounds : Node {
  /// <summary>
  /// The scale factor at which the element "pops" on mouse over
  /// </summary>
  [Export] private float _popUIScale = 1.1f;

  /// <summary>
  /// The audio player for the selection sfx
  /// </summary>
  [Export] private AudioStreamPlayer _sfxSelect;
  /// <summary>
  /// The audio player for the click sfx
  /// </summary>
  [Export] private AudioStreamPlayer _sfxClick;
  private static string _voiceID = "";
  private Tween _lastTween;

  public override void _Ready() {
    var parent = GetParent<Control>();
    if (_voiceID == "") {
      var isEnabled = ProjectSettings.GetSetting("audio/general/text_to_speech", false).AsBool();
      if (isEnabled) {
        _voiceID = DisplayServer.TtsGetVoicesForLanguage(OS.GetLocaleLanguage())[0];
      }
    }

    if (parent is not null) {
      ConnectSignalsDelayed(parent, 50);
      parent.SetAnchorsPreset(Control.LayoutPreset.Center); // anchor center for uniform scaling
    }
    else {
      // we know this is false, push the assertion failure
      Debugging.Assert(false, "UISounds node must be child of a Control node!");
    }
  }

  private async void ConnectSignalsDelayed(Control parent, int delay) {
    await Task.Delay(delay);
    if (!IsInstanceValid(parent)) {
      return;
    }

    if (parent is TabContainer tabs) {
      tabs.TabButtonPressed += (index) => OnClick();
      return; // don't do select on container. That would get annoying
    }

    parent.FocusEntered += OnSelect;
    parent.MouseEntered += OnSelect;
    switch (parent) {
      case Button btn:
        btn.Pressed += OnClick;
        break;
      case HSlider slider:
        slider.DragStarted += OnClick;
        break;
      case LinkButton link:
        link.Pressed += OnClick;
        break;
      default:
        Print.Warn($"Unsupported parent type: {parent.GetType()}");
        break;
    }

  }

  private void OnSelect() {
    if (Access.ReadVisibleTextAloud && GetParent().Get("text").VariantType != Variant.Type.Nil) {
      var text_string = GetParent()?.Get("text").AsString();
      if (text_string is not null) {
        DoTTS(text_string);
      }
    }
    _sfxSelect?.Play();
    AnimatePop();

  }

  private void OnClick() => _sfxClick?.Play();

  private static async void DoTTS(string msg) {
    if (_voiceID == "") {
      return;
    }

    DisplayServer.TtsStop();
    await Task.Delay(300);
    DisplayServer.TtsSpeak(msg, _voiceID);
  }

  private void AnimatePop() {
    if (_lastTween is not null) {
      // prevents duplicate scaling where the tweens will stack to create an infinitely large Control
      _lastTween.CustomStep(5.0f); // Forces a run to the end of the tween
      _lastTween.Kill();
    }

    var parent = GetParent<Control>();
    if (parent is null) {
      return;
    }

    var start_size = parent.Size;
    var tween = GetTree().CreateTween().SetSC4XStyle();
    tween.SetTrans(Tween.TransitionType.Elastic);
    tween.TweenProperty(parent, "size", start_size * _popUIScale, 0.1);
    tween.TweenProperty(parent, "size", start_size, 0.05);
    _lastTween = tween;

  }

  private static void GetTTSVoiceID() {
    var voices = DisplayServer.TtsGetVoicesForLanguage(OS.GetLocaleLanguage());
    if (voices.Length <= 0) {
      Print.Warn($"Warning. No TTS voices installed for host system locale: {OS.GetLocaleLanguage()}. We will not be able to perform TTS lookups. If user wishes to use TTS on UI elements. Please install a TTS voice for your system. More information here: https://docs.godotengine.org/en/stable/tutorials/audio/text_to_speech.html#requirements-for-functionality");
    }
    // TODO finish implementing TTS properly. (Running on bg thread to avoid stuttering???)
  }

}
