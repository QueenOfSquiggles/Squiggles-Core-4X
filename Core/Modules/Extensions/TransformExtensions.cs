namespace queen.extension;

using Godot;

public static class TransformExtensions
{

    public static Vector3 Forward(this Transform3D trans)
    {
        return -trans.Basis.Z;
    }
    public static Vector3 Right(this Transform3D trans)
    {
        return trans.Basis.X;
    }
    public static Vector3 Up(this Transform3D trans)
    {
        return trans.Basis.Y;
    }
    public static Vector3 Back(this Transform3D trans)
    {
        return -trans.Forward();
    }
    public static Vector3 Left(this Transform3D trans)
    {
        return -trans.Right();
    }
    public static Vector3 Down(this Transform3D trans)
    {
        return -trans.Up();
    }

}