namespace Squiggles.Core.Sequencing;

using Godot;


[GlobalClass]
public partial class SequenceActionBase : Resource {

  [Export] public bool OneShot = true;

  public virtual void PerformAction(Node owner) { }

}
