using System;

namespace SilentHunter.FileFormats.Dat.Chunks;

/// <summary>
/// Represents a texture chunk in the Silent Hunter DAT file format.
/// </summary>
public interface ITextureChunk
{
    /// <summary>
    /// Gets or sets the file name of the texture. Must be null, when non-explicit texture is used.
    /// </summary>
    public string Texture { get; set; }

    /// <summary>
    /// Gets or sets the file date/time of the texture.
    /// </summary>
    public DateTime CreationTime { get; set; }

    /// <summary>
    /// Gets or sets the file size of an explicit texture, when stored as TGA. Even if the file is DDS, the size indicated matches that of a TGA.
    /// </summary>
    public int TgaTextureSize { get; set; }

    /// <summary>
    /// Gets or sets the material attributes.
    /// </summary>
    public MaterialAttributes Attributes { get; set; }
}
