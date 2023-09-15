namespace Squiggles.Core.Interaction;

public interface IInteractable {
  public bool Interact();

  public bool GetIsActive();

  public string GetActiveName();

}
