using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using skwas.IO;

namespace SilentHunter.FileFormats.Dat.Chunks
{
	public sealed class PlacementChunk : DatChunk
	{
		public PlacementChunk()
			: base(DatFile.Magics.Placement)
		{
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
		/// Gets or sets the world offset.
		/// </summary>
		public Vector3 Offset { get; set; }

		/// <summary>
		/// Gets or sets the rotation (or well, I think it is rotation).
		/// </summary>
		public Vector3 Rotation { get; set; }

		/// <summary>
		/// Gets or sets the id of the node that this chunk defines placement for. This is usually an id in an external dat-file/library.
		/// </summary>
		public ulong NodeId { get; set; }

		/// <summary>
		/// Deserializes the chunk.
		/// </summary>
		/// <param name="stream">The stream to read from.</param>
		protected override Task DeserializeAsync(Stream stream)
		{
			using (var reader = new BinaryReader(stream, FileEncoding.Default, true))
			{
				Id = reader.ReadUInt64();
				ParentId = reader.ReadUInt64();

				NodeId = reader.ReadUInt64();

				Offset = reader.ReadStruct<Vector3>();
				Rotation = reader.ReadStruct<Vector3>();
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
				writer.Write(Id);
				writer.Write(ParentId);

				writer.Write(NodeId);

				writer.WriteStruct(Offset);
				writer.WriteStruct(Rotation);
			}

			return Task.CompletedTask;
		}
	}
}