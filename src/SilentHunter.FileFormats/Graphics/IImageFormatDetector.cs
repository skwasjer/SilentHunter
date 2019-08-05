using System.IO;

namespace SilentHunter.FileFormats.Graphics
{
	/// <summary>
	/// Describes means to determine an image format.
	/// </summary>
	public interface IImageFormatDetector
	{
		/// <summary>
		/// Gets an image format identifier from the <paramref name="stream"/>.
		/// </summary>
		/// <param name="stream">The stream to get the image format from.</param>
		/// <returns>An image format identifier or <see langword="null"/> if not supported.</returns>
		string GetImageFormat(Stream stream);
	}
}