using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using SilentHunter.FileFormats.Extensions;

namespace SilentHunter.FileFormats.Dat.Chunks
{
	/// <summary>
	/// Represents the label chunk.
	/// </summary>
	[DebuggerDisplay("{ToString(),nq}: {Text}")]
	public sealed class LabelChunk : DatChunk
	{
		private string _text;

		/// <summary>
		/// Initializes a new instance of the <see cref="LabelChunk"/> class.
		/// </summary>
		public LabelChunk()
			: base(DatFile.Magics.Label)
		{
		}

		/// <inheritdoc />
		public override bool SupportsParentId => true;

		/// <summary>
		/// Gets or sets the label text.
		/// </summary>
		public string Text
		{
			get => _text ?? string.Empty;
			set => _text = value;
		}

		/// <inheritdoc />
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

			// Some files contain more data, problem seen mainly in some mods due to hex editing likely. Chunk will be correctly serialized upon next save.
			if (stream.Length > stream.Position)
			{
				stream.Position = stream.Length;
			}

			return Task.CompletedTask;
		}

		/// <inheritdoc />
		protected override Task SerializeAsync(Stream stream)
		{
			using (var writer = new BinaryWriter(stream, FileEncoding.Default, true))
			{
				// Write the parent id.
				writer.Write(ParentId);

				// Write name + terminating zero.
				writer.Write(Text, '\0');
			}

			return Task.CompletedTask;
		}
	}
}