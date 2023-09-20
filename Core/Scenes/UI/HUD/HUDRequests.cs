namespace Squiggles.Core.Scenes.UI;

using Godot;
using Squiggles.Core.Events;

public partial class HUDRequests : Node {

  public void RequestSubtitle(string text) => EventBus.GUI.TriggerRequestSubtitle(text);
  public void RequestAlert(string text) => EventBus.GUI.TriggerRequestAlert(text);

  public void ClearAll() {
    RequestSubtitle("");
    RequestAlert("");
  }

}
