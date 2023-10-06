namespace Squiggles.Core.API;

using Godot;


/// <summary>
/// The main method for acquiring authorization keys for different app services.
/// </summary>
public static class Auth {

  /// <param name="name">The key name for the value</param>
  /// <returns>an instance of SecureKey which stores the value while only allowing it to be accessed once.</returns>
  public static SecureKey GetKey(string name) {
    using var file = FileAccess.Open("res://appconfig.json", FileAccess.ModeFlags.Read);
    if (file is null) { return null; }
    var data = Json.ParseString(file.GetAsText()).AsGodotDictionary();
    return data.ContainsKey(name) ? new SecureKey(data[name].AsString()) : null;
  }
}
