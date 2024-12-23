using System;

namespace SilentHunter.FileFormats.ChunkedFiles;

/// <summary>
/// Represents an activator for creating chunks.
/// </summary>
public interface IChunkActivator
{
    /// <summary>
    /// Creates a chunk of type <paramref name="chunkType"/> with optional <paramref name="magic"/>.
    /// </summary>
    /// <param name="chunkType">The chunk type.</param>
    /// <param name="magic">The magic.</param>
    /// <returns>the newly created chunk</returns>
    /// <exception cref="InvalidOperationException">Thrown when the chunk could not be created.</exception>
    IChunk Create(Type chunkType, object magic);
}