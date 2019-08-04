using System;
using System.IO;
using System.Reflection;
using FluentAssertions;
using Moq;
using SilentHunter.Controllers;
using SilentHunter.Controllers.Decoration;
using SilentHunter.Testing.FluentAssertions;
using Xunit;

namespace SilentHunter.FileFormats.Dat.Controllers.Serialization
{
	public class StringValueSerializerTests
	{
		private class TestClass
		{
			public string Supported;

			[FixedString(10)]
			public string NotSupported;
		}

		private readonly StringValueSerializer _sut;
		private readonly ControllerSerializationContext _serializationContext;

		public StringValueSerializerTests()
		{
			_sut = new StringValueSerializer();

			FieldInfo field = typeof(TestClass).GetField(nameof(TestClass.Supported));
			_serializationContext = new ControllerSerializationContext(field, new Mock<Controller>().Object);
		}

		[Fact]
		public void When_member_is_string_and_has_fixedStringAttribute_should_return_true()
		{
			// Act & assert
			_sut.IsSupported(_serializationContext)
				.Should().BeTrue();
		}

		[Fact]
		public void When_member_is_string_and_does_not_have_fixedStringAttribute_should_return_false()
		{
			FieldInfo field = typeof(TestClass).GetField(nameof(TestClass.NotSupported));

			// Act & assert
			_sut.IsSupported(new ControllerSerializationContext(field, new Mock<Controller>().Object))
				.Should().BeFalse();
		}

		[Theory]
		[InlineData(null, "")]
		[InlineData("", "\0")]
		[InlineData("short", "short\0")]
		[InlineData("exactly 10", "exactly 10\0")]
		[InlineData("a very long string", "a very long string\0")]
		public void Given_string_when_serializing_should_write_null_terminated_string(string value, string expectedSerialized)
		{
			using (var ms = new MemoryStream())
			using (var writer = new BinaryWriter(ms, FileEncoding.Default))
			{
				// Act
				_sut.Serialize(writer, _serializationContext, value);

				// Assert
				FileEncoding.Default.GetString(ms.ToArray()).Should().BeEquivalentTo(expectedSerialized);
			}
		}

		[Theory]
		[InlineData("", null)]
		[InlineData("\0", "")]
		[InlineData("short\0", "short")]
		[InlineData("exactly 10\0", "exactly 10")]
		[InlineData("a very long string\0", "a very long string")]
		public void Given_null_terminated_serialized_string_when_deserializing_should_return_string_without_terminator(string serialized, string expectedDeserialized)
		{
			using (var ms = new MemoryStream(FileEncoding.Default.GetBytes(serialized)))
			using (var reader = new BinaryReader(ms, FileEncoding.Default))
			{
				// Act
				string actual = _sut.Deserialize(reader, _serializationContext);

				// Assert
				actual.Should().Be(expectedDeserialized);
				ms.Should().BeEof();
			}
		}
	}
}
