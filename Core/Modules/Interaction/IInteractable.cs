namespace Squiggles.Core.Interaction;

/// <summary>
/// An interface for allowing an object to be interacted with. Requires some processing in the controller otherwise, but works quite well.
/// </summary>
public interface IInteractable {

  /// <summary>
  /// Called to perform the interaction action.
  /// </summary>
  /// <returns>true if the action was a success.</returns>
  public bool Interact();

  /// <summary>
  /// Determines if the interactable object is currently able to be interacted with.
  /// </summary>
  /// <returns>true if currently able to interact</returns>
  public bool GetIsActive();

  /// <summary>
  /// Returns the name of the object (through this method to allow dynamic changes). Use a translation key to support internationalization
  /// </summary>
  /// <returns>a string of the name which this object is.</returns>
  public string GetActiveName();

}
