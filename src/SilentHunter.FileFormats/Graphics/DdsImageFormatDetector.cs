using System;
using System.IO;

namespace SilentHunter.FileFormats.Graphics
{
	public class DdsImageFormatDetector : IImageFormatDetector
	{
		private const int DdsMagic = 0x20534444;

		public string GetImageFormat(Stream stream)
		{
			const int magicSize = 4;
			var buffer = new byte[magicSize];
			if (stream.Read(buffer, 0, magicSize) != magicSize)
			{
				return null;
			}

			int magic = BitConverter.ToInt32(buffer, 0);
			return magic == DdsMagic ? "dds" : null;
		}
	}
}