namespace Squiggles.Core.Meta;

using System.Linq;
using Godot;


/// <summary>
/// A super rudimentary save slot information provider. It creates absolute bare minimum functionality so you don't have to make the feature a huge priority. However, it is recommended to create your own implementation eventually to yield more descriptive save slot information!
/// Custom implementations should extend this class. This will allow them to populate into the inspector. Simply override the below functions, optionally include any export settings if you want something more dynamic.
/// </summary>
[GlobalClass]
public partial class SlotInfoProviderResource : Resource, ISaveSlotInformationProvider {

  // Oh damn do I love inheritdoc! That's some good shit!!!
  /// <inheritdoc/>
  public virtual string GetSaveSlotName(string absolutePath) => absolutePath?.Split("/")?.Last() ?? "!!Failed to find directory name!!";

  /// <inheritdoc/>
  public virtual string GetSaveSlotSubtitle(string absolutePath) => "N/A";
}
