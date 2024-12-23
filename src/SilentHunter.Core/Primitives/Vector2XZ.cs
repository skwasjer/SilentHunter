using System.Globalization;
using System.Numerics;
using System.Runtime.InteropServices;

namespace SilentHunter;

/// <summary>
/// A special kind of vector that represents XZ.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
// TODO: this was originally introduced because of VertexStartPosition and some conversions were needed for S3D. Investigate if we can move this to Controller templates instead.
// ReSharper disable once InconsistentNaming
public struct Vector2XZ
{
    /// <summary>
    /// Gets or sets the X component.
    /// </summary>
    public float X;

    /// <summary>
    /// Gets or sets the Y component.
    /// </summary>
    public float Z;

    /// <inheritdoc />
    public override string ToString()
    {
        return string.Format("X: {1}{0} Z: {2}", CultureInfo.CurrentUICulture.TextInfo.ListSeparator, X, Z);
    }

    /// <summary>
    /// </summary>
    public static implicit operator Vector2(Vector2XZ v)
    {
        return new Vector2
        {
            X = v.X,
            Y = v.Z
        };
    }

    /// <summary>
    /// </summary>
    public static implicit operator Vector2XZ(Vector2 v)
    {
        return new Vector2XZ
        {
            X = v.X,
            Z = v.Y
        };
    }

    /// <summary>Determines whether the specified object is equal to the current object.</summary>
    /// <param name="other">The other to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public bool Equals(Vector2XZ other)
    {
        return X.Equals(other.X) && Z.Equals(other.Z);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is Vector2XZ vector && Equals(vector);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            // ReSharper disable NonReadonlyMemberInGetHashCode
            int hashCode = X.GetHashCode();
            hashCode = (hashCode * 397) ^ Z.GetHashCode();
            // ReSharper restore NonReadonlyMemberInGetHashCode
            return hashCode;
        }
    }

    /// <summary>
    /// Checks for equality.
    /// </summary>
    public static bool operator ==(Vector2XZ left, Vector2XZ right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Checks for inequality.
    /// </summary>
    public static bool operator !=(Vector2XZ left, Vector2XZ right)
    {
        return !left.Equals(right);
    }
}