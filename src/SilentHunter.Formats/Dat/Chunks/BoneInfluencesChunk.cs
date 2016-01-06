using System;
using System.Collections.Generic;
using System.IO;
using skwas.IO;

namespace SilentHunter.Dat.Chunks
{
	public sealed class BoneInfluencesChunk : DatChunk
	{
		public BoneInfluencesChunk()
			: base(DatFile.Magics.BoneInfluences)
		{
			WeightsAndIndices = new List<BoneInfluence>();
		}

		/// <summary>
		/// Gets or sets the chunk id.
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
		/// Gets or sets the chunk its parent id.
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

		public List<BoneInfluence> WeightsAndIndices { get; }

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

				WeightsAndIndices.Clear();

				var count = reader.ReadInt32();
				for (var i = 0; i < count; i++)
					WeightsAndIndices.Add(reader.ReadStruct<BoneInfluence>());
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
				writer.Write((uint) Id);
				writer.Write((uint) ParentId);

				writer.Write(WeightsAndIndices.Count);
				foreach (var wi in WeightsAndIndices)
					writer.WriteStruct(wi);
			}
		}
	}
}