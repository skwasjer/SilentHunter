using System;
using System.IO;
using System.Linq;

namespace SilentHunter.Dat
{
	public enum EmbeddedImageFormat
	{
		Unknown,
		Tga,
		Dds
	}

	public sealed class EmbeddedImage : DatChunk
	{
		public EmbeddedImage()
			: base(DatFile.Magics.EmbeddedImage)
		{
		}		

		private byte[] _buffer;

		public byte[] Buffer
		{
			get { return _buffer; }
			set
			{
				if (value != null)
				{
					// Determine image format.
					using (var ms = new MemoryStream(value))
					{
						ImageFormat = GetImageFormat(ms, true);						
					}
				}
				_buffer = value;
			}
		}

		/// <summary>
		/// Gets a MemoryStream containing the image data. Caller is responsible for disposing the MemoryStream.
		/// </summary>
		public MemoryStream GetStream()
		{
			return _buffer == null ? new MemoryStream() : new MemoryStream(_buffer);
		}

		/// <summary>
		/// Deserializes the chunk. 
		/// </summary>
		/// <param name="stream">The stream to read from.</param>
		protected override void Deserialize(Stream stream)
		{
			// Create a temporary stream so we can fix potential format problems (some TGA's have bugs).
			using (var ms = new MemoryStream((int)stream.Length))
			{
				stream.CopyTo(ms);
				// Restore stream position.
				ms.Position = 0;

				// Detect image format and fix bugs.
				ImageFormat = GetImageFormat(ms, true);

				_buffer = ms.ToArray();
			}
		}

		/// <summary>
		/// Serializes the chunk.
		/// </summary>
		/// <param name="stream">The stream to write to.</param>
		protected override void Serialize(Stream stream)
		{
			if (_buffer == null) return;

			stream.Write(_buffer, 0, _buffer.Length);
		}

		/// <summary>
		/// Gets the image format of the image.
		/// </summary>
		public EmbeddedImageFormat ImageFormat { get; private set; }

		// TODO: refactor this detection out of this class.
		public static EmbeddedImageFormat GetImageFormat(Stream stream)
		{
			return GetImageFormat(stream, false);
		}

		private static EmbeddedImageFormat GetImageFormat(Stream stream, bool fix)
		{
			var currentPos = stream.Position;

			var buffer = new byte[20];
			var bytesRead = stream.Read(buffer, 0, buffer.Length);

			stream.Position = currentPos;

			if (bytesRead != 20)
				return EmbeddedImageFormat.Unknown;

			// Validate first 20 bytes. If they are all 0, assume the file is invalid.
			if (!buffer.All(b => b == 0))
			{
				var magic = BitConverter.ToInt32(buffer, 0);
				if (magic == 0x20534444)
					return EmbeddedImageFormat.Dds;
				try
				{
					skwas.Drawing.TgaImage.ValidateStream(stream, stream.CanWrite && fix);
					return EmbeddedImageFormat.Tga;
				}
				catch
				{
				}
				finally
				{
					stream.Position = currentPos;
				}
			}

			return EmbeddedImageFormat.Unknown;
		}
	}
}
