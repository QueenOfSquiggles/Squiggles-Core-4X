namespace queen.ai.utility;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Godot;

/// <summary>
/// UtilitySelector is an AI sub-system that is used for selecting the course of action with the most utility at the current moment. This system is designed to be used modularly so each possible action will have a string key.
///
/// </summary>
public partial class UtilitySelector : Node
{

    private readonly Dictionary<string, IUtilitySelectionComponent> _Actions = new();
    private readonly Dictionary<string, float> _Preferences = new();
    /// <summary>
    /// The amount by which two actions' weights are considered equivalent. If actions are equivalent, the choice is made through randomization
    /// </summary>
    public float UtilityFuzziness { get; set; } = 0.1f;
    private Random _Random = new();

    /// <summary>
    /// A property to set the seed of the interal RNG.
    /// </summary>
    public int Seed
    {
        get { return 0; }
        set
        {
            _Random = new Random(value);
        }
    }

    /// <summary>
    /// Uses utility calculating components to determine the optimal action.
    /// </summary>
    /// <param name="actor">An object reference for the actor to pass to the action <seealso cref="UtlitySelectionComponent"/></param>
    /// <param name="doNoActionIfNegative">True if this selector should return no action if all actions have negative weight. Default is false.</param>
    /// <returns>A string key for the selected action. Returns an empty string if no proper action could be found</returns>
    public string GetBestAction(object actor, bool doNoActionIfNegative = false)
    {
        var weights = new List<KeyValuePair<string, float>>();
        foreach (string action in _Actions.Keys)
        {
            if (_Actions[action] is null) continue;
            var pair = new KeyValuePair<string, float>(action, _Actions[action].GetWeight(actor) + GetPreference(action));
            weights.Add(pair);
        }

        var sorted_weights = weights.OrderByDescending((a) => a.Value).ToList();
        float top = sorted_weights[0].Value;
        if (top < 0f && doNoActionIfNegative) return "";

        var available = sorted_weights.Where((x)
            => Mathf.Abs(x.Value - top) < UtilityFuzziness).ToList();
        if (available.Count <= 0) return ""; // no action selected
        if (available.Count == 1) return available[0].Key; // only one top action
        return available[_Random.Next(available.Count)].Key; // select random from top actions based on fuzziness
    }



    public void AddActionComponent(string action, IUtilitySelectionComponent selection_component)
    {
        _Actions[action] = selection_component;
    }

    public void RemoveAction(string action)
    {
        if (!_Actions.ContainsKey(action)) return;
        _Actions.Remove(action);
    }

    public void SetPreference(string action, float preference)
    {
        if (_Preferences.ContainsKey(action)) _Preferences[action] = preference;
        else _Preferences.Add(action, preference);
    }

    public void AddPreference(string action, float preference)
    {
        if (_Preferences.ContainsKey(action)) _Preferences[action] += preference;
        else _Preferences.Add(action, preference);
    }

    public float GetPreference(string action)
    {
        if (_Preferences.ContainsKey(action)) return _Preferences[action];
        return 0.0f;
    }

}
