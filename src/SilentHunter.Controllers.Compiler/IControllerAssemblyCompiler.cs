using System.Reflection;

namespace SilentHunter.Controllers.Compiler;

/// <summary>
/// Describes an assembly compiler for controller templates.
/// </summary>
public interface IControllerAssemblyCompiler
{
    /// <summary>
    /// Gets the full controller path.
    /// </summary>
    string ControllerDir { get; }

    /// <summary>
    /// Gets the application name. This is used to determine the output location of the compiled assembly.
    /// </summary>
    string ApplicationName { get; }

    /// <summary>
    /// Compilers controller templates into an assembly.
    /// </summary>
    /// <param name="force"><see langword="true"/> to invalidate build cache, <see langword="false"/> to use build cache if one exists</param>
    /// <returns> The assembly with compiled controllers.</returns>
    Assembly Compile(bool force = false);
}