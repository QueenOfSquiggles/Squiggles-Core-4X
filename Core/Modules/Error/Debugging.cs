namespace Squiggles.Core.Error;

/// <summary>
/// A utility singleton that helps with assertions
/// </summary>
public static class Debugging {

  /// <summary>
  /// A flexible assertion approach that reacts to whether it is executing in a debugging environment or not. In a debugging environment it will print and error with a stack trace when the value is false. In a non-debug environment (release) it will simply return the value. This allows for an aggressive style programming in debug environments and a defensive style programming in release environments. If you're not familiar with those terms, look them up. It could change your frickin life.
  /// </summary>
  /// <param name="value">The boolean value to assert is true.</param>
  /// <param name="message">A customizable message, defaults to "Assertion Failed"</param>
  /// <returns>The value param without any modification</returns>
  public static bool Assert(bool value, string message = "Assertion Failed") {
#if DEBUG
    if (value) {
      return value;
    }

    Print.Error(message);

#endif
    return value;
  }

}
