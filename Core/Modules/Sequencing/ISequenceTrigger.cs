namespace Squiggles.Core.Sequencing;

/// <summary>
/// A small interface to allow sequence triggers to communicate whether they are currently "triggered" or not. Mainly for the purpose of basic logical operations using each individual trigger as a bool. Custom triggers don't have to implement this, but they will not be recognized as triggers by typical composition triggers.
/// </summary>
public interface ISequenceTrigger {

  /// <returns>true if the trigger is currently active (and should be executing stored actions)</returns>
  public abstract bool GetValidationState();
}
