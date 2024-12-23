using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace SilentHunter.FileFormats.DependencyInjection;

/// <summary>
/// Configurer for Silent Hunter parser dependencies.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public class SilentHunterParsersConfigurer : IServiceCollectionProvider
{
    private readonly IServiceCollection _serviceCollection;

    internal SilentHunterParsersConfigurer(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;

        Controllers = new ControllerConfigurer(this);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    IServiceCollection IServiceCollectionProvider.ServiceCollection { get => _serviceCollection; }

    /// <summary>
    /// Configure how controller types are loaded.
    /// </summary>
    public ControllerConfigurer Controllers { get; }
}
