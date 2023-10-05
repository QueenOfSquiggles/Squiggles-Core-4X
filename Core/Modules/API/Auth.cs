namespace Squiggles.Core.API;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

/// <summary>
/// The main method for acquiring authorization keys for different app services.
/// </summary>
public static class Auth {

  /// <param name="name">The key name for the value</param>
  /// <returns>an instance of SecureKey which stores the value while only allowing it to be accessed once.</returns>
  public static SecureKey GetKey(string name) {
    var builder = Host.CreateApplicationBuilder();
    var env = builder.Environment;
    builder.Configuration
      .AddJsonFile("appconfig.json", optional: true, reloadOnChange: true)
      .AddEnvironmentVariables(name);
    var key = builder.Configuration.GetValue(typeof(string), name);
    return key is null ? null : new SecureKey(key as string);
  }
}
