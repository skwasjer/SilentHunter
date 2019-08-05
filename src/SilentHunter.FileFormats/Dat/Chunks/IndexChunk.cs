using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SilentHunter.FileFormats.Dat.Chunks
{
	/// <summary>
	/// Represents the index chunk, which stores chunk ids with their absolute file position.
	/// </summary>
	[DebuggerDisplay("{ToString(),nq}: {Entries.Count} entries")]
	public sealed class IndexChunk : DatChunk
	{
		/// <summary>
		/// Represents a file index of a chunk.
		/// </summary>
		[DebuggerDisplay("Id = {Id,nq}, FileOffset = {FileOffset,nq}")]
		[StructLayout(LayoutKind.Sequential, Pack = 4)]
		public struct Entry
		{
			/// <summary>
			/// The chunk id.
			/// </summary>
			public ulong Id;

			/// <summary>
			/// The file offset at which the chunk is located.
			/// </summary>
			public int FileOffset;

			/// <summary>
			/// The size of this structure.
			/// </summary>
			internal static readonly int Size = Marshal.SizeOf(typeof(Entry));
		}

		public IndexChunk()
			: base(DatFile.Magics.Index)
		{
		}

		/// <summary>
		/// Gets a list of index entries.
		/// </summary>
		public List<Entry> Entries { get; } = new List<Entry>();

		/// <summary>
		/// Rebuilds the index by enumerating all chunks in the parent file.
		/// </summary>
		public void Rebuild()
		{
			Entries.Clear();

			if (ParentFile != null)
			{
				Entries.AddRange(
					ParentFile.Chunks
						.Cast<DatChunk>()
						.Where(chunk => chunk.SupportsId)
						.Select(chunk => new Entry
						{
							Id = chunk.Id,
							FileOffset = (int)chunk.FileOffset
						})
				);
			}
		}

		/// <inheritdoc />
		protected override Task DeserializeAsync(Stream stream)
		{
			using (var reader = new BinaryReader(stream, FileEncoding.Default, true))
			{
				Entries.Clear();

				int indexItems = (int)(stream.Length - stream.Position) / Entry.Size;
				for (int i = 0; i < indexItems; i++)
				{
					Entries.Add(new Entry
					{
						Id = reader.ReadUInt64(),
						FileOffset = reader.ReadInt32()
					});
				}
			}

			// Sometimes some extra invalid data (not divisible by 12) is here, problem seen mainly in some mods due to hex editing likely. Chunk will be correctly serialized upon next save.
			if (stream.Length > stream.Position)
			{
				stream.Position = stream.Length;
			}

			return Task.CompletedTask;
		}

		/// <inheritdoc />
		protected override Task SerializeAsync(Stream stream)
		{
			Rebuild();

			using (var writer = new BinaryWriter(stream, FileEncoding.Default, true))
			{
				foreach (Entry entry in Entries)
				{
					writer.Write(entry.Id);
					writer.Write(entry.FileOffset);
				}
			}

			return Task.CompletedTask;
		}
	}
}