using Godot;
using queen.extension;
using System;

public partial class TabContainerGamepadSupport : TabContainer
{
    public override void _UnhandledInput(InputEvent e)
    {
        if (e.IsActionPressed("ui_cycle_left"))
        {

            if (CurrentTab == 0) CurrentTab = GetChildCount() - 1;
            else CurrentTab--;
            this.HandleInput();
        }
        if (e.IsActionPressed("ui_cycle_right"))
        {
            if (CurrentTab == (GetChildCount()-1)) CurrentTab = 0;
            else CurrentTab++;
            this.HandleInput();
        }
    }
}
