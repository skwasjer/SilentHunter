using System;
using System.IO;
using System.Threading.Tasks;
using SilentHunter.FileFormats.ChunkedFiles;
using SilentHunter.FileFormats.Dat.Chunks;
using SilentHunter.FileFormats.Extensions;

namespace SilentHunter.FileFormats.Dat
{
	/// <summary>
	/// Represents an DAT/SIM/ZON/DSD/CAM/ANM file parser.
	/// </summary>
	public partial class DatFile : ChunkFile<DatChunk>, ISilentHunterFile
	{
		private readonly IChunkResolver<Magics> _chunkResolver;
		private readonly IChunkActivator _chunkActivator;

		/// <summary>
		/// Every DAT-file starts with this magic.
		/// </summary>
		public const uint Magic = 0x716d0da4;

		private Header _header;

		/// <summary>
		/// Initializes a new instance of the <see cref="DatFile"/> class using specified resolver and activator.
		/// </summary>
		public DatFile(IChunkResolver<Magics> chunkResolver, IChunkActivator chunkActivator)
		{
			_chunkResolver = chunkResolver ?? throw new ArgumentNullException(nameof(chunkResolver));
			_chunkActivator = chunkActivator ?? throw new ArgumentNullException(nameof(chunkActivator));
			_header = new Header();

			HasGenericFlag = true;
		}

		/// <summary>
		/// Gets or sets the raw header flags.
		/// </summary>
		public uint HeaderFlags
		{
			get => (uint)_header.FileType;
			set => _header.FileType = (Header.Flags)value;
		}

		/// <summary>
		/// Gets or sets the generic header flag.
		/// </summary>
		public bool HasGenericFlag
		{
			get => _header.FileType.HasFlag(Header.Flags.Generic);
			set
			{
				_header.FileType |= Header.Flags.Generic;
				if (!value)
				{
					_header.FileType ^= Header.Flags.Generic;
				}
			}
		}

		/// <summary>
		/// Gets or sets the 'contains animations' flag.
		/// </summary>
		public bool ContainsAnimations
		{
			get => _header.FileType.HasFlag(Header.Flags.HasAnimations);
			set
			{
				_header.FileType |= Header.Flags.HasAnimations;
				if (!value)
				{
					_header.FileType ^= Header.Flags.HasAnimations;
				}
			}
		}

		/// <summary>
		/// Gets or sets the 'contains renderable objects' flag.
		/// </summary>
		public bool ContainsRenderableObjects
		{
			get => _header.FileType.HasFlag(Header.Flags.HasRenderableObjects);
			set
			{
				_header.FileType |= Header.Flags.HasRenderableObjects;
				if (!value)
				{
					_header.FileType ^= Header.Flags.HasRenderableObjects;
				}
			}
		}

		/// <summary>
		/// Loads from specified <paramref name="stream"/>.
		/// </summary>
		/// <param name="stream">The stream to load from.</param>
		/// <exception cref="DatFileException">Thrown when a parsing error occurs.</exception>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="stream"/> is null.</exception>
		/// <exception cref="ObjectDisposedException">Thrown when the instance is disposed.</exception>
		public override async Task LoadAsync(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException(nameof(stream));
			}

			ThrowIfDisposed();

			// Check the magic.
			using (var reader = new BinaryReader(stream, FileEncoding.Default, true))
			{
				_header = reader.ReadStruct<Header>();
				if (!_header.IsValid())
				{
					throw new DatFileException(-1, 0, 0, "Unexpected header in stream.", null);
				}
			}

			using (ChunkReader<Magics, DatChunk> reader = CreateReader(stream))
			{
				long chunkStartPosition = 0;
				try
				{
					while (true)
					{
						chunkStartPosition = stream.Position;
						DatChunk chunk = await reader.ReadAsync().ConfigureAwait(false);
						if (chunk == null)
						{
							// EOF.
							break;
						}

						// S3D stores a special chunk that has no purpose anymore, so we get rid of it.
						if (chunk.Magic != Magics.S3DSettings)
						{
							Chunks.Add(chunk);
						}
					}
				}
				catch (Exception ex)
				{
					throw new DatFileException(Chunks.Count, chunkStartPosition, stream.Position, "The file could not be read due to a parser error.", ex);
				}
			}
		}

		/// <summary>
		/// Saves to specified <paramref name="stream"/>.
		/// </summary>
		/// <param name="stream">The stream to write to.</param>
		/// <exception cref="IOException">Thrown when an IO error occurs.</exception>
		/// <exception cref="SilentHunterParserException">Thrown when a parsing error occurs.</exception>
		/// <exception cref="DatFileException">Thrown when a parsing error occurs.</exception>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="stream"/> is null.</exception>
		/// <exception cref="ObjectDisposedException">Thrown when the instance is disposed.</exception>
		public override async Task SaveAsync(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException(nameof(stream));
			}

			ThrowIfDisposed();

			using (var writer = new BinaryWriter(stream, FileEncoding.Default, true))
			{
				writer.WriteStruct(_header);
			}

			using (ChunkWriter<Magics, DatChunk> writer = CreateWriter(stream))
			{
				int chunkIndex = -1;
				long chunkStartPosition = 0;
				try
				{
					foreach (DatChunk chunk in Chunks)
					{
						chunkIndex++;
						chunkStartPosition = stream.Position;
						await writer.WriteAsync(chunk).ConfigureAwait(false);
					}
				}
				catch (Exception ex)
				{
					throw new DatFileException(chunkIndex, chunkStartPosition, stream.Position, "The file could not be written due to a parser error.", ex);
				}
			}
		}

		private void ThrowIfDisposed()
		{
			if (IsDisposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}
		}

		/// <summary>
		/// Creates a new chunk using a <see cref="IChunkActivator"/>.
		/// </summary>
		/// <typeparam name="T">The chunk type.</typeparam>
		/// <returns>The created chunk instance.</returns>
		public virtual T CreateChunk<T>()
			where T : DatChunk
		{
			return (T)CreateChunk(typeof(T));
		}

		/// <summary>
		/// Creates a new chunk using a <see cref="IChunkActivator"/>.
		/// </summary>
		/// <param name="chunkType">The chunk type.</param>
		/// <returns>The created chunk instance.</returns>
		public virtual object CreateChunk(Type chunkType)
		{
			if (chunkType == null)
			{
				throw new ArgumentNullException(nameof(chunkType));
			}

			return _chunkActivator.Create(chunkType, null);
		}

		/// <summary>
		/// Creates a new chunk using a <see cref="IChunkActivator"/>.
		/// </summary>
		/// <param name="magic">The chunk magic.</param>
		/// <returns>The created chunk instance.</returns>
		public virtual object CreateChunk(Magics magic)
		{
			Type chunkType = _chunkResolver.Resolve(magic);
			if (chunkType == null)
			{
				throw new ArgumentException($"No chunk type registered for specified magic ({magic}).");
			}

			return CreateChunk(chunkType);
		}

		/// <summary>
		/// Creates a chunk reader for specified stream.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <returns>Returns the reader.</returns>
		public virtual ChunkReader<Magics, DatChunk> CreateReader(Stream stream)
		{
			return new ChunkReader<Magics, DatChunk>(stream, _chunkResolver, _chunkActivator, true);
		}

		/// <summary>
		/// Creates a chunk writer for specified stream.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <returns>Returns the writer.</returns>
		public virtual ChunkWriter<Magics, DatChunk> CreateWriter(Stream stream)
		{
			return new ChunkWriter<Magics, DatChunk>(stream, true);
		}
	}
}