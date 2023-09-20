namespace Squiggles.Core.Extension;

using Godot;

public static class TransformExtensions {

  public static Vector3 Forward(this Transform3D trans) => -trans.Basis.Z;
  public static Vector3 Right(this Transform3D trans) => trans.Basis.X;
  public static Vector3 Up(this Transform3D trans) => trans.Basis.Y;
  public static Vector3 Back(this Transform3D trans) => -trans.Forward();
  public static Vector3 Left(this Transform3D trans) => -trans.Right();
  public static Vector3 Down(this Transform3D trans) => -trans.Up();

}
