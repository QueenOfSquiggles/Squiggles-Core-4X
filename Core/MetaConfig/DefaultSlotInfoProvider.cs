namespace Squiggles.Core.Meta;

using System.Linq;

/// <summary>
/// A super rudimentary save slot information provider. It creates absolute bare minimum functionality so you don't have to make the feature a huge priority. However, it is recommended to create your own implementation eventually to yield more descriptive save slot information!
/// </summary>
public class DefaultSlotInfoProvider : ISaveSlotInformationProvider {

  public DefaultSlotInfoProvider() { }

  // Oh damn do I love inheritdoc! That's some good shit!!!
  /// <inheritdoc/>
  public string GetSaveSlotName(string absolutePath) => absolutePath?.Split("/")?.Last() ?? "!!Failed to find directory name!!";
  /// <inheritdoc/>
  public string GetSaveSlotSubtitle(string absolutePath) => "N/A";
}
