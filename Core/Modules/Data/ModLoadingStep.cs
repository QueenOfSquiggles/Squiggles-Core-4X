namespace Squiggles.Core.Scenes.Modification;
using Godot;
using Squiggles.Core.Data;
using Squiggles.Core.Error;
using Squiggles.Core.Events;

/// <summary>
/// The node which triggers the <see cref="ModRegistry"/> to load mods. Only executes mod loading outside of a "demo" context to prevent mods that unlock the remainder of the game. Not a foolproff solution, but it at least makes it harder to spoof.
/// </summary>
[GlobalClass]
public partial class ModLoadingStep : Node {

  /// <summary>
  /// In a debug context, do not load any modification data. Ideally the modifications someone is developing are already embedded into the project files. And loading will only happen for exported game versions
  /// </summary>
  public override void _Ready() {
    if (OS.HasFeature("demo")) {
      Print.Warn("Mod Loading is not supported in demo builds. If you would like to use mods, please acquire the full release");
      return;
    }
    // Mod loading is not supported for demo builds.
    // This is explicitly intended to prevent modifications that would gain access to hidden content during demo play.
    // Full releases, usually paid versions, allow full unrestriced access to mod loading because once you've bought the game IDGAF what you do with it. Make an anime titty asset swap for all I care. I'm not responsible for what you choose to add to the game.
    // I may need to make a small tool for mod loading however

    ModRegistry.LoadModsRecursively();
    ModRegistry.OnRegisterMods();
    EventBus.Data.TriggerOnModsLoaded();
  }

  public override void _ExitTree() => ModRegistry.OnUnRegisterMods();

}
