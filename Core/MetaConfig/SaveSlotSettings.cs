namespace Squiggles.Core.Meta;

using Godot;
using Squiggles.Core.Error;

/// <summary>
/// Save Slot Settings is a resource used by the <see cref="SquigglesCoreConfigFile"/> to load properties of the save slot handling. See individual properties for more details.
/// </summary>
[GlobalClass]
public partial class SaveSlotSettings : Resource {

  /// <summary>
  /// An enum for determining how save slots should be handled.
  /// NO_SAVE_DATA = game does not save any data. All progress made must be made in a single session, ideal for simpler games that would rather ignore the save slot functionality.
  /// SINGLE_SAVE_SLOT = all progression is tied to a single save slot. Hitting the "Play" button will create a new slot if one is unavailable, or continue the existing slot if desired.
  /// MULTI_SLOT_SAVE_DATA = many save slots allowed. Full menu for selecting between different existing slots or creating a new one
  /// </summary>
  public enum SaveSlotOptions {
    NO_SAVE_DATA, SINGLE_SAVE_SLOT, MULTI_SLOT_SAVE_DATA
  }

  /// <summary>
  /// The property that is exposed for the project's selected save slot approach.
  /// </summary>
  [Export] public SaveSlotOptions SlotOptions = SaveSlotOptions.MULTI_SLOT_SAVE_DATA;

  [Export] private SlotInfoProviderResource _slotInfoProvider;
  /// <summary>
  /// Enable or disable the ability to quickly reload the last save in the options menu. While technically possible to save scum with this feature disabled, it is more effort than having a button in the pause menu.
  /// </summary>
  [Export] public bool AllowPlayersToReloadLastSave = true;

  /// <summary>
  /// Public attribute that will return the currently loaded (specified) ISaveSlotInformationProvider. If none are specified, the default provider will be loaded with a warning.
  /// </summary>
  public ISaveSlotInformationProvider SlotInfoProvider {
    get {
      // use fallback provider because either no custom provider is specified, or we failed to find it based on the class name
      if (_slotInfoProvider is null) {
        Print.Warn("`SaveSlotSettings.SlotInfoProvider` is using fallback provider! If this is intentional, please enter `Squiggles.Core.Meta.DefaultSlotInfoProvider` as the slot info provider class name in your configuration settings!", this);
        _slotInfoProvider = new SlotInfoProviderResource();
      }
      return _slotInfoProvider;
    }
  }
}
