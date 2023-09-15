namespace Squiggles.Core.AI;

/// <summary>
/// An interface for the function needed by the <see cref="UtilitySelector"/>
/// </summary>
public interface IUtilitySelectionComponent {

  /// <summary>
  /// Returns the weight associated with this component. Weights should be between 0 and 1. This helps with normalization.
  /// </summary>
  /// <param name="actor">A potentially null object which is passed from the AI using the utility selector to let these components access and react to the changing properties and values of the environment</param>
  /// <returns>A float weight between 0 and 1 where 1 is the highest priority and 0 is the lowest priority.</returns>
#nullable enable
  float GetWeight(object? actor);
}
