namespace Squiggles.Core.Data;

using Squiggles.Core.Data;

public interface IHasSaveData
{

  /// <summary>
  /// write component's save data to the save data builder. Which saving out to file is handled by parent
  /// </summary>
  public void Serialize(SaveDataBuilder builder);

  /// <summary>
  /// read component's save data from the save data builder. Which loading from file is handled by parent
  /// </summary>
  public void Deserialize(SaveDataBuilder builder);

}
