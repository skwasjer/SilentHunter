using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using SilentHunter.Controllers.Compiler.DependencyInjection;
using SilentHunter.FileFormats.DependencyInjection;

namespace SilentHunter.FileFormats.Fixtures;

[CollectionDefinition(nameof(CompiledControllers))]
public class CompiledControllers : ICollectionFixture<CompiledControllersFixture>
{
}

public class CompiledControllersFixture : IDisposable
{
    public CompiledControllersFixture()
    {
        string controllerPath = string.Join(Path.DirectorySeparatorChar.ToString(), "..", "..", "..", "..", "..", "src", "SilentHunter.Controllers");
        var services = new ServiceCollection();
        services.AddSilentHunterParsers(configurer => configurer
            .Controllers.CompileFrom(controllerPath, "Controllers", ignorePaths: f => f.StartsWith($"obj{Path.DirectorySeparatorChar}"))
        );

        ServiceProvider = services.BuildServiceProvider();
    }

    public IServiceProvider ServiceProvider { get; }

    public void Dispose()
    {
        ((IDisposable)ServiceProvider)?.Dispose();
    }
}