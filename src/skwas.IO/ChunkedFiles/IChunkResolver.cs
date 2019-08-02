using System;

namespace skwas.IO
{
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
		Type Resolve(object magic);
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
		Type Resolve(TMagic magic);
	}
}