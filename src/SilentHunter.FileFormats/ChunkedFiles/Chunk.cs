using System.IO;
using System.Threading.Tasks;
using skwas.IO;

namespace SilentHunter.FileFormats.ChunkedFiles
{
	/// <summary>
	/// Represents a file chunk, which is identified via a strongly typed magic.
	/// </summary>
	/// <typeparam name="TMagic">The type of the magic.</typeparam>
	public abstract class Chunk<TMagic> : IChunk<TMagic>
	{
		private long _size;

		/// <summary>
		/// Initializes a new instance of <see cref="Chunk{TMagic}" />.
		/// </summary>
		/// <param name="magic">The magic for this chunk.</param>
		protected Chunk(TMagic magic)
		{
			Magic = magic;
		}

		/// <summary>
		/// When implemented, deserializes the implemented class from specified <paramref name="stream" />.
		/// </summary>
		/// <param name="stream">The stream.</param>
		Task IRawSerializable.DeserializeAsync(Stream stream)
		{
			return DeserializeAsync(stream);
		}

		/// <summary>
		/// When implemented, serializes the implemented class to specified <paramref name="stream" />.
		/// </summary>
		/// <param name="stream">The stream.</param>
		Task IRawSerializable.SerializeAsync(Stream stream)
		{
			return SerializeAsync(stream);
		}

		/// <summary>
		/// Gets or sets the magic.
		/// </summary>
		object IChunk.Magic
		{
			get => Magic;
			set => Magic = (TMagic)value;
		}

		/// <summary>
		/// Gets or sets the magic.
		/// </summary>
		public TMagic Magic { get; set; }

		long IChunk.Size
		{
			get => _size;
			set => _size = value;
		}

		/// <summary>
		/// Gets the size of the chunk.
		/// </summary>
		public virtual long Size => _size;

		/// <summary>
		/// Gets or sets the file offset.
		/// </summary>
		public long FileOffset { get; set; }

		/// <summary>
		/// Gets or sets the parent file.
		/// </summary>
		public IChunkFile ParentFile { get; set; }

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

		/// <summary>
		/// Gets array of raw chunk bytes. This array is only filled if inheritors did not fully implement deserialization.
		/// </summary>
		public byte[] Bytes
		{
			get;
			private set;
		}

		/// <summary>
		/// When implemented, deserializes the implemented class from specified <paramref name="stream" />.
		/// </summary>
		/// <param name="stream">The stream.</param>
		protected virtual Task DeserializeAsync(Stream stream)
		{
			_size = stream.Length - stream.Position;
			var buffer = new byte[_size];
			return stream.ReadAsync(buffer, 0, (int)_size);
		}

		/// <summary>
		/// When implemented, serializes the implemented class to specified <paramref name="stream" />.
		/// </summary>
		/// <param name="stream">The stream.</param>
		protected virtual Task SerializeAsync(Stream stream)
		{
			if (Bytes == null || Bytes.Length == 0)
			{
				return Task.CompletedTask;
			}

			return stream.WriteAsync(Bytes, 0, Bytes.Length);
		}
	}
}