using System;
using System.Collections.Generic;
using System.IO;
using skwas.IO;

namespace SilentHunter.Dat.Chunks
{
	public sealed class BodyParts : DatChunk
	{
		public BodyParts()
			: base(DatFile.Magics.BodyParts)
		{
			Parts = new List<string>();
		}

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

		public List<string> Parts { get; }

		/// <summary>
		/// Gets whether the chunk supports an id field.
		/// </summary>
		public override bool SupportsId => true;

		/// <summary>
		/// Deserializes the chunk.
		/// </summary>
		/// <param name="stream">The stream to read from.</param>
		protected override void Deserialize(Stream stream)
		{
			using (var reader = new BinaryReader(stream, Encoding.ParseEncoding, true))
			{
				Id = reader.ReadUInt32(); // Read an id as an uint.

				Parts.Clear();

				var partCount = reader.ReadInt32();
				for (var i = 0; i < partCount; i++)
					Parts.Add(reader.ReadNullTerminatedString());
			}
		}

		/// <summary>
		/// Serializes the chunk.
		/// </summary>
		/// <param name="stream">The stream to write to.</param>
		protected override void Serialize(Stream stream)
		{
			using (var writer = new BinaryWriter(stream, Encoding.ParseEncoding, true))
			{
				writer.Write((uint) Id); // Write an id as an uint.

				writer.Write(Parts.Count);
				foreach (var partName in Parts)
				{
					writer.Write(partName, '\0');
				}
			}
		}
	}
}
