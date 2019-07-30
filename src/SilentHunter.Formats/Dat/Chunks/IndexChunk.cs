using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace SilentHunter.Dat.Chunks
{
	public sealed class IndexChunk : DatChunk
	{
		/// <summary>
		/// Represents a file index of a chunk.
		/// </summary>
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
			public static readonly int Size = Marshal.SizeOf(typeof(Entry));

			/// <summary>
			/// Returns the fully qualified type name of this instance.
			/// </summary>
			/// <returns>
			/// A <see cref="T:System.String" /> containing a fully qualified type name.
			/// </returns>
			public override string ToString()
			{
				return $"0x{Id:x16}, offset 0x{FileOffset:x8}";
			}
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
		/// Deserializes the chunk.
		/// </summary>
		/// <param name="stream">The stream to read from.</param>
		protected override void Deserialize(Stream stream)
		{
			using (var reader = new BinaryReader(stream, Encoding.ParseEncoding, true))
			{
				Entries.Clear();

				int indexItems = (int)(stream.Length - stream.Position) / Entry.Size;
				for (var i = 0; i < indexItems; i++)
				{
					Entries.Add(new Entry
					{
						Id = reader.ReadUInt64(),
						FileOffset = reader.ReadInt32()
					});
				}
			}

			// Some GWX IndexEntry chunks contain some extra invalid data (not divisible by 12). We fix this by forwarding to end of stream. Upon the next save, the index is rebuilt and the problem will restore itself.
			if (stream.Length > stream.Position)
			{
				stream.Position = stream.Length;
			}
		}

		/// <summary>
		/// Serializes the chunk.
		/// </summary>
		/// <param name="stream">The stream to write to.</param>
		protected override void Serialize(Stream stream)
		{
			Rebuild();

			using (var writer = new BinaryWriter(stream, Encoding.ParseEncoding, true))
			{
				Entries.ForEach(entry =>
				{
					writer.Write(entry.Id);
					writer.Write(entry.FileOffset);
				});
			}
		}

		/// <summary>
		/// Rebuilds the index by enumerating all chunks in the parent file.
		/// </summary>
		public void Rebuild()
		{
			Entries.Clear();

			Entries.AddRange(
				ParentFile.Chunks.Cast<DatChunk>()
					.Where(chunk => chunk.SupportsId)
					.Select(chunk => new Entry
					{
						Id = chunk.Id,
						FileOffset = (int)chunk.FileOffset
					})
			);
		}
	}
}