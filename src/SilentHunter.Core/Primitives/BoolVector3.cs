using System.Globalization;
using System.Runtime.InteropServices;

namespace SilentHunter;

/// <summary>
/// Represents a vector3 of boolean flags.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct BoolVector3
{
    /// <summary>
    /// The X component.
    /// </summary>
    [MarshalAs(UnmanagedType.U1)]
    public bool X;

    /// <summary>
    /// The Y component.
    /// </summary>
    [MarshalAs(UnmanagedType.U1)]
    public bool Y;

    /// <summary>
    /// The Z component.
    /// </summary>
    [MarshalAs(UnmanagedType.U1)]
    public bool Z;

    /// <inheritdoc />
    public override string ToString()
    {
        return string.Format("X: {1}{0} Y: {2}{0} Z: {3}", CultureInfo.CurrentUICulture.TextInfo.ListSeparator, X, Y, Z);
    }
}
