#if DEBUG
using System;
using System.IO;

namespace SilentHunter.Dat.Chunks
{
	public sealed class BodyParts2 : DatChunk
	{
		public BodyParts2()
			: base(DatFile.Magics.BodyParts2)
		{
		}

		/// <summary>
		/// Gets or sets the part id.
		/// </summary>
		public override ulong Id
		{
			get { return base.Id; }
			set
			{
				if (value > uint.MaxValue)
					throw new ArgumentOutOfRangeException("The id for this chunk is only 4 bytes in length (UInt32).");
				base.Id = value;
			}
		}

		/// <summary>
		/// Gets or sets the part its parent id.
		/// </summary>
		public override ulong ParentId
		{
			get { return base.ParentId; }
			set
			{
				if (value > uint.MaxValue)
					throw new ArgumentOutOfRangeException("The parent id for this chunk is only 4 bytes in length (UInt32).");
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
		protected override void Deserialize(Stream stream)
		{
			using (var reader = new BinaryReader(stream, Encoding.ParseEncoding, true))
			{
				Id = reader.ReadUInt32(); // Read an id as an uint.
				ParentId = reader.ReadUInt32(); // Read an id as an uint.
			}

			base.Deserialize(stream);
		}

		/// <summary>
		/// Serializes the chunk.
		/// </summary>
		/// <param name="stream">The stream to write to.</param>
		protected override void Serialize(Stream stream)
		{
			using (var writer = new BinaryWriter(stream, Encoding.ParseEncoding, true))
			{
				writer.Write((uint) Id);
				writer.Write((uint) ParentId);
			}

			base.Serialize(stream);
		}
	}
}
#endif