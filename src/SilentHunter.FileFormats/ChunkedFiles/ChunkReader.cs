using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using SilentHunter.FileFormats.Extensions;

namespace SilentHunter.FileFormats.ChunkedFiles
{
	/// <summary>
	/// Represents a reader with which <see cref="IChunk" />'s can be read from a stream.
	/// </summary>
	/// <typeparam name="TMagic">The type of the magic.</typeparam>
	/// <typeparam name="TChunk">The type of the chunk.</typeparam>
	public class ChunkReader<TMagic, TChunk> : IDisposable
		where TMagic : struct
		where TChunk : IChunk
	{
		private readonly IChunkActivator _chunkActivator;
		private readonly IChunkResolver _chunkResolver;
		private BinaryReader _reader;

		/// <summary>
		/// Initializes a new instance of <see cref="ChunkReader{TMagic,TChunk}" />.
		/// </summary>
		/// <param name="stream">The stream to read from.</param>
		/// <param name="chunkResolver">The resolver to use to find the type associated with the magic.</param>
		/// <param name="chunkActivator">The chunk activator used to create chunk instances.</param>
		/// <param name="leaveOpen">True to leave the stream open.</param>
		public ChunkReader(Stream stream, IChunkResolver chunkResolver, IChunkActivator chunkActivator, bool leaveOpen)
		{
			if (stream == null)
			{
				throw new ArgumentNullException(nameof(stream));
			}

			_reader = new BinaryReader(stream, Encoding.Default, leaveOpen);
			_chunkResolver = chunkResolver ?? throw new ArgumentNullException(nameof(chunkResolver));
			_chunkActivator = chunkActivator ?? throw new ArgumentNullException(nameof(chunkActivator));
		}

		/// <summary>
		/// Clean up any remaining resources.
		/// </summary>
		~ChunkReader()
		{
			Dispose(false);
		}

		/// <summary>
		/// Release managed and unmanaged resources.
		/// </summary>
		/// <param name="disposing">True if disposing.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (IsDisposed)
			{
				return;
			}

			if (disposing)
			{
				// Dispose managed.
				_reader?.Dispose();
				_reader = null;
			}
			// Dispose unmanaged.

			IsDisposed = true;
		}

		/// <summary>
		/// Releases all resources associated with this <see cref="ChunkReader{TMagic,TChunk}" /> object.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Gets whether the object is disposed.
		/// </summary>
		protected bool IsDisposed { get; private set; }

		/// <summary>
		/// Gets the underlying stream.
		/// </summary>
		public virtual Stream BaseStream => _reader.BaseStream;

		/// <summary>
		/// Reads the magic from the stream.
		/// </summary>
		/// <returns></returns>
		public virtual TMagic ReadMagic()
		{
			if (IsDisposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}

			return (TMagic)_reader.ReadStruct(typeof(TMagic));
		}

		/// <summary>
		/// Reads the next chunk from the stream or returns null if no more chunks are available.
		/// </summary>
		/// <returns></returns>
		public virtual async Task<TChunk> ReadAsync()
		{
			if (IsDisposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}

			if (BaseStream.Position >= BaseStream.Length)
			{
				return default;
			}

			long offset = BaseStream.Position;
			// Read magic.
			TMagic magic = ReadMagic();

			Type chunkType = _chunkResolver.Resolve(magic) ?? typeof(TChunk);

			// First check for a .ctor with single param of type TMagic.
			var newChunk = (TChunk)_chunkActivator.Create(chunkType, magic);

			newChunk.Magic = magic;
			newChunk.FileOffset = offset;
			await ((IChunk<TMagic>)newChunk).DeserializeAsync(BaseStream).ConfigureAwait(false);
			return newChunk;
		}
	}
}