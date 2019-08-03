using System.IO;

namespace SilentHunter.FileFormats.FluentAssertions
{
	internal static class StreamAssertionExtensions
	{
		public static StreamAssertions Should(this Stream stream)
		{
			return new StreamAssertions(stream);
		}
	}
}
