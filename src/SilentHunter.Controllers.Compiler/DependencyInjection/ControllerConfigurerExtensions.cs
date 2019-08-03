using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SilentHunter.FileFormats.Dat.Controllers;
using SilentHunter.FileFormats.DependencyInjection;

namespace SilentHunter.Controllers.Compiler.DependencyInjection
{
	public static class ControllerConfigurerExtensions
	{
		public static SilentHunterParsersConfigurer CompileFrom(this ControllerConfigurer controllerConfigurer, string controllerPath, string assemblyName = null, Func<string, bool> ignorePaths = null, params string[] dependencySearchPaths)
		{
			AddCSharpCompiler(controllerConfigurer);

			return controllerConfigurer.FromAssembly(s =>
			{
				Assembly entryAssembly = Assembly.GetEntryAssembly();
				string applicationName = entryAssembly?.GetName().Name ?? "SilentHunter.Controllers";
				var assemblyCompiler = new ControllerAssemblyCompiler(s.GetRequiredService<ICSharpCompiler>(), applicationName, controllerPath)
				{
					AssemblyName = assemblyName,
					IgnorePaths = ignorePaths,
					DependencySearchPaths = dependencySearchPaths
				};
				return new ControllerAssembly(assemblyCompiler.Compile());
			});
		}

		private static void AddCSharpCompiler(IServiceCollectionProvider controllerConfigurer)
		{
			IServiceCollection services = controllerConfigurer.ServiceCollection;
#if NETFRAMEWORK
			services.TryAddTransient<ICSharpCompiler, CSharpCompiler>();
#else
			services.TryAddTransient<ICSharpCompiler, RoslynCompiler>();
#endif
		}
	}
}