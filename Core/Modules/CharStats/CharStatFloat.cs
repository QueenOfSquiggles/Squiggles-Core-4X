namespace Squiggles.Core.CharStats;
using Godot;

/// <summary>
/// A character stat (float). Designed to composite with child nodes.
/// </summary>
[GlobalClass]
public partial class CharStatFloat : Node {

  /// <summary>
  /// The modifier to apply with child values
  /// </summary>
  public enum Modifier {
    /// <summary>
    /// Replaces the calculations of the child values with its own
    /// </summary>
    REPLACE,

    /// <summary>
    /// Adds its value to that of its child values
    /// </summary>
    ADD,

    /// <summary>
    /// Scales the child values with its own values. Useful for applying a scalar buff temporarily.
    /// </summary>
    MULTIPLY
  };

  /// <summary>
  /// This instance's modifier
  /// </summary>
  [Export] public Modifier StatMod = Modifier.REPLACE;
  /// <summary>
  /// The stored value of this stat
  /// </summary>
  [Export] public float StoredValue = 0.0f;

  /// <summary>
  /// Calculates the "net" value of itself and all child CharStats
  /// </summary>
  /// <returns>The calculated net of all child stats (using StatMod to determine how this is done)</returns>
  public float GetNetValue() {
    var val = StoredValue;
    foreach (var n in GetChildren()) {
      if (n is not CharStatFloat csf) {
        continue;
      }

      val += csf.ApplyValue(val);
    }
    return val;
  }

  /// <summary>
  /// Calculates how a particular in-value is treated considering this stats's StatMod value.
  /// </summary>
  /// <param name="inVal">a float value of a given stat</param>
  /// <returns>the calculated result</returns>
  public float ApplyValue(float inVal) {
    switch (StatMod) {
      case Modifier.REPLACE:
        return StoredValue;
      case Modifier.ADD:
        return inVal + StoredValue;
      case Modifier.MULTIPLY:
        return inVal * StoredValue;
      default:
        break;
    }
    return inVal;
  }

}
