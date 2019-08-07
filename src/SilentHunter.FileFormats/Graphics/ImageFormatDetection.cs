using System;
using System.Collections.Generic;
using System.IO;

namespace SilentHunter.FileFormats.Graphics
{
	/// <summary>
	/// Provides a way to detect image format.
	/// </summary>
	public class ImageFormatDetection
	{
		/// <summary>
		/// Gets the default image format detection instance.
		/// </summary>
		public static ImageFormatDetection Default { get; } = new ImageFormatDetection(
			new IImageFormatDetector[]
			{
				new DdsImageFormatDetector(),
				new TgaImageFormatDetector(),
			}
		);

		/// <summary>
		/// Initializes a new instance of the <see cref="ImageFormatDetection"/> class using specified image detectors.
		/// </summary>
		/// <param name="detectors"></param>
		public ImageFormatDetection(IEnumerable<IImageFormatDetector> detectors)
		{
			Detectors = detectors ?? throw new ArgumentNullException(nameof(detectors));
		}

		/// <summary>
		/// Gets the chained detectors used.
		/// </summary>
		public IEnumerable<IImageFormatDetector> Detectors { get; }

		/// <summary>
		/// Gets the image format from image data on specified <paramref name="stream"/>.
		/// </summary>
		/// <param name="stream">The stream to read from.</param>
		/// <returns>The image format</returns>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="stream"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown when <paramref name="stream"/> is not readable or seekable.</exception>
		public ImageFormat GetFormat(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException(nameof(stream));
			}

			if (!stream.CanRead)
			{
				throw new ArgumentException("The stream does not support reading.", nameof(stream));
			}

			if (!stream.CanSeek)
			{
				throw new ArgumentException("The stream does not support seeking.", nameof(stream));
			}

			long currentPos = stream.Position;
			string formatStr = null;
			foreach (IImageFormatDetector detector in Detectors)
			{
				try
				{
					formatStr = detector.GetImageFormat(stream);
					if (formatStr != null)
					{
						break;
					}
				}
				finally
				{
					stream.Position = currentPos;
				}
			}

			Enum.TryParse(formatStr, true, out ImageFormat format);
			return format;
		}
	}
}