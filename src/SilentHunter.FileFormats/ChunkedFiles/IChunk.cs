using SilentHunter.FileFormats.IO;

namespace SilentHunter.FileFormats.ChunkedFiles;

/// <summary>
/// Represents a file chunk, which is identified via a magic.
/// </summary>
public interface IChunk : IRawSerializable
{
    /// <summary>
    /// Gets or sets the magic.
    /// </summary>
    object Magic { get; set; }
    /// <summary>
    /// Gets or sets the size of the chunk.
    /// </summary>
    long Size { get; set; }
    /// <summary>
    /// Gets or sets the file offset.
    /// </summary>
    long FileOffset { get; set; }
    /// <summary>
    /// Gets or sets the parent file.
    /// </summary>
    IChunkFile ParentFile { get; set; }
}

/// <summary>
/// Represents a file chunk, which is identified via a strongly typed magic.
/// </summary>
/// <typeparam name="TMagic">The type of the magic.</typeparam>
public interface IChunk<TMagic> : IChunk
{
    /// <summary>
    /// Gets or sets the magic.
    /// </summary>
    new TMagic Magic { get; set; }
}