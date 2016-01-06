using System;
using System.IO;
using skwas.IO;
using SilentHunter.Dat.Chunks;

namespace SilentHunter.Dat
{
	public sealed partial class DatFile 
		: ChunkFile<DatChunk>, ISilentHunterFile
	{
		/// <summary>
		/// Every DAT-file starts with this magic.
		/// </summary>
		public const uint Magic = 0x716d0da4;

		private Header _header;
		private S3DSettings _settings;

		public DatFile()
		{
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
						_settings?.Dispose();
						_settings = null;

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
			get { return (uint)_header.FileType; }
			set { _header.FileType = (Header.Flags)value; }
		}

		public bool HasGenericFlag
		{
			get { return _header.FileType.HasFlag(Header.Flags.Generic); }
			set
			{
				_header.FileType = _header.FileType | Header.Flags.Generic;
				if (!value)
					_header.FileType ^= Header.Flags.Generic;
			}
		}

		public bool ContainsAnimations
		{
			get { return _header.FileType.HasFlag(Header.Flags.HasAnimations); }
			set
			{
				_header.FileType = _header.FileType | Header.Flags.HasAnimations;
				if (!value)
					_header.FileType ^= Header.Flags.HasAnimations;
			}
		}

		public bool ContainsRenderableObjects
		{
			get { return _header.FileType.HasFlag(Header.Flags.HasRenderableObjects); }
			set
			{
				_header.FileType = _header.FileType | Header.Flags.HasRenderableObjects;
				if (!value)
					_header.FileType ^= Header.Flags.HasRenderableObjects;

			}
		}

		/// <summary>
		/// Gets a reference to the S3D settings chunk.
		/// </summary>
		private S3DSettings Settings
		{
			get
			{
				if (IsDisposed) throw new ObjectDisposedException(GetType().Name);

				return _settings ?? (_settings = new S3DSettings());
			}
		}

		#region Implementation of ISilentHunterFile

		/// <summary>
		/// Loads the file from specified stream.
		/// </summary>
		/// <param name="stream">The stream to load from.</param>
		public override void Load(Stream stream)
		{
			if (IsDisposed) throw new ObjectDisposedException(GetType().Name);

			_settings = null;

			var bufferedStream = new BufferedStream(stream, 1024);

			using (var reader = new BinaryReader(bufferedStream, Encoding.ParseEncoding, true))
				// Check the magic.
				_header = reader.ReadStruct<Header>();

			if (!_header.IsValid())
				throw new ArgumentException("Unexpected header in stream.", nameof(stream));

			using (var reader = new ChunkReader<Magics, DatChunk>(bufferedStream, new DatChunkResolver(), true))
			{
				long chunkStart = 0;
				try
				{
					while (true)
					{
						chunkStart = bufferedStream.Position;
						var chunk = reader.Read();
						if (chunk == null) break;   // EOF.

						// If we have a settings chunk, do not add it to collection. Instead, store a reference in a private variable. We don't want this special chunk to show up in the UI.
						if (chunk.Magic == Magics.S3DSettings)
							_settings = (S3DSettings)chunk;
						else
							Chunks.Add(chunk);
					}
				}
				catch (Exception ex) when (chunkStart != 0)
				{
					throw new DatFileException(Chunks.Count, chunkStart, bufferedStream.Position, ex);
				}
			}
		}

		/// <summary>
		/// Saves the file to specified stream.
		/// </summary>
		/// <param name="stream">The stream to write to.</param>
		public override void Save(Stream stream)
		{
			if (IsDisposed) throw new ObjectDisposedException(GetType().Name);

			using (var writer = new BinaryWriter(stream, Encoding.ParseEncoding, true))
				writer.WriteStruct(_header);

			using (var writer = new ChunkWriter<Magics, DatChunk>(stream, true))
			{
				foreach (var chunk in Chunks)
					// Write chunk.
					writer.Write(chunk);

				// Write settings.
				writer.Write(Settings);
			}
		}

		#endregion

		#region Implementation of IRawSerializable

		/// <summary>
		/// When implemented, deserializes the implemented class from specified <paramref name="stream"/>.
		/// </summary>
		/// <param name="stream">The stream.</param>
		void IRawSerializable.Deserialize(Stream stream)
		{
			Load(stream);
		}

		/// <summary>
		/// When implemented, serializes the implemented class to specified <paramref name="stream"/>.
		/// </summary>
		/// <param name="stream">The stream.</param>
		void IRawSerializable.Serialize(Stream stream)
		{
			Save(stream);
		}

		#endregion
	}
}
