#nullable enable
using System;
using Microsoft.Extensions.DependencyInjection;
using SilentHunter.Controllers;
using SilentHunter.FileFormats.Fixtures;

namespace SilentHunter.FileFormats.Dat.Controllers;

[Collection(nameof(CompiledControllers))]
public class ItemFactoryTests
{
    private readonly IControllerFactory _controllerFactory;
    private readonly IItemFactory _sut;
    private readonly Controller _particleGenerator;

    public ItemFactoryTests(CompiledControllersFixture compiledControllers)
    {
        CompiledControllersFixture compiledControllers1 = compiledControllers ?? throw new ArgumentNullException(nameof(compiledControllers));
        _controllerFactory = compiledControllers1.ServiceProvider.GetRequiredService<IControllerFactory>();
        _sut = (IItemFactory)_controllerFactory;
        _particleGenerator = _controllerFactory.CreateController("ParticleGenerator", ControllerProfile.SH4, true);
    }

    [Fact]
    public void CreateNewItem_should_create_identical_copies_if_created_by_type_or_clone()
    {
        Type? opType = _particleGenerator.GetType().GetField("ObjectParticles")?.FieldType.GetGenericArguments()[0];
        opType.Should().NotBeNull();

        // Act
        object createdFromType = _sut.CreateNewItem(opType);
        object createdFromClone = _sut.CreateNewItem(createdFromType);

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
        Type? opType = _particleGenerator.GetType().GetField("ObjectParticles")?.FieldType.GetGenericArguments()[0];
        opType.Should().NotBeNull();

        // Act
        object createdFromType1 = _sut.CreateNewItem(opType);
        object createdFromType2 = _sut.CreateNewItem(opType);

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
        Type? opType = _particleGenerator.GetType().GetField("ObjectParticles")?.FieldType.GetGenericArguments()[0];
        opType.Should().NotBeNull();
        object createdFromType = _sut.CreateNewItem(opType);

        // Act
        object createdFromClone1 = _sut.CreateNewItem(createdFromType);
        object createdFromClone2 = _sut.CreateNewItem(createdFromType);

        // Assert
        createdFromClone1.Should()
            .BeEquivalentTo(createdFromClone2,
                opts => opts
                    .IncludingFields()
                    .RespectingRuntimeTypes());
    }

    [Fact]
    public void CreateNewItem_should_throw_when_type_is_null()
    {
        Type? type = null;

        // Act
        Func<object?> act = () => _sut.CreateNewItem(type);

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithParameterName(nameof(type));
    }

    [Fact]
    public void CreateNewItem_should_return_empty_when_type_is_string()
    {
        _sut.CreateNewItem(typeof(string)).Should().BeOfType<string>().Which.Should().BeEmpty();
    }

    [Fact]
    public void CreateNewItem_should_return_null_when_original_is_null()
    {
        object? item = null;
        _sut.CreateNewItem(item).Should().BeNull();
    }

    [Theory]
    [InlineData("Particles.ParticleGenerator")]
    [InlineData("SH4.SHControllers.BumpMap")] // Empty
    [InlineData("BezierWaypoint.BezierWP")]
    [InlineData("Particles.EmissionArea")] // Struct
    [InlineData("Particles.RunMode")]
    [InlineData("SHCollisions.Box")] // SHUnion child.
    [InlineData("SHSim.TorpedoPistol")] // DateTime
    [InlineData("EnvSim.Rain")] // Color
    [InlineData("anim.AnimationObject")] // List
    [InlineData("Particles.ObjectParticles")]
    public void CreateNewItem_should_return_instance_when_type_is_ref_or_value_type_and_equal_clone(string typeName)
    {
        Type? type = _particleGenerator.GetType().Assembly.GetType(typeName);
        type.Should().NotBeNull();

        // Act
        object actual = _sut.CreateNewItem(type);

        // Assert
        actual.Should().NotBeNull().And.BeOfType(type);
        if (typeName.EndsWith("BumpMap"))
        {
            return; // No properties/fields to compare further.
        }

        // Act
        object clone = _sut.CreateNewItem(actual);

        // Assert
        clone.Should().BeEquivalentTo(actual,
            opts => opts
                .IncludingFields()
                .RespectingRuntimeTypes());

    }

    [Fact]
    public void CreateNewItem_should_create_identical_copies_for_union()
    {
        Type? boxType = _particleGenerator.GetType().Assembly.GetType("SHCollisions.Box");
        boxType.Should().NotBeNull();

        object box1 = _sut.CreateNewItem(boxType);
        box1.Should().NotBeNull();

        object box2 = _sut.CreateNewItem(box1);
        box2.Should().NotBeNull();

        box1.Should().BeEquivalentTo(box2,
            opts => opts
                .IncludingFields()
                .RespectingRuntimeTypes());

    }
}
