namespace Squiggles.Core.AI;

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

/// <summary>
/// UtilitySelector is an AI sub-system that is used for selecting the course of action with the most utility at the current moment. This system is designed to be used modularly so each possible action will have a string key.
/// </summary>
public partial class UtilitySelector : Node {

  private readonly Dictionary<string, IUtilitySelectionComponent> _actions = new();
  private readonly Dictionary<string, float> _preferences = new();
  /// <summary>
  /// The amount by which two actions' weights are considered equivalent. If actions are equivalent, the choice is made through randomization
  /// </summary>
  public float UtilityFuzziness { get; set; } = 0.1f;
  private Random _random = new();

  /// <summary>
  /// A property to set the seed of the interal RNG.
  /// </summary>
  public int Seed {
    get => 0;
    set => _random = new Random(value);
  }

  /// <summary>
  /// Uses utility calculating components to determine the optimal action.
  /// </summary>
  /// <param name="actor">An object reference for the actor to pass to the action <see cref="IUtilitySelectionComponent"/></param>
  /// <param name="doNoActionIfNegative">True if this selector should return no action if all actions have negative weight. Default is false.</param>
  /// <returns>A string key for the selected action. Returns an empty string if no proper action could be found</returns>
  public string GetBestAction(object actor, bool doNoActionIfNegative = false) {
    var weights = new List<KeyValuePair<string, float>>();
    foreach (var action in _actions.Keys) {
      if (_actions[action] is null) {
        continue;
      }

      var pair = new KeyValuePair<string, float>(action, _actions[action].GetWeight(actor) + GetPreference(action));
      weights.Add(pair);
    }

    var sorted_weights = weights.OrderByDescending((a) => a.Value).ToList();
    var top = sorted_weights[0].Value;
    if (top < 0f && doNoActionIfNegative) {
      return "";
    }

    var available = sorted_weights.Where((x)
        => Mathf.Abs(x.Value - top) < UtilityFuzziness).ToList();
    if (available.Count <= 0) {
      return ""; // no action selected
    }

    if (available.Count == 1) {
      return available[0].Key; // only one top action
    }

    return available[_random.Next(available.Count)].Key; // select random from top actions based on fuzziness
  }


  /// <summary>
  ///   Registers an action component with this utility selector.
  /// </summary>
  /// <param name="action">the action name</param>
  /// <param name="selection_component">the component reference</param>
  public void AddActionComponent(string action, IUtilitySelectionComponent selection_component) => _actions[action] = selection_component;

  /// <summary>
  ///  Removes an action by name from the utility selector.
  /// </summary>
  /// <param name="action">the name of the action</param>
  public void RemoveAction(string action) {
    if (!_actions.ContainsKey(action)) {
      return;
    }

    _actions.Remove(action);
  }

  /// <summary>
  /// Adds a preference weighting to a particular action. This can be used to modify the weight of a given action on a per-instance basis. Helpful for large groups of near identical behaviours with "personality" desired.
  /// </summary>
  /// <param name="action">the action to add a preference for</param>
  /// <param name="preference">the weight to add to the utility weight</param>
  public void SetPreference(string action, float preference) {
    if (_preferences.ContainsKey(action)) {
      _preferences[action] = preference;
    }
    else {
      _preferences.Add(action, preference);
    }
  }

  /// <summary>
  /// Adds the value to any existing preference (default of which is zero). This can be used to modify preference over time based on some stimulus
  /// </summary>
  /// <param name="action">the name of the action</param>
  /// <param name="preference">the amount to add to the preference (can be negative!)</param>
  public void AddPreference(string action, float preference) {
    if (_preferences.ContainsKey(action)) {
      _preferences[action] += preference;
    }
    else {
      _preferences.Add(action, preference);
    }
  }

  /// <summary>
  /// Gets the current preference value for a given action.
  /// </summary>
  /// <param name="action">the name of the action</param>
  /// <returns>the preference weight currently stored (or zero if none are stored)</returns>
  public float GetPreference(string action) => _preferences.ContainsKey(action) ? _preferences[action] : 0.0f;

}
