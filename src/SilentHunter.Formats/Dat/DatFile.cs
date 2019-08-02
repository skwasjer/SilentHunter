using System;
using System.IO;
using SilentHunter.Dat.Chunks;
using skwas.IO;

namespace SilentHunter.Dat
{
	public sealed partial class DatFile : ChunkFile<DatChunk>, ISilentHunterFile
	{
		private readonly IChunkResolver<Magics> _chunkResolver;
		private readonly IChunkActivator _chunkActivator;

		/// <summary>
		/// Every DAT-file starts with this magic.
		/// </summary>
		public const uint Magic = 0x716d0da4;

		private Header _header;

		public DatFile(IChunkResolver<Magics> chunkResolver, IChunkActivator chunkActivator)
		{
			_chunkResolver = chunkResolver ?? throw new ArgumentNullException(nameof(chunkResolver));
			_chunkActivator = chunkActivator ?? throw new ArgumentNullException(nameof(chunkActivator));
			_header = new Header();

			HasGenericFlag = true;
		}

		~DatFile()
		{
			Dispose(false);
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (!IsDisposed)
				{
					if (disposing)
					{
						_header = null;
					}
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		public uint HeaderFlags
		{
			get => (uint)_header.FileType;
			set => _header.FileType = (Header.Flags)value;
		}

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
		/// Loads the file from specified stream.
		/// </summary>
		/// <param name="stream">The stream to load from.</param>
		public override void Load(Stream stream)
		{
			if (IsDisposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}

			// Check the magic.
			using (var reader = new BinaryReader(stream, FileEncoding.Default, true))
			{
				_header = reader.ReadStruct<Header>();
				if (!_header.IsValid())
				{
					throw new ArgumentException("Unexpected header in stream.", nameof(stream));
				}
			}

			using (ChunkReader<Magics, DatChunk> reader = CreateReader(stream))
			{
				long chunkStart = 0;
				try
				{
					while (true)
					{
						chunkStart = stream.Position;
						DatChunk chunk = reader.Read();
						if (chunk == null)
						{
							// EOF.
							break;
						}

						// S3D stores a special chunk that has no purpose anymore, so we get rid of it.
						if (chunk.Magic == Magics.S3DSettings)
						{
							chunk.Dispose();
						}
						else
						{
							Chunks.Add(chunk);
						}
					}
				}
				catch (Exception ex) when (chunkStart != 0)
				{
					throw new DatFileException(Chunks.Count, chunkStart, stream.Position, ex);
				}
			}
		}

		/// <summary>
		/// Saves the file to specified stream.
		/// </summary>
		/// <param name="stream">The stream to write to.</param>
		public override void Save(Stream stream)
		{
			if (IsDisposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}

			using (var writer = new BinaryWriter(stream, FileEncoding.Default, true))
			{
				writer.WriteStruct(_header);
			}

			using (ChunkWriter<Magics, DatChunk> writer = CreateWriter(stream))
			{
				foreach (DatChunk chunk in Chunks)
				{
					writer.Write(chunk);
				}
			}
		}

		public ChunkReader<Magics, DatChunk> CreateReader(Stream stream)
		{
			return new ChunkReader<Magics, DatChunk>(stream, _chunkResolver, _chunkActivator, true);
		}

		public ChunkWriter<Magics, DatChunk> CreateWriter(Stream stream)
		{
			return new ChunkWriter<Magics, DatChunk>(stream, true);
		}

		/// <summary>
		/// When implemented, deserializes the implemented class from specified <paramref name="stream" />.
		/// </summary>
		/// <param name="stream">The stream.</param>
		void IRawSerializable.Deserialize(Stream stream)
		{
			Load(stream);
		}

		/// <summary>
		/// When implemented, serializes the implemented class to specified <paramref name="stream" />.
		/// </summary>
		/// <param name="stream">The stream.</param>
		void IRawSerializable.Serialize(Stream stream)
		{
			Save(stream);
		}
	}
}