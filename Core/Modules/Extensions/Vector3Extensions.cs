namespace Squiggles.Core.Extension;

using Godot;

/// <summary>
/// SC4X Godot.Vector3 extensions
/// </summary>
public static class Vector3Extensions {

  /// <summary>
  /// returns the float angle of the XZ components of the vector. Assuming Y-up, this provides the horizontal angle for rotating to look in a particular direction. Using Node.LookAt may be better/easier but I made this so it's staying
  /// </summary>
  /// <param name="vec"></param>
  /// <returns></returns>
  public static float AngleXZ(this Vector3 vec) => Mathf.Atan2(vec.X, vec.Z);

  /// <summary>
  /// rotates the vector along the X axis. Used for FPS camera rotation so I don't have to manually make a copy of the rotation vector every time I write a new FPS character controller. Fucking hell I've written so many god damn FPS character controllers. When will I be free of the FPS beans!?
  /// </summary>
  /// <param name="vec"></param>
  /// <param name="angle"></param>
  /// <returns></returns>
  public static Vector3 SetRotateX(this Vector3 vec, float angle) {
    var copy = vec;
    copy.X = angle;
    return copy;
  }

  /// <summary>
  /// rotates the vecotr along the Y axis
  /// </summary>
  /// <param name="vec"></param>
  /// <param name="angle"></param>
  /// <returns></returns>
  public static Vector3 SetRotateY(this Vector3 vec, float angle) {
    var copy = vec;
    copy.Y = angle;
    return copy;
  }
  /// <summary>
  /// Rotates the vector along the Z axis.
  /// </summary>
  /// <param name="vec"></param>
  /// <param name="angle"></param>
  /// <returns></returns>
  public static Vector3 SetRotateZ(this Vector3 vec, float angle) {
    var copy = vec;
    copy.Z = angle;
    return copy;
  }


}
