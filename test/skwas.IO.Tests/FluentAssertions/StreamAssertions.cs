using System.IO;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace skwas.IO.FluentAssertions
{
	internal class StreamAssertions : ReferenceTypeAssertions<Stream, StreamAssertions>
	{
		public StreamAssertions(Stream stream)
		{
			Subject = stream;
		}

		protected override string Identifier => "stream";

		public AndConstraint<Stream> BeEof(string because = null, params object[] reasonArgs)
		{
			Execute.Assertion
				.BecauseOf(because, reasonArgs)
				.ForCondition(Subject.Position == Subject.Length)
				.FailWith("Expected stream to be at the end {reason}, but it wasn't.");

			return new AndConstraint<Stream>(Subject);
		}
	}
}