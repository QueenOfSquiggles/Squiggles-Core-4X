namespace Squiggles.Core.Interaction;

/// <summary>
/// An interface to show that a particular object is capable of being selected (which implies a graphical feedback when selected). Typically used in conjunction with <see cref="IInteractable"/>
/// </summary>
public interface ISelectable {
  /// <summary>
  /// Called when this object is selected
  /// </summary>
  void OnSelect();

  /// <summary>
  /// Called when this object is deselected
  /// </summary>
  void OnDeselect();
}
