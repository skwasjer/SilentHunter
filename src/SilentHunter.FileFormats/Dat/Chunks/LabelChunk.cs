using System.IO;
using System.Threading.Tasks;
using skwas.IO;

namespace SilentHunter.FileFormats.Dat.Chunks
{
	public sealed class LabelChunk : DatChunk
	{
		public LabelChunk()
			: base(DatFile.Magics.Label)
		{
		}

		/// <summary>
		/// Gets or sets the label text.
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		/// Gets whether the chunk supports a parent id field.
		/// </summary>
		public override bool SupportsParentId => true;

		/// <summary>
		/// Deserializes the chunk.
		/// </summary>
		/// <param name="stream">The stream to read from.</param>
		protected override Task DeserializeAsync(Stream stream)
		{
			using (var reader = new BinaryReader(stream, FileEncoding.Default, true))
			{
				// Read the parent id.
				ParentId = reader.ReadUInt64();

				// The rest of the stream holds the name + terminating zero.
				if (stream.Length > stream.Position)
				{
					Text = reader.ReadNullTerminatedString();
				}
			}

			// Some files contain more data, problem seen mainly in older mods. Chunk will be correctly serialized upon next save.
			if (stream.Length > stream.Position)
			{
				stream.Position = stream.Length;
			}

			return Task.CompletedTask;
		}

		/// <summary>
		/// Serializes the chunk.
		/// </summary>
		/// <param name="stream">The stream to write to.</param>
		protected override Task SerializeAsync(Stream stream)
		{
			using (var writer = new BinaryWriter(stream, FileEncoding.Default, true))
			{
				// Write the parent id.
				writer.Write(ParentId);

				// Write name + terminating zero.
				if (!string.IsNullOrEmpty(Text))
				{
					writer.Write(Text, '\0');
				}
			}

			return Task.CompletedTask;
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>
		/// A string that represents the current object.
		/// </returns>
		public override string ToString()
		{
			return base.ToString() + ": " + Text;
		}
	}
}