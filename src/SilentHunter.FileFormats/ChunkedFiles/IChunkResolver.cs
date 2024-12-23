﻿using System;

namespace SilentHunter.FileFormats.ChunkedFiles;

/// <summary>
/// Resolves a chunk type by a magic.
/// </summary>
public interface IChunkResolver
{
    /// <summary>
    /// Resolves the type that is associated with the specified magic.
    /// </summary>
    /// <param name="magic">The magic to resolve a type for.</param>
    /// <returns>The type or null if the magic is not supported/implemented.</returns>
    public Type Resolve(object magic);
}

/// <summary>
/// Resolves a chunk type by a magic.
/// </summary>
/// <typeparam name="TMagic"></typeparam>
public interface IChunkResolver<in TMagic>
    : IChunkResolver
{
    /// <summary>
    /// Resolves the type that is associated with the specified magic.
    /// </summary>
    /// <param name="magic">The magic to resolve a type for.</param>
    /// <returns>The type or null if the magic is not supported/implemented.</returns>
    public Type Resolve(TMagic magic);
}
