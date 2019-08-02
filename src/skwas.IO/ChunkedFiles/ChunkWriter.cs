using System;
using System.IO;
using System.Text;

namespace skwas.IO
{
	/// <summary>
	/// Represents a writer with which <see cref="IChunk" />'s can be written to a stream.
	/// </summary>
	/// <typeparam name="TMagic">The type of the magic.</typeparam>
	/// <typeparam name="TChunk">The type of the chunk.</typeparam>
	public class ChunkWriter<TMagic, TChunk> : IDisposable
		where TChunk : IChunk
	{
		private BinaryWriter _writer;

		/// <summary>
		/// Initializes a new instance of <see cref="ChunkWriter{TMagic,TChunk}" />.
		/// </summary>
		/// <param name="stream">The stream to write to.</param>
		/// <param name="leaveOpen">True to leave the stream open.</param>
		public ChunkWriter(Stream stream, bool leaveOpen)
		{
			if (stream == null)
			{
				throw new ArgumentNullException(nameof(stream));
			}

			_writer = new BinaryWriter(stream, Encoding.Default, leaveOpen);
		}

		/// <summary>
		/// Clean up any remaining resources.
		/// </summary>
		~ChunkWriter()
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
				_writer?.Dispose();
				_writer = null;
			}
			// Dispose unmanaged.

			IsDisposed = true;
		}

		/// <summary>
		/// Releases all resources associated with this <see cref="ChunkWriter{TMagic,TChunk}" /> object.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Gets whether the object is disposed.
		/// </summary>
		protected bool IsDisposed
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the underlying stream.
		/// </summary>
		public virtual Stream BaseStream => _writer.BaseStream;

		/// <summary>
		/// Writes the magic to the stream.
		/// </summary>
		/// <param name="magic">The magic.</param>
		public virtual void WriteMagic(TMagic magic)
		{
			if (IsDisposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}

			_writer.WriteStruct(magic);
		}

		/// <summary>
		/// Writes the chunk magic to the stream.
		/// </summary>
		/// <param name="chunk">The chunk to write the magic of.</param>
		public virtual void WriteMagic(TChunk chunk)
		{
			if (IsDisposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}

			_writer.WriteStruct(chunk.Magic);
		}

		/// <summary>
		/// Writes the specified <paramref name="chunk" /> to the stream.
		/// </summary>
		/// <param name="chunk">The chunk to write to stream.</param>
		public virtual void Write(TChunk chunk)
		{
			if (IsDisposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}

			chunk.FileOffset = BaseStream.Position;
			WriteMagic(chunk);

			((IChunk)chunk).Serialize(BaseStream);
		}
	}
}