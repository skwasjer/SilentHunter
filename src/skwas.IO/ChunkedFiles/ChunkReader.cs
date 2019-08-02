using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace skwas.IO
{
	/// <summary>
	/// Represents a reader with which <see cref="IChunk" />'s can be read from a stream.
	/// </summary>
	/// <typeparam name="TMagic">The type of the magic.</typeparam>
	/// <typeparam name="TChunk">The type of the chunk.</typeparam>
	public class ChunkReader<TMagic, TChunk> : IDisposable
		where TChunk : IChunk
	{
		private readonly IServiceProvider _serviceProvider;
		private BinaryReader _reader;
		private IChunkResolver _chunkResolver;

		#region Implementation of IDisposable / .ctor

		/// <summary>
		/// Initializes a new instance of <see cref="ChunkReader{TMagic,TChunk}" />.
		/// </summary>
		/// <param name="stream">The stream to read from.</param>
		/// <param name="chunkResolver">The resolver to use to find the type associated with the magic.</param>
		/// <param name="serviceProvider">The service provider to use to create chunks</param>
		/// <param name="leaveOpen">True to leave the stream open.</param>
		public ChunkReader(Stream stream, IChunkResolver chunkResolver, IServiceProvider serviceProvider, bool leaveOpen)
			: this(stream, chunkResolver, leaveOpen)
		{
			_serviceProvider = serviceProvider;
		}

		/// <summary>
		/// Initializes a new instance of <see cref="ChunkReader{TMagic,TChunk}" />.
		/// </summary>
		/// <param name="stream">The stream to read from.</param>
		/// <param name="chunkResolver">The resolver to use to find the type associated with the magic.</param>
		/// <param name="leaveOpen">True to leave the stream open.</param>
		public ChunkReader(Stream stream, IChunkResolver chunkResolver, bool leaveOpen)
		{
			if (stream == null)
			{
				throw new ArgumentNullException(nameof(stream));
			}

			if (chunkResolver == null)
			{
				throw new ArgumentNullException(nameof(chunkResolver));
			}

			_reader = new BinaryReader(stream, Encoding.Default, leaveOpen);
			_chunkResolver = chunkResolver;
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

				_chunkResolver = null;
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

		#endregion

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
		public virtual TChunk Read()
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

			// If not a valuetype, check if the magic is null. This indicates that the stream is empty, and thus we have to exit.
			if (!typeof(TMagic).IsValueType && magic == null)
			{
				return default;
			}

			Type chunkType = _chunkResolver.Resolve(magic) ?? typeof(TChunk);

			// First check for a .ctor with single param of type TMagic.
			TChunk newChunk;
			if (_serviceProvider != null)
			{
				try
				{
					newChunk = (TChunk)ActivatorUtilities.CreateInstance(_serviceProvider, chunkType);
				}
				catch (InvalidOperationException)
				{
					newChunk = (TChunk)ActivatorUtilities.CreateInstance(_serviceProvider, chunkType, magic);
				}
			}
			else
			{
				ConstructorInfo constructor = chunkType.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(TMagic) }, null);
				if (constructor != null)
				{
					newChunk = (TChunk)Activator.CreateInstance(chunkType, magic);
				}
				else
				{
					// Try a parameterless constructor.
					newChunk = (TChunk)Activator.CreateInstance(chunkType);
				}
			}

			newChunk.Magic = magic;
			newChunk.FileOffset = offset;
			((IChunk<TMagic>)newChunk).Deserialize(BaseStream);
			return newChunk;
		}
	}
}