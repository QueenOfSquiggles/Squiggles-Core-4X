namespace queen.extension;

using Godot;

public static class Vector3Extensions
{

    public static float AngleXZ(this Vector3 vec)
    {
        return Mathf.Atan2(vec.X, vec.Z);
    }

    public static Vector3 SetRotateX(this Vector3 vec, float angle)
    {
        var copy = vec;
        copy.X = angle;
        return copy;
    }
    public static Vector3 SetRotateY(this Vector3 vec, float angle)
    {
        var copy = vec;
        copy.Y = angle;
        return copy;
    }
    public static Vector3 SetRotateZ(this Vector3 vec, float angle)
    {
        var copy = vec;
        copy.Z = angle;
        return copy;
    }


}