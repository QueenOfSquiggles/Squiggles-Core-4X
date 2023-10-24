namespace Squiggles.Core.Extension;

using Godot;

/// <summary>
/// SC4X Godot.Transform3D extendsions (kinda Unity-style if you're into that kind of thing)
/// </summary>
public static class TransformExtensions {

  /// <summary>
  /// Using the basis, determines the forward (-Z) vector.
  /// </summary>
  public static Vector3 Forward(this Transform3D trans) => -trans.Basis.Z;
  /// <summary>
  /// Using the basis, determines the right (+X) vector.
  /// </summary>
  public static Vector3 Right(this Transform3D trans) => trans.Basis.X;
  /// <summary>
  /// Using the basis, determines the Up (+Y) vector.
  /// </summary>
  public static Vector3 Up(this Transform3D trans) => trans.Basis.Y;
  /// <summary>
  /// Using the basis, determines the Back (+Z) vector.
  /// </summary>
  public static Vector3 Back(this Transform3D trans) => -trans.Forward();
  /// <summary>
  /// Using the basis, determines the Left (-X) vector.
  /// </summary>
  public static Vector3 Left(this Transform3D trans) => -trans.Right();
  /// <summary>
  /// Using the basis, determines the Down (-Y) vector.
  /// </summary>
  public static Vector3 Down(this Transform3D trans) => -trans.Up();

}
