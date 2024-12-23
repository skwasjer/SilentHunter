using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SilentHunter.Controllers;
using SilentHunter.Testing.Extensions;
using SilentHunter.Testing.FluentAssertions;

namespace SilentHunter.FileFormats.Dat.Controllers.Serialization.ControllerSerializers;

public class StateMachineControllerSerializerTests
{
    private readonly StateMachineControllerSerializer _sut;
    private readonly StateMachineController _controller;
    private readonly byte[] _rawControllerData;

    public StateMachineControllerSerializerTests()
    {
        _sut = new StateMachineControllerSerializer();
        _controller = new Mock<StateMachineController>().Object;

        _rawControllerData = GetType().Assembly.GetManifestResourceStream(GetType(), "StateMachineController.chunkdata").ToArray();
    }

    [Fact]
    public void Given_invalid_controller_type_when_serializing_should_throw()
    {
        // Act
        Action act = () => _sut.Serialize(Stream.Null, new Mock<Controller>().Object);

        // Assert
        act.Should()
            .Throw<InvalidOperationException>()
            .WithMessage($"Expected controller of type {nameof(StateMachineController)}.");
    }

    [Fact]
    public void Given_invalid_controller_type_when_deserializing_should_throw()
    {
        // Act
        Action act = () => _sut.Deserialize(Stream.Null, new Mock<Controller>().Object);

        // Assert
        act.Should()
            .Throw<InvalidOperationException>()
            .WithMessage($"Expected controller of type {nameof(StateMachineController)}.");
    }

    [Fact]
    public void When_deserializing_should_populate_controller_with_correct_stateMachine()
    {
        string[] expectedEntryNames = ["Start", "Stalker_Idle", "Stalker_Idle_Finish", "Stalker_Idle_Finish_DIE", "Hunter_Idle", "Hunter_Idle_Finish", "Hunter_Idle_Finish_DIE", "Prey_Idle", "Prey_Idle_Finish", "Prey_Idle_Finish_DIE"];

        using var ms = new MemoryStream(_rawControllerData);
        // Act
        _sut.Deserialize(ms, _controller);

        // Assert
        _controller.GraphName.Should().Be("BMC01");
        _controller.StateEntries.Select(se => se.Name).Should().BeEquivalentTo(expectedEntryNames);
        ms.Should().BeEof();
    }

    [Fact]
    public void When_serializing_should_produce_same_byte_result()
    {
        using var ms = new MemoryStream(_rawControllerData);
        _sut.Deserialize(ms, _controller);
        ms.SetLength(0);

        // Act
        _sut.Serialize(ms, _controller);
        ms.ToArray().Should().BeEquivalentTo(_rawControllerData);
    }

    [Fact]
    public void Given_mesh_animation_controller_when_serializing_and_then_deserializing_should_give_equivalent()
    {
        _controller.GraphName = "MyGraphName";
        _controller.Unknown0 = 1;
        _controller.Unknown1 = 2;
        _controller.Unknown2 = 3;
        _controller.Unknown3 = 4;
        _controller.StateEntries =
        [
            new()
            {
                Name = "MyEntryName",
                Index = 0,
                Conditions =
                [
                    new()
                    {
                        ParentEntryIndex = 0,
                        Type = 123,
                        Value = "MyConditionValue",
                        Expression = "MyExpression",
                        GotoEntry = 20,
                        Actions =
                        [
                            new()
                            {
                                ParentEntryIndex = 0,
                                ParentConditionIndex = 0,
                                Name = "MyActionName",
                                Value = "MyActionValue"
                            }
                        ]
                    }
                ]
            }
        ];

        StateMachineController deserializedController = new Mock<StateMachineController>().Object;

        // Act
        using var ms = new MemoryStream();
        _sut.Serialize(ms, _controller);
        ms.Position = 0;

        _sut.Deserialize(ms, deserializedController);

        // Assert
        ms.Should().BeEof();
        _controller.Should().BeEquivalentTo(deserializedController);
    }
}
