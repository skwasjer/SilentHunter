using System.IO;

namespace skwas.IO.FluentAssertions
{
	internal static class StreamAssertionExtensions
	{
		public static StreamAssertions Should(this Stream stream)
		{
			return new StreamAssertions(stream);
		}
	}
}
