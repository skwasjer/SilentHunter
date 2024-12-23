using System;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SilentHunter.FileFormats.Dat.Controllers;

namespace SilentHunter.FileFormats.DependencyInjection;

/// <summary>
/// Configures how controller types are loaded.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public class ControllerConfigurer : IServiceCollectionProvider
{
    private readonly SilentHunterParsersConfigurer _configurer;
    private readonly IServiceCollection _serviceCollection;

    internal ControllerConfigurer(SilentHunterParsersConfigurer configurer)
    {
        _configurer = configurer;
        _serviceCollection = ((IServiceCollectionProvider)_configurer).ServiceCollection;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    IServiceCollection IServiceCollectionProvider.ServiceCollection { get => _serviceCollection; }

    /// <summary>
    /// Loads controller types from specified <paramref name="assembly" />.
    /// </summary>
    /// <param name="assembly">The assembly to load controller types from.</param>
    public SilentHunterParsersConfigurer FromAssembly(Assembly assembly)
    {
        return FromAssembly(new ControllerAssembly(assembly));
    }

    /// <summary>
    /// Loads controller types from specified <paramref name="controllerAssembly" />.
    /// </summary>
    /// <param name="controllerAssembly">The assembly to load controller types from.</param>
    public SilentHunterParsersConfigurer FromAssembly(ControllerAssembly controllerAssembly)
    {
        return FromAssembly(() => controllerAssembly);
    }

    /// <summary>
    /// Loads controller types from specified <paramref name="controllerAssembly" />.
    /// </summary>
    /// <param name="controllerAssembly">The assembly to load controller types from.</param>
    public SilentHunterParsersConfigurer FromAssembly(Func<ControllerAssembly> controllerAssembly)
    {
        return FromAssembly(_ => controllerAssembly());
    }

    /// <summary>
    /// Loads controller types from specified <paramref name="controllerAssembly" />.
    /// </summary>
    /// <param name="controllerAssembly">The assembly to load controller types from.</param>
    public SilentHunterParsersConfigurer FromAssembly(Func<IServiceProvider, ControllerAssembly> controllerAssembly)
    {
        _serviceCollection.AddSingleton(controllerAssembly);

        return _configurer;
    }
}
