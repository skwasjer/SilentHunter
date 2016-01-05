using System.IO;
using skwas.IO;

namespace SilentHunter.Dat
{
	public sealed class Label : DatChunk
	{
		public Label()
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
		protected override void Deserialize(Stream stream)
		{
			using (var reader = new BinaryReader(stream, Encoding.ParseEncoding, true))
			{
				// Read the parent id.
				ParentId = reader.ReadUInt64();

				// The rest of the stream holds the name + terminating zero.
				if (stream.Length > stream.Position)
					Text = reader.ReadNullTerminatedString();
			}

			// Some files contain more data, problem seen mainly in older mods. Chunk will be correctly serialized upon next save.
			if (stream.Length > stream.Position) stream.Position = stream.Length;
		}

		/// <summary>
		/// Serializes the chunk.
		/// </summary>
		/// <param name="stream">The stream to write to.</param>
		protected override void Serialize(Stream stream)
		{
			using (var writer = new BinaryWriter(stream, Encoding.ParseEncoding, true))
			{
				// Write the parent id.
				writer.Write(ParentId);

				// Write name + terminating zero.
				if (!string.IsNullOrEmpty(Text))
					writer.Write(Text, '\0');
			}
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
