namespace Squiggles.Core.Scenes.UI;

using Godot;
using Squiggles.Core.Events;

/// <summary>
/// A utility for driving subtitles and alerts through an animation player or signal driver approach.
/// </summary>
[GlobalClass]
public partial class HUDRequests : Node {

  /// <summary>
  /// Requests a subtitle using the <see cref="EventBus"/>
  /// <para/>
  /// (<seealso cref="EventBus.GUI.TriggerRequestSubtitle"/>)
  /// </summary>
  /// <param name="text">the text to display, or "" to hide the subtitles</param>
  public void RequestSubtitle(string text) => EventBus.GUI.TriggerRequestSubtitle(text);
  /// <summary>
  /// Requests an alert using the <see cref="EventBus"/>
  /// <para/>
  /// (<seealso cref="EventBus.GUI.TriggerRequestAlert"/>)
  /// </summary>
  /// <param name="text">the text to display, or "" to hide the alert</param>
  public void RequestAlert(string text) => EventBus.GUI.TriggerRequestAlert(text);

  /// <summary>
  /// Clears both the current subtitle and the current alert at the same time without any need for bindings
  /// </summary>
  public void ClearAll() {
    RequestSubtitle("");
    RequestAlert("");
  }

}
