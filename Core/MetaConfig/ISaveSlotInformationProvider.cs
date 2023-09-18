namespace Squiggles.Core.Meta;

/// <summary>
/// An interface for providing meaningful information for a given save slot. All paths are considered absolute. Implement this to allow loading custom information for your game's save slots.
/// WARNING for the reflection implementation, a constructor that takes no arguments must be provided for your implementation
/// </summary>
public interface ISaveSlotInformationProvider {
  /// <summary>
  /// Gets the main (first) name for the save slot at the given path. Every save slot is a directory with a parseable date that is was created. This name will be first on the button to load said slot. Ideally loaded from some kind of metadata file
  /// </summary>
  /// <param name="absolutePath">The absolute path (starting with "C://" or "/home/" leading to the dirctory of the given save slot</param>
  /// <returns>A string that is the primary name for this save slot.</returns>
  public string GetSaveSlotName(string absolutePath);
  /// <summary>
  /// Gets the secondary title (or subtitle) of the save slot for the given path. This may return "" if no subtitles are desired. Ideally this should provide some extra metadata such as what level the player has reached, or possibly some key items they have acquired at this point.
  /// </summary>
  /// <param name="absolutePath">The absolute path (starting with "C://" or "/home/" leading to the dirctory of the given save slot</param>
  /// <returns>A string that is the secondary name for this save slot.</returns>
  public string GetSaveSlotSubtitle(string absolutePath);
}
