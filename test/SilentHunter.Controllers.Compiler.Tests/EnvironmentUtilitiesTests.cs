using FluentAssertions;
using Xunit;

namespace SilentHunter.Controllers.Compiler
{
	public class EnvironmentUtilitiesTests
	{
		[Fact]
		public void When_requesting_target_framework_should_return_correct_framework_and_version()
		{
			// Act
			string targetFramework = EnvironmentUtilities.GetCurrentTargetFramework();

			// Assert
#if NETFRAMEWORK
			targetFramework.Should().Be("net472");
#else
			targetFramework.Should().Be("netcoreapp8.0");
#endif
		}
	}
}