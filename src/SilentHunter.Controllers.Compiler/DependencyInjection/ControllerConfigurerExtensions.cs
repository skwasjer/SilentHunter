using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SilentHunter.FileFormats.Dat.Controllers;
using SilentHunter.FileFormats.DependencyInjection;

namespace SilentHunter.Controllers.Compiler.DependencyInjection;

/// <summary>
/// Extensions for <see cref="ControllerConfigurer"/>.
/// </summary>
public static class ControllerConfigurerExtensions
{
    /// <summary>
    /// Registers a <see cref="ControllerAssembly"/> based on dynamically compiled controllers.
    /// </summary>
    /// <param name="controllerConfigurer"></param>
    /// <param name="controllerPath">The path containing the controller templates.</param>
    /// <param name="assemblyName">Th assembly name to give this assembly.</param>
    /// <param name="applicationName">The application name (your app), used to separate the dynamic assembly from other applications using this library.</param>
    /// <param name="ignorePaths">A delegate to exclude specific source file paths. Typically not needed.</param>
    /// <param name="dependencySearchPaths">The paths where to search for dependencies. Typically not needed.</param>
    /// <returns></returns>
    public static SilentHunterParsersConfigurer CompileFrom(this ControllerConfigurer controllerConfigurer, string controllerPath, string assemblyName = null, string applicationName = null, Func<string, bool> ignorePaths = null, params string[] dependencySearchPaths)
    {
        AddCSharpCompiler(controllerConfigurer);

        return controllerConfigurer.FromAssembly(s =>
        {
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            string appName = applicationName ?? entryAssembly?.GetName().Name ?? "SilentHunter.Controllers";
            var assemblyCompiler = new ControllerAssemblyCompiler(s.GetRequiredService<ICSharpCompiler>(), appName, controllerPath)
            {
                AssemblyName = assemblyName,
                IgnoreDirs = ignorePaths,
                DependencySearchDirs = dependencySearchPaths
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