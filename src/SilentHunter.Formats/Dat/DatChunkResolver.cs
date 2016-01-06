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
			var types = typeof (DatFile).Assembly
				.DefinedTypes
				.Where(t => t.IsClass && typeof (IChunk).IsAssignableFrom(t.UnderlyingSystemType))
				.Select(t =>
				{
					DatFile.Magics m;
					if (t.Name.Length > 5 && Enum.TryParse(t.Name.Substring(0, t.Name.Length - 5), out m))
						return new
						{
							Magic = m,
							Type = t.UnderlyingSystemType
						};
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
			return Resolve((DatFile.Magics) magic);
		}

		#endregion
	}
}
