namespace Squiggles.Core.Extension;

using System;
using System.Reflection;
using Squiggles.Core.Error;

/// <summary>
/// SC4X Extensions for System.Reflection.Assembly
/// </summary>
public static class AssemblyExtensions {

  /// <summary>
  /// An incredibly naive reflection based type loading function. Given the full class name and expected type, tries to instance an object, returning null when failing. If an error occurs during the loading process a warning with a stack trace will be emitted. This method does require an explicit constructor that takes no arguments in order to work properly.
  /// This was originally created to help me with a specific implementation, which proved to be less efficient than using Godot's Resource so I scrapped the idea. But this utility exists just in case someone finds a use for it. Reflection is pretty powerful so use it wisely.
  /// </summary>
  /// <typeparam name="T">The type for the instance to expect and be cast to</typeparam>
  /// <param name="assem">The target assembly. If no fancy C# work has been done, all of Squiggles.Core as well as your game code should be in the currently executing assembly. </param>
  /// <param name="classNameFull">The full name of the class. For this class, that would be "Squiggles.Core.Extension.AssemblyExtensions", but of course as a static class it couldn't be instanced.</param>
  /// <returns>The instance of T, or null if a failure occurred</returns>
  public static T InstanceClassOrNullSimple<T>(this Assembly assem, string classNameFull) where T : class {
    try {
      var type = assem.GetType(classNameFull);
      var available = type.GetConstructors();
      ConstructorInfo targetConstructor = null;
      foreach (var a in available) {
        var @params = a.GetParameters();
        if (@params.Length <= 0) {
          targetConstructor = a;
        }
      }

      var tempObj = targetConstructor?.Invoke(Array.Empty<object>());
      return tempObj as T;

    }
    catch (Exception e) {
      Print.Warn($"Failed to reflect instance of `{classNameFull}` : {e.Message} \n\n {e}");
    }
    return null;

  }
}
