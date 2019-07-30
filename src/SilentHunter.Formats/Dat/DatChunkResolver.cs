using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using skwas.IO;

namespace SilentHunter.Dat
{
	public class DatChunkResolver : IChunkResolver<DatFile.Magics>
	{
		private static readonly ReadOnlyDictionary<DatFile.Magics, Type> ResolvedTypes = LoadTypes();

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

		#region Implementation of IChunkResolver

		/// <summary>
		/// Resolves the type that is associated with the specified magic.
		/// </summary>
		/// <param name="magic">The magic to resolve a type for.</param>
		/// <returns>The type or null if the magic is not supported/implemented.</returns>
		public Type Resolve(DatFile.Magics magic)
		{
			return ResolvedTypes.ContainsKey(magic) ? ResolvedTypes[magic] : null;
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

		#endregion
	}
}