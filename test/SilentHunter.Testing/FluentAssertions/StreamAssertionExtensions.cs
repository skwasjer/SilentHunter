using System.IO;

namespace SilentHunter.Testing.FluentAssertions
{
	public static class StreamAssertionExtensions
	{
		public static StreamAssertions Should(this Stream stream)
		{
			return new StreamAssertions(stream);
		}
	}
}
