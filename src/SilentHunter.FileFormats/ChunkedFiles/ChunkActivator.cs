using System;
using System.Reflection;

namespace SilentHunter.FileFormats.ChunkedFiles;

/// <summary>
/// Represents an activator for creating chunks using <see cref="Activator"/>.
/// </summary>
public class ChunkActivator : IChunkActivator
{
    /// <inheritdoc />
    public IChunk Create(Type chunkType, object magic)
    {
        if (chunkType == null)
        {
            throw new ArgumentNullException(nameof(chunkType));
        }

        if (magic == null)
        {
            throw new ArgumentNullException(nameof(magic));
        }

        ConstructorInfo constructor = chunkType.GetConstructor(
            BindingFlags.Public | BindingFlags.Instance,
            null,
            new[] { magic.GetType() },
            null
        );
        if (constructor != null)
        {
            return (IChunk)Activator.CreateInstance(chunkType, magic);
        }

        // Try a parameterless constructor.
        return (IChunk)Activator.CreateInstance(chunkType);
    }
}