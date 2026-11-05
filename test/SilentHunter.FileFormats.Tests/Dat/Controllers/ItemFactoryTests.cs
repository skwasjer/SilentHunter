using System;
using Microsoft.Extensions.DependencyInjection;
using SilentHunter.Controllers;
using SilentHunter.FileFormats.Fixtures;

namespace SilentHunter.FileFormats.Dat.Controllers;

[Collection(nameof(CompiledControllers))]
public class ItemFactoryTests
{
    private readonly IControllerFactory _controllerFactory;
    private readonly IItemFactory _itemFactory;
    private readonly Controller _particleGenerator;

    public ItemFactoryTests(CompiledControllersFixture compiledControllers)
    {
        CompiledControllersFixture compiledControllers1 = compiledControllers ?? throw new ArgumentNullException(nameof(compiledControllers));
        _controllerFactory = compiledControllers1.ServiceProvider.GetRequiredService<IControllerFactory>();
        _itemFactory = (IItemFactory)_controllerFactory;
        _particleGenerator = _controllerFactory.CreateController("ParticleGenerator", ControllerProfile.SH4, true);
    }

    [Fact]
    public void CreateNewItem_should_create_identical_copies_if_created_by_type_or_clone()
    {
        Type opType = _particleGenerator.GetType().GetField("ObjectParticles")?.FieldType.GetGenericArguments()[0];
        opType.Should().NotBeNull();

        // Act
        object createdFromType = _itemFactory.CreateNewItem(opType);
        object createdFromClone = _itemFactory.CreateNewItem(createdFromType);

        // Assert
        createdFromClone.Should()
            .BeEquivalentTo(createdFromType,
                opts => opts
                    .IncludingFields()
                    .RespectingRuntimeTypes());
    }

    [Fact]
    public void CreateNewItem_should_create_identical_copies_if_created_by_type()
    {
        Type opType = _particleGenerator.GetType().GetField("ObjectParticles")?.FieldType.GetGenericArguments()[0];
        opType.Should().NotBeNull();

        // Act
        object createdFromType1 = _itemFactory.CreateNewItem(opType);
        object createdFromType2 = _itemFactory.CreateNewItem(opType);

        // Assert
        createdFromType1.Should()
            .BeEquivalentTo(createdFromType2,
                opts => opts
                    .IncludingFields()
                    .RespectingRuntimeTypes());
    }

    [Fact]
    public void CreateNewItem_should_create_identical_copies_if_created_by_clone()
    {
        Type opType = _particleGenerator.GetType().GetField("ObjectParticles")?.FieldType.GetGenericArguments()[0];
        opType.Should().NotBeNull();
        object createdFromType = _itemFactory.CreateNewItem(opType);

        // Act
        object createdFromClone1 = _itemFactory.CreateNewItem(createdFromType);
        object createdFromClone2 = _itemFactory.CreateNewItem(createdFromType);

        // Assert
        createdFromClone1.Should()
            .BeEquivalentTo(createdFromClone2,
                opts => opts
                    .IncludingFields()
                    .RespectingRuntimeTypes());
    }
}
