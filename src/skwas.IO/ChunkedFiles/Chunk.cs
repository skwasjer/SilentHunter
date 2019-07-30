using System.IO;
using System.Text;

namespace skwas.IO
{
	/// <summary>
	/// Represents a file chunk, which is identified via a strongly typed magic.
	/// </summary>
	/// <typeparam name="TMagic">The type of the magic.</typeparam>
	public abstract class Chunk<TMagic> 
		: IChunk<TMagic>
	{
		private long _size;
		private byte[] _bytes;

		/// <summary>
		/// Initializes a new instance of <see cref="Chunk{TMagic}"/>.
		/// </summary>
		/// <param name="magic">The magic for this chunk.</param>
		protected Chunk(TMagic magic)
		{
			Magic = magic;
		}

		#region Implementation of IRawSerializable

		/// <summary>
		/// When implemented, deserializes the implemented class from specified <paramref name="stream"/>.
		/// </summary>
		/// <param name="stream">The stream.</param>
		void IRawSerializable.Deserialize(Stream stream)
		{
			Deserialize(stream);
		}

		/// <summary>
		/// When implemented, serializes the implemented class to specified <paramref name="stream"/>.
		/// </summary>
		/// <param name="stream">The stream.</param>
		void IRawSerializable.Serialize(Stream stream)
		{
			Serialize(stream);
		}

		#endregion

		#region Implementation of IChunk

		/// <summary>
		/// Gets or sets the magic.
		/// </summary>
		object IChunk.Magic
		{
			get { return Magic; }
			set { Magic = (TMagic)value; }
		}

		/// <summary>
		/// Gets or sets the magic.
		/// </summary>
		public TMagic Magic { get; set; }

		long IChunk.Size
		{
			get { return _size; }
			set { _size = value; }
		}

		/// <summary>
		/// Gets or sets the size of the chunk.
		/// </summary>
		public virtual long Size { get; }

		/// <summary>
		/// Gets or sets the file offset.
		/// </summary>
		public long FileOffset { get; set; }

		/// <summary>
		/// Gets or sets the parent file.
		/// </summary>
		public IChunkFile ParentFile { get; set; }

		#endregion

		#region Overrides of Object

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>
		/// A string that represents the current object.
		/// </returns>
		public override string ToString()
		{
			return string.Format("{0}: magic={1}, size={2}", GetType().Name, Magic, Size);
		}

		#endregion


		/// <summary>
		/// Gets array of raw chunk bytes. This array is only filled if inheritors did not fully implement deserialization.
		/// </summary>
		public byte[] Bytes => _bytes;

		/// <summary>
		/// When implemented, deserializes the implemented class from specified <paramref name="stream"/>.
		/// </summary>
		/// <param name="stream">The stream.</param>
		protected virtual void Deserialize(Stream stream)
		{
			using (var reader = new BinaryReader(stream, Encoding.Default, true))
			{
				_size = stream.Length - stream.Position;
				_bytes = reader.ReadBytes((int) _size);
			}
		}

		/// <summary>
		/// When implemented, serializes the implemented class to specified <paramref name="stream"/>.
		/// </summary>
		/// <param name="stream">The stream.</param>
		protected virtual void Serialize(Stream stream)
		{
			if ((_bytes == null) || (_bytes.Length == 0)) return;
			using (var writer = new BinaryWriter(stream, Encoding.Default, true))
				writer.Write(_bytes);
		}
	}
}
