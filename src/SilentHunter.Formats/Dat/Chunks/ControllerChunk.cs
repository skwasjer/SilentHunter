using System.Diagnostics;
using System.IO;
using skwas.IO;

namespace SilentHunter.Dat.Chunks
{
	public sealed class ControllerChunk : DatChunk
	{
		public ControllerChunk()
			: base(DatFile.Magics.Controller)
		{
		}

		/// <summary>
		/// Gets or sets the controller name.
		/// </summary>
		public string Name { get; set; }

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
			using (var reader = new BinaryReader(stream, FileEncoding.Default, true))
			{
				// Read id and parent id.
				Id = reader.ReadUInt64();
				ParentId = reader.ReadUInt64();

				// Skip byte.
				byte alwaysZero = reader.ReadByte();
				Debug.Assert(alwaysZero == byte.MinValue, "Controllers: expecting byte=0.");

				// The rest of the stream holds the name + terminating zero.
				if (stream.Length > stream.Position)
				{
					Name = reader.ReadNullTerminatedString();
				}
			}
		}

		/// <summary>
		/// Serializes the chunk.
		/// </summary>
		/// <param name="stream">The stream to write to.</param>
		protected override void Serialize(Stream stream)
		{
			using (var writer = new BinaryWriter(stream, FileEncoding.Default, true))
			{
				// Write id and parent id.
				writer.Write(Id);
				writer.Write(ParentId);

				// Zero byte.
				writer.Write(byte.MinValue);

				// Write name + terminating zero.
				if (!string.IsNullOrEmpty(Name))
				{
					writer.Write(Name, '\0');
				}
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
			return base.ToString() + ": " + Name;
		}
	}
}