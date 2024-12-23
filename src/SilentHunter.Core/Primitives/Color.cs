using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace SilentHunter;

/// <summary>
/// Represents a color primitive.
/// </summary>
[Serializable]
[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct Color : IEquatable<Color>
{
    /// <summary>
    /// The alpha component.
    /// </summary>
    [FieldOffset(0)]
    public byte A;

    /// <summary>
    /// The red component.
    /// </summary>
    [FieldOffset(3)]
    public byte R;

    /// <summary>
    /// The green component.
    /// </summary>
    [FieldOffset(2)]
    public byte G;

    /// <summary>
    /// The blue component.
    /// </summary>
    [FieldOffset(1)]
    public byte B;

    /// <summary>
    /// Returns the string representation, optionally omitting the alpha value.
    /// </summary>
    /// <param name="ignoreAlpha">true to omit the alpha component in the string representation</param>
    public string ToString(bool ignoreAlpha)
    {
        return A < 255 && !ignoreAlpha
            ? string.Format("A: {1}{0} R: {2}{0} G: {3}{0} B: {4}", CultureInfo.CurrentUICulture.TextInfo.ListSeparator, A, R, G, B)
            : string.Format("R: {1}{0} G: {2}{0} B: {3}", CultureInfo.CurrentUICulture.TextInfo.ListSeparator, R, G, B);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return ToString(true);
    }

    /// <summary>
    /// Returns a <see cref="Color"/> from an alpha value and a <paramref name="color"/>.
    /// </summary>
    public static Color FromArgb(int alpha, Color color)
    {
        color.A = (byte)(alpha & byte.MaxValue);
        return color;
    }

    /// <summary>
    /// Returns a <see cref="Color"/> from an alpha, red, green and blue component.
    /// </summary>
    public static Color FromArgb(int alpha, int red, int green, int blue)
    {
        return new Color
        {
            A = (byte)(alpha & byte.MaxValue),
            R = (byte)(red & byte.MaxValue),
            G = (byte)(green & byte.MaxValue),
            B = (byte)(blue & byte.MaxValue)
        };
    }

    /// <summary>
    /// Returns a <see cref="Color"/> from a red, green and blue component.
    /// </summary>
    public static Color FromArgb(int red, int green, int blue)
    {
        return FromArgb(byte.MaxValue, red, green, blue);
    }

    /// <summary>
    /// Returns a <see cref="Color"/> from an integer.
    /// </summary>
    public static Color FromArgb(int argb)
    {
        return new Color
        {
            A = (byte)(((long)argb >> 24) & byte.MaxValue),
            R = (byte)((argb >> 16) & byte.MaxValue),
            G = (byte)((argb >> 8) & byte.MaxValue),
            B = (byte)(argb & byte.MaxValue)
        };
    }

    /// <summary>
    /// Converts to an integer.
    /// </summary>
    public int ToArgb()
    {
        return (int)(((long)A << 24) | ((long)R << 16) | ((long)G << 8) | B);
    }

    /// <summary>
    /// </summary>
    public bool Equals(Color other)
    {
        return A == other.A && R == other.R && G == other.G && B == other.B;
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is Color other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        // ReSharper disable NonReadonlyMemberInGetHashCode
        return (A << 24) | (R << 16) | (G << 8) | B;
        // ReSharper restore NonReadonlyMemberInGetHashCode
    }

    /// <summary>
    /// </summary>
    public static bool operator ==(Color left, Color right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// </summary>
    public static bool operator !=(Color left, Color right)
    {
        return !left.Equals(right);
    }
}