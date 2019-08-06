using System;
using System.IO;
using System.Threading.Tasks;
using SilentHunter.FileFormats.Extensions;

namespace SilentHunter.FileFormats.Dat.Chunks.Partial
{
#if DEBUG
	/// <summary>
	/// </summary>
	/// <remarks>
	/// Largely unknown chunk (not released).
	/// </remarks>
	public sealed class Animation1Chunk : DatChunk
	{
		/// <summary>
		/// </summary>
		public Animation1Chunk()
			: base(DatFile.Magics.Animation1)
		{
		}

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the chunk id.
		/// </summary>
		public override ulong Id
		{
			get => base.Id;
			set
			{
				if (value > uint.MaxValue)
				{
					throw new ArgumentOutOfRangeException(nameof(value), "The id for this chunk is only 4 bytes in length (UInt32).");
				}

				base.Id = value;
			}
		}

		/// <summary>
		/// Gets or sets the chunk its parent id.
		/// </summary>
		public override ulong ParentId
		{
			get => base.ParentId;
			set
			{
				if (value > uint.MaxValue)
				{
					throw new ArgumentOutOfRangeException(nameof(value), "The parent id for this chunk is only 4 bytes in length (UInt32).");
				}

				base.ParentId = value;
			}
		}

		/// <summary>
		/// Gets whether the chunk supports an id field.
		/// </summary>
		public override bool SupportsId => true;

		/// <summary>
		/// Gets whether the chunk supports a parent id field.
		/// </summary>
		public override bool SupportsParentId => true;

		/// <inheritdoc />
		protected override Task DeserializeAsync(Stream stream)
		{
			using (var reader = new BinaryReader(stream, FileEncoding.Default, true))
			{
				Id = reader.ReadUInt32();
				// Read the parent id.
				ParentId = reader.ReadUInt32();

				// The rest of the stream holds the name + terminating zero.
				if (stream.Length > stream.Position)
				{
					Name = reader.ReadNullTerminatedString();
				}

				// Make sure we are at end of stream.
				//if (stream.Length > stream.Position)
				//	stream.Position = stream.Length;
				//Debug.WriteLine(_texture);
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
				if (!string.IsNullOrEmpty(Name))
				{
					writer.Write(Name, '\0');
				}
				else
				{
					writer.Write(byte.MinValue);
				}
			}

			return Task.CompletedTask;
		}

		/// <inheritdoc />
		public override string ToString()
		{
			return base.ToString() + ": " + Name;
		}
	}
#endif
}