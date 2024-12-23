using FluentAssertions;
using FluentAssertions.Streams;

namespace SilentHunter.Testing.FluentAssertions;

public static class StreamAssertionsExtensions
{
    public static AndConstraint<StreamAssertions> BeEof(this StreamAssertions assertions, string because = null, params object[] becauseArgs)
    {
        return assertions.HavePosition(assertions.Subject.Length, because, becauseArgs);
    }
}