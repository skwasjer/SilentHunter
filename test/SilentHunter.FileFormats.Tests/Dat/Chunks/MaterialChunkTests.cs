using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using SilentHunter.Testing.FluentAssertions;
using Xunit;

namespace SilentHunter.FileFormats.Dat.Chunks;

public class MaterialChunkTests
{
    [Fact]
    public void Should_support_id()
    {
        ulong id = unchecked((ulong)DateTime.Now.Ticks);

        // Act
        Action act = () => new MaterialChunk().Id = id;

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Should_not_support_parent_id()
    {
        ulong id = unchecked((ulong)DateTime.Now.Ticks);

        // Act
        Action act = () => new MaterialChunk().ParentId = id;

        // Assert
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void When_creating_new_instance_should_set_defaults()
    {
        var newInstance = new MaterialChunk();
        var compareToInstance = new MaterialChunk
        {
            ParentFile = null,
            FileOffset = 0,
            Magic = DatFile.Magics.Material,
            Id = 0,
            SubType = 0,
            Attributes = MaterialAttributes.MagFilterLinear | MaterialAttributes.MinFilterLinear,
            Diffuse = Color.FromArgb(149, 149, 149),
            Opacity = 255,
            SpecularMode = SpecularMode.Normal,
            Specular = Color.FromArgb(229, 229, 229),
            SpecularStrength = 0,
            Glossiness = 0,
            Emission = 0,
            CreationTime = DateTime.UtcNow,
            Texture = null,
            TgaTextureSize = 0
        };

        // Assert
        newInstance.Should().BeEquivalentTo(compareToInstance);
    }

    [Fact]
    public async Task When_serializing_should_produce_correct_binary_data()
    {
        byte[] expectedRawData = { 0xc8, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x32, 0x64, 0xc8, 0x01, 0x4e, 0x0d, 0xfc, 0xc1, 0x40, 0x4e, 0x00, 0x02, 0x80, 0x00, 0x00, 0x15, 0x03, 0x00, 0x00, 0xc5, 0x10, 0xa3, 0x48, 0x00, 0x00, 0x00, 0x00, 0x6d, 0x79, 0x20, 0x74, 0x65, 0x78, 0x74, 0x75, 0x72, 0x65, 0x2e, 0x74, 0x67, 0x61, 0x00 };
        var chunk = new MaterialChunk
        {
            Id = 456,
            Attributes = MaterialAttributes.CullNone | MaterialAttributes.NoDxtCompression,
            Diffuse = Color.FromArgb(200, 100, 50),
            Opacity = 128,
            SpecularMode = SpecularMode.SunOnly,
            Specular = Color.FromArgb(252, 13, 78),
            SpecularStrength = 193,
            Glossiness = 64,
            Emission = 78,
            CreationTime = new DateTime(2008, 8, 13, 16, 50, 13),
            Texture = "my texture.tga",
            TgaTextureSize = 789
        };

        using (var ms = new MemoryStream())
        {
            // Act
            await chunk.SerializeAsync(ms, false);

            // Assert
            ms.ToArray().Should().BeEquivalentTo(expectedRawData);
        }
    }

    /// <summary>
    /// Asserts proper serialize/deserialize behavior with regards to normal/extended materials.
    /// </summary>
    [Theory]
    [InlineData("my texture.tga", 0)]
    [InlineData("my texture.tga", 789)]
    [InlineData("my texture.tga", -1)]
    [InlineData(null, 0)]
    [InlineData(null, 789)]
    [InlineData(null, -1)]
    [InlineData("", 0)]
    [InlineData("", 789)]
    [InlineData("", -1)]
    public async Task When_serializing_and_then_deserializing_should_produce_equivalent(string texture, int tgaTextureSize)
    {
        var chunk = new MaterialChunk
        {
            Id = 456,
            CreationTime = new DateTime(2008, 8, 13, 16, 50, 13),
            Texture = texture,
            TgaTextureSize = tgaTextureSize
        };

        using (var ms = new MemoryStream())
        {
            await chunk.SerializeAsync(ms, false);
            ms.Position = 0;

            // Act
            var deserializedChunk = new MaterialChunk();
            await deserializedChunk.DeserializeAsync(ms, false);

            // Assert
            if (deserializedChunk.HasTextureReference)
            {
                deserializedChunk.Should().BeEquivalentTo(chunk);
            }
            else
            {
                deserializedChunk.Should().BeEquivalentTo(chunk,
                    opts => opts
                        .Excluding(c => c.CreationTime)
                        .Excluding(c => c.Texture));
            }

            ms.Should().BeEof();
        }
    }

    [Fact]
    public async Task Given_stream_contains_more_data_than_chunk_needs_should_advance_to_end()
    {
        var chunk = new MaterialChunk();
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

    [Fact]
    public void When_toggling_z_buffer_write_should_update_material_attributes()
    {
        var otherAttribute = MaterialAttributes.CullNone;
        var chunk = new MaterialChunk
        {
            // Add sut attribute + some other flag
            Attributes = MaterialAttributes.DisableZBufferWrite | otherAttribute
        };

        chunk.ZBufferWriteEnabled.Should().BeFalse();
        chunk.Attributes = otherAttribute;
        chunk.ZBufferWriteEnabled.Should().BeTrue();

        // Test setting true/false twice in a row. This asserts that set/unset works in both states.
        chunk.ZBufferWriteEnabled = false;
        chunk.Attributes.Should().Be(MaterialAttributes.DisableZBufferWrite | otherAttribute);

        chunk.ZBufferWriteEnabled = false;
        chunk.Attributes.Should().Be(MaterialAttributes.DisableZBufferWrite | otherAttribute);

        chunk.ZBufferWriteEnabled = true;
        chunk.Attributes.Should().Be(otherAttribute);

        chunk.ZBufferWriteEnabled = true;
        chunk.Attributes.Should().Be(otherAttribute);
    }

    [Fact]
    public void When_toggling_cull_none_should_update_material_attributes()
    {
        var otherAttribute = MaterialAttributes.MagFilterLinear;
        var chunk = new MaterialChunk
        {
            // Add sut attribute + some other flag
            Attributes = MaterialAttributes.CullNone | otherAttribute
        };

        chunk.UseCounterClockwiseCulling.Should().BeFalse();
        chunk.Attributes = otherAttribute;
        chunk.UseCounterClockwiseCulling.Should().BeTrue();

        // Test setting true/false twice in a row. This asserts that set/unset works in both states.
        chunk.UseCounterClockwiseCulling = false;
        chunk.Attributes.Should().Be(MaterialAttributes.CullNone | otherAttribute);

        chunk.UseCounterClockwiseCulling = false;
        chunk.Attributes.Should().Be(MaterialAttributes.CullNone | otherAttribute);

        chunk.UseCounterClockwiseCulling = true;
        chunk.Attributes.Should().Be(otherAttribute);

        chunk.UseCounterClockwiseCulling = true;
        chunk.Attributes.Should().Be(otherAttribute);
    }

    [Fact]
    public void When_toggling_min_filter_linear_should_update_material_attributes()
    {
        var otherAttribute = MaterialAttributes.DisableZBufferWrite;
        var chunk = new MaterialChunk
        {
            // Add sut attribute + some other flag
            Attributes = MaterialAttributes.MinFilterLinear | otherAttribute
        };

        chunk.MinFilterLinear.Should().BeTrue();
        chunk.Attributes = otherAttribute;
        chunk.MinFilterLinear.Should().BeFalse();

        // Test setting true/false twice in a row. This asserts that set/unset works in both states.
        chunk.MinFilterLinear = true;
        chunk.Attributes.Should().Be(MaterialAttributes.MinFilterLinear | otherAttribute);

        chunk.MinFilterLinear = true;
        chunk.Attributes.Should().Be(MaterialAttributes.MinFilterLinear | otherAttribute);

        chunk.MinFilterLinear = false;
        chunk.Attributes.Should().Be(otherAttribute);

        chunk.MinFilterLinear = false;
        chunk.Attributes.Should().Be(otherAttribute);
    }

    [Fact]
    public void When_toggling_mag_filter_linear_should_update_material_attributes()
    {
        var otherAttribute = MaterialAttributes.DisableZBufferWrite;
        var chunk = new MaterialChunk
        {
            // Add sut attribute + some other flag
            Attributes = MaterialAttributes.MagFilterLinear | otherAttribute
        };

        chunk.MagFilterLinear.Should().BeTrue();
        chunk.Attributes = otherAttribute;
        chunk.MagFilterLinear.Should().BeFalse();

        // Test setting true/false twice in a row. This asserts that set/unset works in both states.
        chunk.MagFilterLinear = true;
        chunk.Attributes.Should().Be(MaterialAttributes.MagFilterLinear | otherAttribute);

        chunk.MagFilterLinear = true;
        chunk.Attributes.Should().Be(MaterialAttributes.MagFilterLinear | otherAttribute);

        chunk.MagFilterLinear = false;
        chunk.Attributes.Should().Be(otherAttribute);

        chunk.MagFilterLinear = false;
        chunk.Attributes.Should().Be(otherAttribute);
    }

    [Fact]
    public void When_toggling_dxt_compression_should_update_material_attributes()
    {
        var otherAttribute = MaterialAttributes.MagFilterLinear;
        var chunk = new MaterialChunk
        {
            // Add sut attribute + some other flag
            Attributes = MaterialAttributes.NoDxtCompression | otherAttribute
        };

        chunk.DxtCompression.Should().BeFalse();
        chunk.Attributes = otherAttribute;
        chunk.DxtCompression.Should().BeTrue();

        // Test setting true/false twice in a row. This asserts that set/unset works in both states.
        chunk.DxtCompression = false;
        chunk.Attributes.Should().Be(MaterialAttributes.NoDxtCompression | otherAttribute);

        chunk.DxtCompression = false;
        chunk.Attributes.Should().Be(MaterialAttributes.NoDxtCompression | otherAttribute);

        chunk.DxtCompression = true;
        chunk.Attributes.Should().Be(otherAttribute);

        chunk.DxtCompression = true;
        chunk.Attributes.Should().Be(otherAttribute);
    }
}