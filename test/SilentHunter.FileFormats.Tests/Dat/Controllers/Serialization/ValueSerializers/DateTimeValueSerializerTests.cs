using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using SilentHunter.Controllers;
using SilentHunter.Testing.FluentAssertions;

namespace SilentHunter.FileFormats.Dat.Controllers.Serialization;

public class DateTimeValueSerializerTests
{
    private class TestClass
    {
#pragma warning disable 649
        public DateTime Supported;

        public string NotSupported;
#pragma warning restore 649
    }

    private readonly DateTimeValueSerializer _sut;
    private readonly ControllerSerializationContext _serializationContext;

    public DateTimeValueSerializerTests()
    {
        _sut = new DateTimeValueSerializer();

        FieldInfo field = typeof(TestClass).GetField(nameof(TestClass.Supported));
        _serializationContext = new ControllerSerializationContext(field, new Mock<Controller>().Object);
    }

    [Fact]
    public void When_member_is_dateTime_should_return_true()
    {
        // Act & assert
        _sut.IsSupported(_serializationContext)
            .Should()
            .BeTrue();
    }

    [Fact]
    public void When_member_is_not_dateTime_should_return_false()
    {
        FieldInfo field = typeof(TestClass).GetField(nameof(TestClass.NotSupported));

        // Act & assert
        _sut.IsSupported(new ControllerSerializationContext(field, new Mock<Controller>().Object))
            .Should()
            .BeFalse();
    }

    [Theory]
    [InlineData("1945-04-12", 19450412)]
    [InlineData("1936-12-31", 19361231)]
    [InlineData("2019-08-04", 20190804)]
    public void Given_date_when_serializing_should_write_correct_integer(string dateStr, int expectedSerialized)
    {
        var date = DateTime.ParseExact(dateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture);

        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms, FileEncoding.Default);
        // Act
        _sut.Serialize(writer, _serializationContext, date);

        // Assert
        BitConverter.ToInt32(ms.ToArray(), 0).Should().Be(expectedSerialized);
    }

    [Theory]
    [InlineData("1945-04-12", 19450412)]
    [InlineData("1936-12-31", 19361231)]
    [InlineData("2019-08-04", 20190804)]
    public void Given_serialized_date_when_deserializing_should_read_correct_date(string expectedDateStr, int serializedDate)
    {
        var expectedDate = DateTime.ParseExact(expectedDateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture);

        using var ms = new MemoryStream(BitConverter.GetBytes(serializedDate));
        using var reader = new BinaryReader(ms, FileEncoding.Default);
        // Act
        DateTime actual = _sut.Deserialize(reader, _serializationContext);

        // Assert
        actual.Should().Be(expectedDate);
        ms.Should().BeEof();
    }

    /// <summary>
    /// In some files, an impossible date can occur, f.ex. 30th feb, 31st feb, etc. The serializer tries (a small number of times) to fix this.
    /// </summary>
    [Theory]
    [InlineData("1942-02-28", 19420231)]
    [InlineData("1943-02-28", 19430231)]
    [InlineData("1944-02-29", 19440231)]
    [InlineData("1945-02-28", 19450231)]
    public void Given_invalid_serialized_date_when_deserializing_should_fix_date(string expectedDateStr, int serializedDate)
    {
        var expectedDate = DateTime.ParseExact(expectedDateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture);

        using var ms = new MemoryStream(BitConverter.GetBytes(serializedDate));
        using var reader = new BinaryReader(ms, FileEncoding.Default);
        // Act
        DateTime actual = _sut.Deserialize(reader, _serializationContext);

        // Assert
        actual.Should().Be(expectedDate);
    }

    [Fact]
    public void Given_really_wrong_data_when_deserializing_should_throw()
    {
        const int impossibleDate = int.MaxValue;

        // Act
        Action act = () =>
        {
            using var ms = new MemoryStream(BitConverter.GetBytes(impossibleDate));
            using var reader = new BinaryReader(ms, FileEncoding.Default);
            _sut.Deserialize(reader, _serializationContext);
        };

        // Assert
        act.Should().Throw<FormatException>();
    }
}
