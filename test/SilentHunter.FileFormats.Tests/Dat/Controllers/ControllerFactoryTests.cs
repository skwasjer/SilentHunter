using System;
using Microsoft.Extensions.DependencyInjection;
using SilentHunter.Controllers;
using SilentHunter.FileFormats.Fixtures;

namespace SilentHunter.FileFormats.Dat.Controllers;

[Collection(nameof(CompiledControllers))]
public sealed class ControllerFactoryTests
{
    private readonly IControllerFactory _controllerFactory;

    public ControllerFactoryTests(CompiledControllersFixture compiledControllers)
    {
        CompiledControllersFixture compiledControllers1 = compiledControllers ?? throw new ArgumentNullException(nameof(compiledControllers));
        _controllerFactory = compiledControllers1.ServiceProvider.GetRequiredService<IControllerFactory>();
    }

    [Theory]
    [InlineData("ParticleGenerator")]
    [InlineData("SH3ZonesCtrl")]
    public void CreateController_should_produce_identical_copies(string controllerName)
    {
        Controller controller1 = _controllerFactory.CreateController(controllerName, ControllerProfile.SH4, true);
        Controller controller2 = _controllerFactory.CreateController(controllerName, ControllerProfile.SH4, true);

        // Assert
        controller1.Should()
            .BeEquivalentTo(controller2,
                opts => opts.IncludingFields().RespectingRuntimeTypes());
    }
}
