using System;
using System.Collections.ObjectModel;
using System.Linq;
using skwas.IO;

namespace SilentHunter.Dat
{
	public class DatChunkResolver
		: IChunkResolver<DatFile.Magics>
	{
		private static readonly ReadOnlyDictionary<DatFile.Magics, Type> ResolvedTypes = LoadTypes();

		private static ReadOnlyDictionary<DatFile.Magics, Type> LoadTypes()
		{
			DatFile.Magics parsed;
			var types = typeof (DatFile).Assembly
				.DefinedTypes
				.Where(t => t.IsClass && typeof (IChunk).IsAssignableFrom(t.UnderlyingSystemType) && Enum.TryParse(t.Name, false, out parsed))
				.ToDictionary(
					kvp => (DatFile.Magics)Enum.Parse(typeof(DatFile.Magics), kvp.Name, false),
					kvp => kvp.UnderlyingSystemType
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
			return Resolve((DatFile.Magics) magic);
		}

		#endregion
	}
}
