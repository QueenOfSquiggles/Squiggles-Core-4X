namespace Squiggles.Core.Math;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Godot;



/// <summary>
/// For some reason Vector2I is not working well for equating for a SortedDictionary in the Item system. This serves to replace the Vector2I in that case.
/// Note that all comparisons of InventoryPositions are operating on the HashCode which is generated as an integer index for an imaginary container of size HASH_CODE_CONTAINER_SIZE, which currently is 128 <seealso cref="HASH_CODE_CONTAINER_SIZE"/>
/// <para/>
/// See also <see cref="Vector2I"/>
/// </summary>
public class InventoryPosition : IEqualityComparer<InventoryPosition>, IComparable<InventoryPosition> {
  public static readonly InventoryPosition Zero = new(0, 0);
  public static readonly InventoryPosition One = new(0, 0);

  private const int HASH_CODE_CONTAINER_SIZE = 128;

  public int X { get; set; }
  public int Y { get; set; }


  public static InventoryPosition FromVector2I(Vector2I vec)
    => new() {
      X = vec.X,
      Y = vec.Y
    };

  public static InventoryPosition FromIndex(int index, int container_width)
    => new() {
      X = index % container_width,
      Y = index / container_width
    };


  public InventoryPosition() { }

  public InventoryPosition(int x, int y) {
    X = x;
    Y = y;
  }

  public Vector2I ToVector2I()
    => new() {
      X = X,
      Y = Y
    };

  public int ToIndex(int container_width) => X + (Y * container_width);

  public override string ToString() => $"vec2i({X}, {Y})";

  public int CompareTo(InventoryPosition other) {
    if (other is null) {
      return 0;
    }

    var indexOther = other.ToIndex(HASH_CODE_CONTAINER_SIZE);
    var indexSelf = ToIndex(HASH_CODE_CONTAINER_SIZE);
    return indexOther == indexSelf ? 0 : indexOther < indexSelf ? -1 : 1;
  }

  public bool Equals(InventoryPosition x, InventoryPosition y) {
    if (x is null || y is null) {
      return x is null && y is null;
    }
    return x.X == y.X && x.Y == y.Y;
  }

  // treat hash code like a really big container index
  public int GetHashCode([DisallowNull] InventoryPosition obj) => obj.ToIndex(HASH_CODE_CONTAINER_SIZE);

  public InventoryPosition Copy() => (InventoryPosition)MemberwiseClone();

  public static implicit operator Vector2I(InventoryPosition vec) => vec.ToVector2I();

  public static InventoryPosition operator +(InventoryPosition vec, int value) {
    var n_vec = vec.Copy();
    n_vec.X += value;
    n_vec.Y += value;
    return n_vec;
  }
  public static InventoryPosition operator -(InventoryPosition vec, int value) {
    var n_vec = vec.Copy();
    n_vec.X -= value;
    n_vec.Y -= value;
    return n_vec;
  }
  public static InventoryPosition operator /(InventoryPosition vec, int value) {
    var n_vec = vec.Copy();
    n_vec.X /= value;
    n_vec.Y /= value;
    return n_vec;
  }
  public static InventoryPosition operator *(InventoryPosition vec, int value) {
    var n_vec = vec.Copy();
    n_vec.X *= value;
    n_vec.Y *= value;
    return n_vec;
  }

  public static InventoryPosition operator +(InventoryPosition vec, InventoryPosition vec2) {
    var n_vec = vec.Copy();
    n_vec.X += vec2.X;
    n_vec.Y += vec2.Y;
    return n_vec;
  }

  public static InventoryPosition operator -(InventoryPosition vec, InventoryPosition vec2) {
    var n_vec = vec.Copy();
    n_vec.X -= vec2.X;
    n_vec.Y -= vec2.Y;
    return n_vec;
  }

  public static InventoryPosition operator /(InventoryPosition vec, InventoryPosition vec2) {
    var n_vec = vec.Copy();
    n_vec.X /= vec2.X;
    n_vec.Y /= vec2.Y;
    return n_vec;
  }

  public static InventoryPosition operator *(InventoryPosition vec, InventoryPosition vec2) {
    var n_vec = vec.Copy();
    n_vec.X *= vec2.X;
    n_vec.Y *= vec2.Y;
    return n_vec;
  }

  public static bool operator <(InventoryPosition a, InventoryPosition b) => a.GetHashCode() < b.GetHashCode();
  public static bool operator >(InventoryPosition a, InventoryPosition b) => a.GetHashCode() < b.GetHashCode();
  public static bool operator <=(InventoryPosition a, InventoryPosition b) => a.GetHashCode() <= b.GetHashCode();
  public static bool operator >=(InventoryPosition a, InventoryPosition b) => a.GetHashCode() <= b.GetHashCode();
  public static bool operator ==(InventoryPosition a, InventoryPosition b) => a.GetHashCode() == b.GetHashCode();
  public static bool operator !=(InventoryPosition a, InventoryPosition b) => a.GetHashCode() != b.GetHashCode();

  public override bool Equals(object obj) {
    if (ReferenceEquals(this, obj)) {
      return true;
    }

    return obj is InventoryPosition pos && pos.GetHashCode() == GetHashCode();
  }

  public override int GetHashCode() => GetHashCode(this);
}
