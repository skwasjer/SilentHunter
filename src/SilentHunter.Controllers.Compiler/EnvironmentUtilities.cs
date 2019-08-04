using System.Reflection;
using System.Runtime.Versioning;

namespace SilentHunter.Controllers.Compiler
{
	internal class EnvironmentUtilities
	{
		/// <summary>
		/// Gets the current target framework (ie.: net452, netstandard2.0, etc)
		/// </summary>
		/// <returns></returns>
		public static string GetCurrentTargetFramework()
		{
			Assembly assembly = Assembly.GetCallingAssembly();

			return GetTargetFramework(assembly);
		}

		private static string GetTargetFramework(Assembly assembly)
		{
			string frameworkVersion = assembly
				.GetCustomAttribute<TargetFrameworkAttribute>()
				.FrameworkName.ToLowerInvariant()
				.Replace(",version=v", string.Empty)
				.TrimStart('.');

			if (frameworkVersion.StartsWith("netframework"))
			{
				frameworkVersion = frameworkVersion
					.Replace("netframework", "net")
					.Replace(".", string.Empty);
			}

			return frameworkVersion;
		}
	}
}
