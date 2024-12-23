using System.Collections.Generic;

namespace SilentHunter.Controllers;

/// <summary>
/// Represents a list of compressed vectors.
/// </summary>
public class CompressedVectors
{
    /// <summary>
    /// The scale factor.
    /// </summary>
    public float Scale;

    /// <summary>
    /// The translation factor.
    /// </summary>
    public float Translation;

    /// <summary>
    /// The list of compressed vectors.
    /// </summary>
    public List<short> Vectors;
}