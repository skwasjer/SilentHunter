using System;
using System.Threading.Tasks;
#if DEBUG
using System.IO;

namespace SilentHunter.Dat.Chunks
{
	public sealed class BodyParts3Chunk : DatChunk
	{
		public BodyParts3Chunk()
			: base(DatFile.Magics.BodyParts3)
		{
		}

		/// <summary>
		/// Gets or sets the part id.
		/// </summary>
		public override ulong Id
		{
			get => base.Id;
			set
			{
				if (value > uint.MaxValue)
				{
					throw new ArgumentOutOfRangeException(nameof(value), "The id for this part is only 4 bytes in length (UInt32).");
				}

				base.Id = value;
			}
		}

		/// <summary>
		/// Gets or sets the part its parent id.
		/// </summary>
		public override ulong ParentId
		{
			get => base.ParentId;
			set
			{
				if (value > uint.MaxValue)
				{
					throw new ArgumentOutOfRangeException(nameof(value), "The parent id for this part is only 4 bytes in length (UInt32).");
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

		/// <summary>
		/// Deserializes the chunk.
		/// </summary>
		/// <param name="stream">The stream to read from.</param>
		protected override Task DeserializeAsync(Stream stream)
		{
			using (var reader = new BinaryReader(stream, FileEncoding.Default, true))
			{
				Id = reader.ReadUInt32(); // Read an id as an uint.
				ParentId = reader.ReadUInt32(); // Read an id as an uint.
			}

			return base.DeserializeAsync(stream);
		}

		/// <summary>
		/// Serializes the chunk.
		/// </summary>
		/// <param name="stream">The stream to write to.</param>
		protected override Task SerializeAsync(Stream stream)
		{
			using (var writer = new BinaryWriter(stream, FileEncoding.Default, true))
			{
				writer.Write((uint)Id);
				writer.Write((uint)ParentId);
			}

			return base.SerializeAsync(stream);
		}
	}
}
#endif