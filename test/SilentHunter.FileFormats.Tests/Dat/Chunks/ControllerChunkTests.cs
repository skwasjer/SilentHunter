using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using SilentHunter.Testing.FluentAssertions;
using Xunit;

namespace SilentHunter.FileFormats.Dat.Chunks;

public class ControllerChunkTests
{
    [Fact]
    public void Should_support_id()
    {
        ulong id = unchecked((ulong)DateTime.Now.Ticks);

        // Act
        Action act = () => new ControllerChunk().Id = id;

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Should_support_parent_id()
    {
        ulong id = unchecked((ulong)DateTime.Now.Ticks);

        // Act
        Action act = () => new ControllerChunk().ParentId = id;

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void When_creating_new_instance_should_set_defaults()
    {
        var newInstance = new ControllerChunk();
        var compareToInstance = new ControllerChunk
        {
            ParentFile = null,
            FileOffset = 0,
            Magic = DatFile.Magics.Controller,
            Id = 0,
            ParentId = 0,
            SubType = 0,
            Name = null
        };

        // Assert
        newInstance.Should().BeEquivalentTo(compareToInstance);
    }

    [Theory]
    [InlineData("Some Name")]
    [InlineData("")]
    [InlineData(null)]
    public async Task When_serializing_should_produce_correct_binary_data(string controllerName)
    {
        byte[] expectedRawData = new byte[] { 0xc8, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }
                .Concat(controllerName == null ? new byte[0] : FileEncoding.Default.GetBytes(controllerName + '\0'))
                .ToArray()
            ;
        var chunk = new ControllerChunk
        {
            Id = 456,
            ParentId = 123,
            Name = controllerName
        };

        using (var ms = new MemoryStream())
        {
            // Act
            await chunk.SerializeAsync(ms, false);

            // Assert
            ms.ToArray().Should().BeEquivalentTo(expectedRawData);
        }
    }

    [Theory]
    [InlineData("Some Name")]
    [InlineData("")]
    [InlineData(null)]
    public async Task When_deserializing_should_read_correctly(string controllerName)
    {
        byte[] rawData = new byte[] { 0xc8, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }
                .Concat(controllerName == null ? new byte[0] : FileEncoding.Default.GetBytes(controllerName + '\0'))
                .ToArray()
            ;
        var chunk = new ControllerChunk();

        using (var ms = new MemoryStream(rawData))
        {
            // Act
            await chunk.DeserializeAsync(ms, false);

            // Assert
            chunk.Id.Should().Be(456);
            chunk.ParentId.Should().Be(123);
            chunk.Name.Should().Be(controllerName);
        }
    }

    [Theory]
    [InlineData("my label")]
    [InlineData("")]
    [InlineData(null)]
    public async Task When_serializing_and_then_deserializing_should_produce_equivalent(string controllerName)
    {
        var chunk = new ControllerChunk
        {
            Id = 456,
            ParentId = 123,
            Name = controllerName
        };

        using (var ms = new MemoryStream())
        {
            await chunk.SerializeAsync(ms, false);
            ms.Position = 0;

            // Act
            var deserializedChunk = new ControllerChunk();
            await deserializedChunk.DeserializeAsync(ms, false);

            // Assert
            deserializedChunk.Should().BeEquivalentTo(chunk);
            ms.Should().BeEof();
        }
    }

    [Fact]
    public async Task Given_stream_contains_more_data_than_chunk_needs_should_advance_to_end()
    {
        var chunk = new ControllerChunk
        {
            Name = "ControllerName"
        };
        using (var ms = new MemoryStream())
        {
            await chunk.SerializeAsync(ms, false);
            // Add garbage to end.
            ms.Write(new byte[] { 0x1, 0x2 }, 0, 2);
            ms.Position = 0;

            // Act
            await chunk.DeserializeAsync(ms, false);

            // Assert
            ms.Should().BeEof();
        }
    }
}