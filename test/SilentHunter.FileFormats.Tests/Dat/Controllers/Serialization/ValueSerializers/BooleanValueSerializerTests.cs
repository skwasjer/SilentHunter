using System;
using System.IO;
using System.Reflection;
using FluentAssertions;
using Moq;
using SilentHunter.Controllers;
using SilentHunter.Testing.FluentAssertions;
using Xunit;

namespace SilentHunter.FileFormats.Dat.Controllers.Serialization;

public class BooleanValueSerializerTests
{
    private class TestClass
    {
#pragma warning disable 649
        public bool Supported;

        public string NotSupported;
#pragma warning restore 649
    }

    private readonly BooleanValueSerializer _sut;
    private readonly ControllerSerializationContext _serializationContext;

    public BooleanValueSerializerTests()
    {
        _sut = new BooleanValueSerializer();

        FieldInfo field = typeof(TestClass).GetField(nameof(TestClass.Supported));
        _serializationContext = new ControllerSerializationContext(field, new Mock<Controller>().Object);
    }

    [Fact]
    public void When_member_is_bool_should_return_true()
    {
        // Act & assert
        _sut.IsSupported(_serializationContext)
            .Should()
            .BeTrue();
    }

    [Fact]
    public void When_member_is_not_bool_should_return_false()
    {
        FieldInfo field = typeof(TestClass).GetField(nameof(TestClass.NotSupported));

        // Act & assert
        _sut.IsSupported(new ControllerSerializationContext(field, new Mock<Controller>().Object))
            .Should()
            .BeFalse();
    }

    [Theory]
    [InlineData(false, byte.MinValue)]
    [InlineData(true, (byte)1)]
    public void Given_boolean_when_serializing_should_write_correct_byte(bool value, byte expectedSerialized)
    {
        using (var ms = new MemoryStream())
        using (var writer = new BinaryWriter(ms, FileEncoding.Default))
        {
            // Act
            _sut.Serialize(writer, _serializationContext, value);

            // Assert
            ms.ToArray().Should().BeEquivalentTo(new[] { expectedSerialized });
        }
    }

    [Theory]
    [InlineData(false, byte.MinValue)]
    [InlineData(true, (byte)1)]
    [InlineData(false, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue)]
    [InlineData(true, (byte)1, byte.MinValue, byte.MinValue, byte.MinValue)]
    public void Given_serialized_boolean_when_deserializing_should_read_correct_boolean(bool expectedSerialized, params byte[] buffer)
    {
        using (var ms = new MemoryStream(buffer))
        using (var reader = new BinaryReader(ms, FileEncoding.Default))
        {
            // Act
            bool actual = _sut.Deserialize(reader, _serializationContext);

            // Assert
            actual.Should().Be(expectedSerialized);
        }
    }

    [Fact]
    public void Given_really_wrong_data_when_deserializing_should_throw()
    {
        const long impossibleValue = long.MaxValue;

        // Act
        Action act = () =>
        {
            using (var ms = new MemoryStream(BitConverter.GetBytes(impossibleValue)))
            using (var reader = new BinaryReader(ms, FileEncoding.Default))
            {
                _sut.Deserialize(reader, _serializationContext);
                ms.Should().BeEof();
            }
        };

        // Assert
        act.Should().Throw<FormatException>();
    }
}