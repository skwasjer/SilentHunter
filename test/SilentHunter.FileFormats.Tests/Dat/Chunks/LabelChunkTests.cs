using System;
using System.IO;
using System.Threading.Tasks;
using SilentHunter.Testing.FluentAssertions;

namespace SilentHunter.FileFormats.Dat.Chunks;

public class LabelChunkTests
{
    [Fact]
    public void Should_not_support_id()
    {
        ulong id = unchecked((ulong)DateTime.Now.Ticks);

        // Act
        Action act = () => new LabelChunk().Id = id;

        // Assert
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Should_support_parent_id()
    {
        ulong id = unchecked((ulong)DateTime.Now.Ticks);

        // Act
        Action act = () => new LabelChunk().ParentId = id;

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void When_creating_new_instance_should_set_defaults()
    {
        var newInstance = new LabelChunk();
        var compareToInstance = new LabelChunk
        {
            ParentFile = null,
            FileOffset = 0,
            Magic = DatFile.Magics.Label,
            ParentId = 0,
            SubType = 0,
            Text = null
        };

        // Assert
        newInstance.Should().BeEquivalentTo(compareToInstance);
    }

    [Fact]
    public async Task When_serializing_should_produce_correct_binary_data()
    {
        byte[] expectedRawData = [0x7b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x54, 0x68, 0x69, 0x73, 0x20, 0x69, 0x73, 0x20, 0x6d, 0x79, 0x20, 0x6c, 0x61, 0x62, 0x65, 0x6c, 0x00];
        var chunk = new LabelChunk { ParentId = 123, Text = "This is my label" };

        using var ms = new MemoryStream();
        // Act
        await chunk.SerializeAsync(ms, false);

        // Assert
        ms.ToArray().Should().BeEquivalentTo(expectedRawData);
    }

    [Theory]
    [InlineData("my label")]
    [InlineData("")]
    [InlineData(null)]
    public async Task When_serializing_and_then_deserializing_should_produce_equivalent(string label)
    {
        var chunk = new LabelChunk { ParentId = 123, Text = label };

        using var ms = new MemoryStream();
        await chunk.SerializeAsync(ms, false);
        ms.Position = 0;

        // Act
        var deserializedChunk = new LabelChunk();
        await deserializedChunk.DeserializeAsync(ms, false);

        // Assert
        deserializedChunk.Should().BeEquivalentTo(chunk);
        ms.Should().BeEof();
    }

    [Fact]
    public async Task Given_stream_contains_more_data_than_chunk_needs_should_advance_to_end()
    {
        var chunk = new LabelChunk();
        using var ms = new MemoryStream();
        await chunk.SerializeAsync(ms, false);
        // Add garbage to end.
        ms.Write([0x1, 0x2], 0, 2);
        ms.Position = 0;

        // Act
        await chunk.DeserializeAsync(ms, false);

        // Assert
        ms.Should().BeEof();
    }
}
