using System.Numerics;
using System.Runtime.InteropServices;

namespace SilentHunter;

/// <summary>
/// A bounding box is defined by two points in 3D space.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct BoundingBox
{
    /// <summary>
    /// The min component.
    /// </summary>
    public Vector3 Min { get; set; }

    /// <summary>
    /// The max component.
    /// </summary>
    public Vector3 Max { get; set; }
}
