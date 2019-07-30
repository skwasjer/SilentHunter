using System.IO;
using FluentAssertions;
using FluentAssertions.Execution;

namespace skwas.IO.Tests
{
	static class FluentExtensions
	{
		public static AndConstraint<Stream> ShouldBeEof(this Stream stream)
		{
			return ShouldBeEof(stream, string.Empty);
		}

		public static AndConstraint<Stream> ShouldBeEof(this Stream stream, string because, params object[] reasonArgs)
		{
			if (stream.Position != stream.Length)
				Execute.Assertion.BecauseOf(because, reasonArgs).FailWith("Expected stream to be at the end {reason}, but it wasn't.");
			return new AndConstraint<Stream>(stream);
		}
	}
}
