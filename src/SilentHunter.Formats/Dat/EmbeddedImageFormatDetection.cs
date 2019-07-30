using System;
using System.IO;
using System.Runtime.InteropServices;
using skwas.IO;

namespace SilentHunter.Dat
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	internal struct TGA_HEADER
	{
		/// <summary>
		/// Size of ID field that follows 18 byte header (0 usually).
		/// </summary>
		public byte identsize;

		/// <summary>Type of colour map 0=none, 1=has palette.</summary>
		public byte colormaptype;

		/// <summary>
		/// Type of image 0=none, 1=indexed, 2=rgb, 3=grey, 9=indexedRLE, 10=rgbRLE, 11=greyRLE.
		/// </summary>
		public byte imagetype;

		/// <summary>First colour map entry in palette.</summary>
		public short colormapstart;

		/// <summary>Number of colours in palette.</summary>
		public short colormaplength;

		/// <summary>Number of bits per palette entry 15, 16, 24, 32.</summary>
		public byte colormapbits;

		/// <summary>Image x origin.</summary>
		public short xstart;

		/// <summary>Image y origin.</summary>
		public short ystart;

		/// <summary>Image width in pixels.</summary>
		public short width;

		/// <summary>Image height in pixels.</summary>
		public short height;

		/// <summary>Image bits per pixel 8, 16, 24, 32</summary>
		public byte bits;

		/// <summary>Image descriptor bits (vh flip bits).</summary>
		public byte descriptor;

		public TgaPixelFormat PixelFormat => (TgaPixelFormat)(imagetype - (imagetype > (byte)8 ? 8 : 0));

		public bool HasColorMap => colormaptype == 1;
	}

	/// <summary>
	/// Specifies the format of the color data for each pixel in the image.
	/// </summary>
	internal enum TgaPixelFormat
	{
		/// <summary>No image data present.</summary>
		None,

		/// <summary>Indexed (using palette).</summary>
		Indexed,

		/// <summary>True color.</summary>
		Rgb,

		/// <summary>Greyscale.</summary>
		Greyscale
	}

	internal class EmbeddedImageFormatDetection
	{
		/// <summary>
		/// Validates stream contains valid TGA data. Returns true if <paramref name="fixCommonBugs"/> and repair was successful, or false if no repair was done. Throws, if invalid TGA data or repair was not successful.
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="fixCommonBugs"></param>
		/// <returns></returns>
		public static bool ValidateTgaStream(Stream stream, bool fixCommonBugs)
		{
			if (stream == null)
			{
				throw new ArgumentNullException();
			}

			if (!stream.CanSeek)
			{
				throw new ArgumentException("The stream does not support seeking.");
			}

			if (!stream.CanRead)
			{
				throw new ArgumentException("The stream does not support reading.");
			}

			long position = stream.Position;
			var reader = new BinaryReader(stream, System.Text.Encoding.ASCII);
			var tgaHeader = (TGA_HEADER)reader.ReadStruct(typeof(TGA_HEADER));
			if (tgaHeader.identsize > 0)
			{
				stream.Seek(tgaHeader.identsize, SeekOrigin.Current);
			}

			if (!Enum.IsDefined(typeof(TgaPixelFormat), tgaHeader.PixelFormat))
			{
				throw new Exception("Unknown image data type.");
			}

			if (tgaHeader.PixelFormat == TgaPixelFormat.Indexed)
			{
				if (!tgaHeader.HasColorMap)
				{
					throw new Exception("No color map is defined.");
				}

				reader.ReadBytes(tgaHeader.colormapbits / 8 * tgaHeader.colormaplength);
			}

			if (fixCommonBugs)
			{
				fixCommonBugs = false;
				if (!tgaHeader.HasColorMap && tgaHeader.PixelFormat != TgaPixelFormat.Indexed)
				{
					if (!stream.CanWrite)
					{
						throw new ArgumentException("Tga header contains a bug but can't be fixed since the stream does not support writing.");
					}

					if (tgaHeader.colormapstart != 0 || tgaHeader.colormaplength != 0)
					{
						stream.Position = position;
						tgaHeader.colormapstart = 0;
						tgaHeader.colormaplength = 0;
						new BinaryWriter(stream).WriteStruct(tgaHeader);
						fixCommonBugs = true;
					}
				}
			}

			stream.Position = position;
			switch (tgaHeader.bits)
			{
				case 8:
					if (tgaHeader.HasColorMap && tgaHeader.PixelFormat == TgaPixelFormat.Indexed || tgaHeader.PixelFormat == TgaPixelFormat.Greyscale)
					{
						return fixCommonBugs;
					}

					break;

				case 16:
				case 24:
				case 32:
					return fixCommonBugs;
			}

			throw new NotSupportedException("Stream does not appear to be a valid TGA stream.");
		}
	}
}