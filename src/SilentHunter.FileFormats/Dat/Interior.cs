using System.Runtime.InteropServices;

namespace SilentHunter.FileFormats.Dat;

/// <summary>
/// Represents meta data for interior nodes.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public sealed class Interior
{
    internal Interior()
    {
    }

    internal int Reserved0 { get; set; }

    internal byte Reserved1 { get; set; }

    /// <summary>
    /// Gets or sets the bounding box.
    /// </summary>
    public BoundingBox BoundingBox { get; set; }

    internal uint Reserved2 { get; set; }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>
    /// A string that represents the current object.
    /// </returns>
    public override string ToString()
    {
        return BoundingBox.Min + " " + BoundingBox.Max + " " + Reserved0 + " " + Reserved1 + " " + Reserved2;
    }
}
