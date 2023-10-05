namespace Squiggles.Core.API;

/// <summary>
/// Stores a string value and only allows access to it once.
/// </summary>
public sealed class SecureKey {

  private string _value;
  internal SecureKey(string value) {
    _value = value;
  }

  public string GetSecureKey() {
    var temp = _value;
    _value = null;
    return temp;
  }

}
