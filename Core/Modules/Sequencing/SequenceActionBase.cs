namespace Squiggles.Core.Sequencing;

using Godot;


/// <summary>
/// The base resource for sequence actions. Implement a custom child class with `[GlobalCLass]` and it should be available in-editor
/// </summary>
[GlobalClass]
public partial class SequenceActionBase : Resource {

  /// <summary>
  /// Whether or note to remove this action from the queue upon completion.
  /// </summary>
  [Export] public bool OneShot = true;

  /// <summary>
  /// Called to perform the stored action. Override this method to create custom actions. See <see cref="SequenceActionSpawnable"/> for an example of implementation
  /// </summary>
  /// <param name="owner">the node which is targeted by the trigger (not necessarily the Godot node owner)</param>
  public virtual void PerformAction(Node owner) { }

}
