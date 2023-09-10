using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Godot;

namespace queen.math;


/// <summary>
/// For some reason Vector2I is not working well for equating for a SortedDictionary in the Item system. This serves to replace the Vector2I in that case.
/// Note that all comparisons of InventoryPositions are operating on the HashCode which is generated as an integer index for an imaginary container of size HASH_CODE_CONTAINER_SIZE, which currently is 128 <seealso cref="HASH_CODE_CONTAINER_SIZE"/>
/// See also <seealso cref="Vector2I"/>
/// </summary>
public class InventoryPosition : IEqualityComparer<InventoryPosition>, IComparable<InventoryPosition>
{
    public static readonly InventoryPosition Zero = new(0, 0);
    public static readonly InventoryPosition One = new(0, 0);

    private const int HASH_CODE_CONTAINER_SIZE = 128;

    public int X { get; set; } = 0;
    public int Y { get; set; } = 0;


    public static InventoryPosition FromVector2I(Vector2I vec)
    {
        return new InventoryPosition()
        {
            X = vec.X,
            Y = vec.Y
        };
    }

    public static InventoryPosition FromIndex(int index, int container_width)
    {
        return new InventoryPosition()
        {
            X = index % container_width,
            Y = index / container_width
        };
    }


    public InventoryPosition() { }

    public InventoryPosition(int m_x, int m_y)
    {
        X = m_x;
        Y = m_y;
    }

    public Vector2I ToVector2I()
    {
        return new Vector2I()
        {
            X = X,
            Y = Y
        };
    }

    public int ToIndex(int container_width)
    {
        return X + (Y * container_width);
    }

    public override string ToString()
    {
        return $"vec2i({X}, {Y})";
    }
    public int CompareTo(InventoryPosition? other)
    {
        if (other is null) return 0;
        int indexOther = other.ToIndex(HASH_CODE_CONTAINER_SIZE);
        int indexSelf = ToIndex(HASH_CODE_CONTAINER_SIZE);
        if (indexOther == indexSelf) return 0;
        return indexOther < indexSelf ? -1 : 1;
    }

    public bool Equals(InventoryPosition? x, InventoryPosition? y)
    {
        if (x is null || y is null)
        {
            return x is null && y is null;
        }
        return x.X == y.X && x.Y == y.Y;
    }

    public int GetHashCode([DisallowNull] InventoryPosition obj)
    {
        // treat hash code like a really big container index
        return obj.ToIndex(HASH_CODE_CONTAINER_SIZE);
    }

    public InventoryPosition Copy() => (InventoryPosition)MemberwiseClone();

    public static implicit operator Vector2I(InventoryPosition vec) => vec.ToVector2I();

    public static InventoryPosition operator +(InventoryPosition vec, int value)
    {
        var n_vec = vec.Copy();
        n_vec.X += value;
        n_vec.Y += value;
        return n_vec;
    }
    public static InventoryPosition operator -(InventoryPosition vec, int value)
    {
        var n_vec = vec.Copy();
        n_vec.X -= value;
        n_vec.Y -= value;
        return n_vec;
    }
    public static InventoryPosition operator /(InventoryPosition vec, int value)
    {
        var n_vec = vec.Copy();
        n_vec.X /= value;
        n_vec.Y /= value;
        return n_vec;
    }
    public static InventoryPosition operator *(InventoryPosition vec, int value)
    {
        var n_vec = vec.Copy();
        n_vec.X *= value;
        n_vec.Y *= value;
        return n_vec;
    }

    public static InventoryPosition operator +(InventoryPosition vec, InventoryPosition vec2)
    {
        var n_vec = vec.Copy();
        n_vec.X += vec2.X;
        n_vec.Y += vec2.Y;
        return n_vec;
    }

    public static InventoryPosition operator -(InventoryPosition vec, InventoryPosition vec2)
    {
        var n_vec = vec.Copy();
        n_vec.X -= vec2.X;
        n_vec.Y -= vec2.Y;
        return n_vec;
    }

    public static InventoryPosition operator /(InventoryPosition vec, InventoryPosition vec2)
    {
        var n_vec = vec.Copy();
        n_vec.X /= vec2.X;
        n_vec.Y /= vec2.Y;
        return n_vec;
    }

    public static InventoryPosition operator *(InventoryPosition vec, InventoryPosition vec2)
    {
        var n_vec = vec.Copy();
        n_vec.X *= vec2.X;
        n_vec.Y *= vec2.Y;
        return n_vec;
    }

    public static bool operator <(InventoryPosition a, InventoryPosition b)
    {
        return a.GetHashCode() < b.GetHashCode();
    }
    public static bool operator >(InventoryPosition a, InventoryPosition b)
    {
        return a.GetHashCode() < b.GetHashCode();
    }
    public static bool operator <=(InventoryPosition a, InventoryPosition b)
    {
        return a.GetHashCode() <= b.GetHashCode();
    }
    public static bool operator >=(InventoryPosition a, InventoryPosition b)
    {
        return a.GetHashCode() <= b.GetHashCode();
    }
    public static bool operator ==(InventoryPosition a, InventoryPosition b)
    {
        return a.GetHashCode() == b.GetHashCode();
    }
    public static bool operator !=(InventoryPosition a, InventoryPosition b)
    {
        return a.GetHashCode() != b.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        if (obj is not InventoryPosition pos) return false;
        return pos.GetHashCode() == GetHashCode();
    }

    public override int GetHashCode()
    {
        return GetHashCode(this);
    }
}