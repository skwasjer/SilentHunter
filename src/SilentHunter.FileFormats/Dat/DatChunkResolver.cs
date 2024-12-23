using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using SilentHunter.FileFormats.ChunkedFiles;

namespace SilentHunter.FileFormats.Dat;

/// <summary>
/// Chunk resolver that returns the chunk type that is associated with a given magic.
/// </summary>
public class DatChunkResolver : IChunkResolver<DatFile.Magics>
{
    private static readonly Lazy<ReadOnlyDictionary<DatFile.Magics, Type>> ResolvedTypes = new Lazy<ReadOnlyDictionary<DatFile.Magics, Type>>(LoadTypes, LazyThreadSafetyMode.PublicationOnly);

    private static ReadOnlyDictionary<DatFile.Magics, Type> LoadTypes()
    {
        Dictionary<DatFile.Magics, Type> types = typeof(DatFile).Assembly
            .DefinedTypes
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IChunk).IsAssignableFrom(t.UnderlyingSystemType))
            .Select(t =>
            {
                string chunkName = t.Name;
                if (chunkName.EndsWith("Chunk"))
                {
                    chunkName = chunkName.Substring(0, t.Name.Length - "Chunk".Length);
                }

                if (Enum.TryParse(chunkName, out DatFile.Magics m))
                {
                    return new
                    {
                        Magic = m,
                        Type = t.UnderlyingSystemType
                    };
                }

                return null;
            })
            .Where(t => t != null)
            .ToDictionary(
                kvp => kvp.Magic,
                kvp => kvp.Type
            );

        return new ReadOnlyDictionary<DatFile.Magics, Type>(types);
    }

    /// <summary>
    /// Resolves the type that is associated with the specified magic.
    /// </summary>
    /// <param name="magic">The magic to resolve a type for.</param>
    /// <returns>The type or null if the magic is not supported/implemented.</returns>
    public Type Resolve(DatFile.Magics magic)
    {
        return ResolvedTypes.Value.ContainsKey(magic) ? ResolvedTypes.Value[magic] : null;
    }

    /// <summary>
    /// Resolves the type that is associated with the specified magic.
    /// </summary>
    /// <param name="magic">The magic to resolve a type for.</param>
    /// <returns>The type or null if the magic is not supported/implemented.</returns>
    Type IChunkResolver.Resolve(object magic)
    {
        return Resolve((DatFile.Magics)magic);
    }
}