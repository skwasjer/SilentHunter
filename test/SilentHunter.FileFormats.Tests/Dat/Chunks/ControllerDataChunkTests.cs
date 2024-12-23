using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using SilentHunter.FileFormats.ChunkedFiles;
using SilentHunter.FileFormats.Dat.Controllers;
using Xunit;

namespace SilentHunter.FileFormats.Dat.Chunks;

public class ControllerDataChunkTests
{
    private readonly Mock<IControllerReader> _controllerReaderMock;
    private readonly Mock<IControllerWriter> _controllerWriterMock;
    private readonly ControllerDataChunk _sut;

    public ControllerDataChunkTests()
    {
        _controllerReaderMock = new Mock<IControllerReader>();
        _controllerWriterMock = new Mock<IControllerWriter>();

        _sut = new ControllerDataChunk(
            _controllerReaderMock.Object,
            _controllerWriterMock.Object
        );
    }

    [Fact]
    public void Should_not_support_id()
    {
        ulong id = unchecked((ulong)DateTime.Now.Ticks);

        // Act
        Action act = () =>
            new ControllerDataChunk(
                _controllerReaderMock.Object,
                _controllerWriterMock.Object
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
                _controllerReaderMock.Object,
                _controllerWriterMock.Object
            ).ParentId = id;

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void When_creating_new_instance_should_set_defaults()
    {
        var newInstance = new ControllerDataChunk(
            _controllerReaderMock.Object,
            _controllerWriterMock.Object
        );
        var compareToInstance = new ControllerDataChunk(
            _controllerReaderMock.Object,
            _controllerWriterMock.Object
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
        byte[] fakeControllerData = { 0x20, 0x6c, 0x61, 0x62 };
        byte[] expectedRawData = new byte[] { 0x7b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }
            .Concat(fakeControllerData)
            .ToArray();

        _controllerWriterMock
            .Setup(m => m.Write(It.IsAny<Stream>(), It.IsAny<object>()))
            .Callback<Stream, object>((ms, _) => ms.Write(fakeControllerData, 0, fakeControllerData.Length));

        var controller = new object();
        var chunk = new ControllerDataChunk(
            _controllerReaderMock.Object,
            _controllerWriterMock.Object
        )
        {
            ParentId = 123,
            ControllerData = controller
        };

        using (var ms = new MemoryStream())
        {
            // Act
            await chunk.SerializeAsync(ms, false);

            // Assert
            ms.ToArray().Should().BeEquivalentTo(expectedRawData);
            _controllerWriterMock.Verify(m => m.Write(ms, controller), Times.Once);
        }
    }

    [Fact]
    public async Task Given_controller_data_is_not_accessed_when_serializing_should_rewrite_from_buffer()
    {
        byte[] fakeControllerData = { 0x20, 0x6c, 0x61, 0x62 };
        byte[] rawData = new byte[] { 0x7b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }
            .Concat(fakeControllerData)
            .ToArray();

        using (var ms = new MemoryStream(rawData))
        {
            // Act
            await _sut.DeserializeAsync(ms, false);
            ms.SetLength(0);

            await _sut.SerializeAsync(ms, false);

            // Assert
            ms.ToArray().Should().BeEquivalentTo(rawData);
            _controllerWriterMock.Verify(m => m.Write(It.IsAny<Stream>(), It.IsAny<object>()), Times.Never);
        }
    }

    [Fact]
    public async Task Given_controller_data_when_accessing_property_should_use_reader()
    {
        byte[] rawData = { 0x7b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        var controller = new object();
        _controllerReaderMock
            .Setup(m => m.Read(It.IsAny<Stream>(), It.IsAny<string>()))
            .Returns(controller);

        using (var ms = new MemoryStream(rawData))
        {
            // Act
            await _sut.DeserializeAsync(ms, false);
            ms.SetLength(0);

            // Assert
            _sut.ControllerData.Should().BeSameAs(controller);
            _controllerReaderMock.Verify(m => m.Read(It.IsAny<Stream>(), null), Times.Once);
        }
    }

    [Fact]
    public async Task Given_controller_data_has_been_accessed_through_property_when_serializing_should_use_writer()
    {
        byte[] rawData = { 0x7b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        var controller = new object();
        _controllerReaderMock
            .Setup(m => m.Read(It.IsAny<Stream>(), It.IsAny<string>()))
            .Returns(controller);

        using (var ms = new MemoryStream(rawData))
        {
            await _sut.DeserializeAsync(ms, false);
            object ctrl = _sut.ControllerData;
            ms.SetLength(0);

            // Act
            await _sut.SerializeAsync(ms, false);

            // Assert
            _controllerWriterMock.Verify(m => m.Write(It.IsAny<Stream>(), ctrl), Times.Once);
        }
    }

    [Fact]
    public async Task Given_controller_data_has_been_accessed_before_when_accessing_again_should_return_cached_instance()
    {
        byte[] rawData = { 0x7b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        var controller = new object();
        _controllerReaderMock
            .Setup(m => m.Read(It.IsAny<Stream>(), It.IsAny<string>()))
            .Returns(controller);

        using (var ms = new MemoryStream(rawData))
        {
            await _sut.DeserializeAsync(ms, false);
            // ReSharper disable once RedundantAssignment
            object ctrl = _sut.ControllerData;
            _controllerWriterMock.Reset();

            // Act
            ctrl = _sut.ControllerData;

            // Assert
            _controllerWriterMock.Verify(m => m.Write(It.IsAny<Stream>(), ctrl), Times.Never);
        }
    }

    [Fact]
    public async Task Given_controller_data_cannot_be_deserialized_when_accessing_property_should_return_byte_array()
    {
        byte[] rawData = { 0x7b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        byte[] unsupportedController = { 0x20, 0x6c, 0x61, 0x62 };
        _controllerReaderMock
            .Setup(m => m.Read(It.IsAny<Stream>(), It.IsAny<string>()))
            .Returns(unsupportedController);

        using (var ms = new MemoryStream(rawData))
        {
            await _sut.DeserializeAsync(ms, false);

            // Act
            object ctrl = _sut.ControllerData;

            // Assert
            ctrl.Should()
                .BeOfType<byte[]>()
                .And.BeSameAs(unsupportedController);
            _controllerReaderMock.Verify(m => m.Read(It.IsAny<Stream>(), It.IsAny<string>()), Times.Once);
        }
    }

    [Fact]
    public async Task Given_controller_data_is_not_yet_deserialized_when_setting_new_controller_should_write_new_controller()
    {
        byte[] rawData = { 0x7b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        var newController = new object();

        using (var ms = new MemoryStream(rawData))
        {
            await _sut.DeserializeAsync(ms, false);
            ms.SetLength(0);

            // Act
            _sut.ControllerData = newController;
            await _sut.SerializeAsync(ms, false);

            // Assert
            _controllerReaderMock.Verify(m => m.Read(It.IsAny<Stream>(), It.IsAny<string>()), Times.Never);
            _controllerWriterMock.Verify(m => m.Write(It.IsAny<Stream>(), newController), Times.Once);
        }
    }

    [Fact]
    public async Task Given_parent_controller_chunk_exists_when_deserializing_controller_data_should_use_name_as_hint()
    {
        byte[] rawData = { 0x7b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        var newController = new object();

        var parentFileChunks = new[]
        {
            new ControllerChunk
            {
                Id = 123,
                Name = "I have the same id, but don't match me!"
            },
            new ControllerChunk
            {
                Id = 123,
                Name = "Match me!"
            },
            new ControllerChunk
            {
                Id = 1234,
                Name = "I have a different id"
            }
        };

        var parentFileMock = new Mock<IChunkFile>();
        parentFileMock
            .Setup(m => m.Chunks)
            .Returns(parentFileChunks);
        _sut.ParentFile = parentFileMock.Object;

        using (var ms = new MemoryStream(rawData))
        {
            await _sut.DeserializeAsync(ms, false);

            // Act
            object ctrl = _sut.ControllerData;

            // Assert
            _controllerReaderMock.Verify(m => m.Read(It.IsAny<Stream>(), "Match me!"), Times.Once);
        }
    }
}