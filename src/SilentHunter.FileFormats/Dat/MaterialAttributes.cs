﻿using System;

namespace SilentHunter.FileFormats.Dat;

/// <summary>
/// Material attributes.
/// </summary>
[Flags]
public enum MaterialAttributes
{
    /// <summary>
    /// </summary>
    None = 0,

    /// <summary>
    /// If this flag is set, z-buffer writing is disabled.
    /// </summary>
    DisableZBufferWrite = 1,

    /// <summary>
    /// If this flag is specified, culling is set to none (allowing back faces to be rendered). If not, culling is counterclockwise (allowing only front side faces).
    /// </summary>
    CullNone = 2,

    // =================================
    // Only available for the material chunks with the bigger size.

    /// <summary>
    /// Use linear (or better) minification filter.
    /// </summary>
    MinFilterLinear = 4,

    /// <summary>
    /// Use linear (or better) magnification filter.
    /// </summary>
    MagFilterLinear = 8,

    /// <summary>
    /// Disable in-memory DXT-texture compression (DXT2/3).
    /// </summary>
    NoDxtCompression = 0x8000
}