using Godot;
using queen.events;
using System;

public partial class HUDRequests : Node
{

    public void RequestSubtitle(string text) => Events.GUI.TriggerRequestSubtitle(text);
    public void RequestAlert(string text) => Events.GUI.TriggerRequestAlert(text);

    public void ClearAll()
    {
        RequestSubtitle("");
        RequestAlert("");
    }

}
