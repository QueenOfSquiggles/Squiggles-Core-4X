using Godot;
using queen.math;

namespace queen.extension;

public static class Vector2IExtensions
{

    public static InventoryPosition ToInventoryPosition(this Vector2I vec)
    {
        return InventoryPosition.FromVector2I(vec);
    }

}