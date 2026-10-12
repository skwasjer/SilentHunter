using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SilentHunter.FileFormats.ChunkedFiles;
using SilentHunter.FileFormats.Dat.Controllers;

namespace SilentHunter.FileFormats.Dat.Chunks;

public class ControllerDataChunkTests
{
    private readonly IControllerReader _controllerReaderMock;
    private readonly IControllerWriter _controllerWriterMock;
    private readonly ControllerDataChunk _sut;

    public ControllerDataChunkTests()
    {
        _controllerReaderMock = Substitute.For<IControllerReader>();
        _controllerWriterMock = Substitute.For<IControllerWriter>();

        _sut = new ControllerDataChunk(
            _controllerReaderMock,
            _controllerWriterMock
        );
    }

    [Fact]
    public void Should_not_support_id()
    {
        ulong id = unchecked((ulong)DateTime.Now.Ticks);

        // Act
        Action act = () =>
            new ControllerDataChunk(
                _controllerReaderMock,
                _controllerWriterMock
            ).Id = id;

        // Assert
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Should_support_parent_id()
    {
        ulong id = unchecked((ulong)DateTime.Now.Ticks);

        // Act
        Action act = () =>
            new ControllerDataChunk(
                _controllerReaderMock,
                _controllerWriterMock
            ).ParentId = id;

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void When_creating_new_instance_should_set_defaults()
    {
        var newInstance = new ControllerDataChunk(
            _controllerReaderMock,
            _controllerWriterMock
        );
        var compareToInstance = new ControllerDataChunk(
            _controllerReaderMock,
            _controllerWriterMock
        )
        {
            ParentFile = null,
            FileOffset = 0,
            Magic = DatFile.Magics.ControllerData,
            ParentId = 0,
            SubType = 0
        };

        // Assert
        newInstance.Should().BeEquivalentTo(compareToInstance);
    }

    [Fact]
    public async Task When_serializing_should_produce_correct_binary_data()
    {
        byte[] fakeControllerData = [0x20, 0x6c, 0x61, 0x62];
        byte[] expectedRawData = new byte[] { 0x7b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }
            .Concat(fakeControllerData)
            .ToArray();

        _controllerWriterMock
            .When(m => m.Write(Arg.Any<Stream>(), Arg.Any<object>()))
            .Do(ci =>
            {
                Stream ms = ci.Arg<Stream>();
                ms.Write(fakeControllerData, 0, fakeControllerData.Length);
            });

        object controller = new();
        var chunk = new ControllerDataChunk(
            _controllerReaderMock,
            _controllerWriterMock
        ) { ParentId = 123, ControllerData = controller };

        using (var ms = new MemoryStream())
        {
            // Act
            await chunk.SerializeAsync(ms, false);

            // Assert
            ms.ToArray().Should().BeEquivalentTo(expectedRawData);
            _controllerWriterMock.Received(1).Write(ms, controller);
        }
    }

    [Fact]
    public async Task Given_controller_data_is_not_accessed_when_serializing_should_rewrite_from_buffer()
    {
        byte[] fakeControllerData = [0x20, 0x6c, 0x61, 0x62];
        byte[] rawData = new byte[] { 0x7b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }
            .Concat(fakeControllerData)
            .ToArray();

        using var ms = new MemoryStream(rawData);
        // Act
        await _sut.DeserializeAsync(ms, false);
        ms.SetLength(0);

        await _sut.SerializeAsync(ms, false);

        // Assert
        ms.ToArray().Should().BeEquivalentTo(rawData);
        _controllerWriterMock.DidNotReceive().Write(Arg.Any<Stream>(), Arg.Any<object>());
    }

    [Fact]
    public async Task Given_controller_data_when_accessing_property_should_use_reader()
    {
        byte[] rawData = [0x7b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00];

        object controller = new();
        _controllerReaderMock
            .Read(Arg.Any<Stream>(), Arg.Any<string>())
            .Returns(controller);

        using var ms = new MemoryStream(rawData);
        // Act
        await _sut.DeserializeAsync(ms, false);
        ms.SetLength(0);

        // Assert
        _sut.ControllerData.Should().BeSameAs(controller);
        _controllerReaderMock.Received(1).Read(Arg.Any<Stream>(), null);
    }

    [Fact]
    public async Task Given_controller_data_has_been_accessed_through_property_when_serializing_should_use_writer()
    {
        byte[] rawData = [0x7b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00];

        object controller = new();
        _controllerReaderMock
            .Read(Arg.Any<Stream>(), Arg.Any<string>())
            .Returns(controller);

        using var ms = new MemoryStream(rawData);
        await _sut.DeserializeAsync(ms, false);
        object ctrl = _sut.ControllerData;
        ms.SetLength(0);

        // Act
        await _sut.SerializeAsync(ms, false);

        // Assert
        _controllerWriterMock.Received(1).Write(ms, ctrl);
    }

    [Fact]
    public async Task Given_controller_data_has_been_accessed_before_when_accessing_again_should_return_cached_instance()
    {
        byte[] rawData = [0x7b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00];

        object controller = new();
        _controllerReaderMock
            .Read(Arg.Any<Stream>(), Arg.Any<string>())
            .Returns(controller);

        using var ms = new MemoryStream(rawData);
        await _sut.DeserializeAsync(ms, false);
        // ReSharper disable once RedundantAssignment
        object ctrl = _sut.ControllerData;
        _controllerWriterMock.ClearReceivedCalls();

        // Act
        ctrl = _sut.ControllerData;

        // Assert
        _controllerWriterMock.DidNotReceive().Write(Arg.Any<Stream>(), ctrl);
    }

    [Fact]
    public async Task Given_controller_data_cannot_be_deserialized_when_accessing_property_should_return_byte_array()
    {
        byte[] rawData = [0x7b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00];

        byte[] unsupportedController = [0x20, 0x6c, 0x61, 0x62];
        _controllerReaderMock
            .Read(Arg.Any<Stream>(), Arg.Any<string>())
            .Returns(unsupportedController);

        using var ms = new MemoryStream(rawData);
        await _sut.DeserializeAsync(ms, false);

        // Act
        object ctrl = _sut.ControllerData;

        // Assert
        ctrl.Should()
            .BeOfType<byte[]>()
            .And.BeSameAs(unsupportedController);
        _controllerReaderMock.Received(1).Read(Arg.Any<Stream>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Given_controller_data_is_not_yet_deserialized_when_setting_new_controller_should_write_new_controller()
    {
        byte[] rawData = [0x7b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00];
        object newController = new();

        using var ms = new MemoryStream(rawData);
        await _sut.DeserializeAsync(ms, false);
        ms.SetLength(0);

        // Act
        _sut.ControllerData = newController;
        await _sut.SerializeAsync(ms, false);

        // Assert
        _controllerReaderMock.DidNotReceive().Read(Arg.Any<Stream>(), Arg.Any<string>());
        _controllerWriterMock.Received(1).Write(ms, newController);
    }

    [Fact]
    public async Task Given_parent_controller_chunk_exists_when_deserializing_controller_data_should_use_name_as_hint()
    {
        byte[] rawData = [0x7b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00];
        object newController = new();

        ControllerChunk[] parentFileChunks = [new ControllerChunk { Id = 123, Name = "I have the same id, but don't match me!" }, new ControllerChunk { Id = 123, Name = "Match me!" }, new ControllerChunk { Id = 1234, Name = "I have a different id" }];

        _sut.ParentFile = Substitute.For<IChunkFile>();
        _sut.ParentFile.Chunks.Returns(parentFileChunks);

        using var ms = new MemoryStream(rawData);
        await _sut.DeserializeAsync(ms, false);

        // Act
        object ctrl = _sut.ControllerData;

        // Assert
        _controllerReaderMock.Received(1).Read(Arg.Any<Stream>(), "Match me!");
    }
}
