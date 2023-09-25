namespace Squiggles.Core.Modification;
/// <summary>
/// An interface for creating adapters that serve as an entry-point for C# modifications. Currently unsupported as the C# Assembly loading code is not active (due to various reasons)
/// <para/>
/// See <see cref="ModRegistry"/> for more information. (mostly in the source code)
/// </summary>
public interface IModificationAdapter {

  /// <summary>
  /// Called when the mod is added into the game
  /// </summary>
  void OnRegister();

  /// <summary>
  /// Called when the mod is removed from the game.
  /// </summary>
  void OnUnRegister();

}
