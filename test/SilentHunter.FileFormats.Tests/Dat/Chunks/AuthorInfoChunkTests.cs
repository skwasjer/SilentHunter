using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using SilentHunter.Testing.FluentAssertions;
using Xunit;

namespace SilentHunter.FileFormats.Dat.Chunks;

public class AuthorInfoChunkTests
{
    [Fact]
    public void Should_not_support_id()
    {
        ulong id = unchecked((ulong)DateTime.Now.Ticks);

        // Act
        Action act = () => new AuthorInfoChunk().Id = id;

        // Assert
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Should_not_support_parent_id()
    {
        ulong id = unchecked((ulong)DateTime.Now.Ticks);

        // Act
        Action act = () => new AuthorInfoChunk().ParentId = id;

        // Assert
        act.Should().Throw<Exception>();
    }

    [Fact]
    public async Task When_serializing_should_produce_correct_binary_data()
    {
        byte[] expectedRawData = { 00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x73, 0x6b, 0x77, 0x61, 0x73, 0x00, 0x4d, 0x79, 0x20, 0x64, 0x65, 0x73, 0x63, 0x72, 0x69, 0x70, 0x74, 0x69, 0x6f, 0x6e, 0x00 };

        var chunk = new AuthorInfoChunk
        {
            Author = "skwas",
            Description = "My description"
        };

        using (var ms = new MemoryStream())
        {
            // Act
            await chunk.SerializeAsync(ms, false);

            // Assert
            ms.ToArray().Should().BeEquivalentTo(expectedRawData);
        }
    }

    [Fact]
    public async Task When_serializing_without_description_should_produce_correct_binary_data()
    {
        byte[] expectedRawData = { 00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x73, 0x6b, 0x77, 0x61, 0x73, 0x00 };

        var chunk = new AuthorInfoChunk
        {
            Author = "skwas",
            Description = null
        };

        using (var ms = new MemoryStream())
        {
            // Act
            await chunk.SerializeAsync(ms, false);

            // Assert
            ms.ToArray().Should().BeEquivalentTo(expectedRawData);
        }
    }

    [Fact]
    public async Task When_serializing_and_then_deserializing_should_produce_equivalent()
    {
        var chunk = new AuthorInfoChunk
        {
            Unknown = 123,
            Author = "skwas",
            Description = "My description"
        };

        using (var ms = new MemoryStream())
        {
            await chunk.SerializeAsync(ms, false);
            ms.Position = 0;

            // Act
            var deserializedChunk = new AuthorInfoChunk();
            await deserializedChunk.DeserializeAsync(ms, false);

            // Assert
            deserializedChunk.Should().BeEquivalentTo(chunk, options => options.Excluding(c => c.UnknownData));
            ms.Should().BeEof();
        }
    }

    [Fact]
    public async Task Given_stream_contains_more_data_than_chunk_needs_should_advance_to_end()
    {
        var chunk = new AuthorInfoChunk
        {
            Author = "skwas",
            Description = "My file"
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