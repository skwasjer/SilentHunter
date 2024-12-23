using System;
using System.Collections;

namespace SilentHunter.FileFormats.ChunkedFiles;

/// <summary>
/// Reads/write binary files that are based on individual chunks (of blocks).
/// </summary>
public interface IChunkFile
{
    /// <summary>
    /// Gets the chunks in the file.
    /// </summary>
    IList Chunks { get; }
}