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
	public class FixedStringValueSerializerTests
	{
		private class TestClass
		{
			[FixedString(10)]
			public string Supported;

			public string NotSupported;
		}

		private readonly FixedStringValueSerializer _sut;
		private readonly ControllerSerializationContext _serializationContext;

		public FixedStringValueSerializerTests()
		{
			_sut = new FixedStringValueSerializer();

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
		[InlineData(null, "\0\0\0\0\0\0\0\0\0\0")]
		[InlineData("", "\0\0\0\0\0\0\0\0\0\0")]
		[InlineData("short", "short\0\0\0\0\0")]
		[InlineData("exactly 10", "exactly 10")]
		public void Given_fixed_string_when_serializing_should_write_fixed_string_correctly(string value, string expectedSerialized)
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
		[InlineData("\0\0\0\0\0\0\0\0\0\0", "")]
		[InlineData("short\0\0\0\0\0", "short")]
		[InlineData("exactly 10", "exactly 10")]
		public void Given_serialized_fixed_string_when_deserializing_should_return_string_without_null_chars(string serialized, string expectedDeserialized)
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

		[Fact]
		public void Given_not_enough_data_when_deserializing_should_throw()
		{
			const string notEnoughData = "TooShort";

			// Act
			Action act = () =>
			{
				using (var ms = new MemoryStream(FileEncoding.Default.GetBytes(notEnoughData)))
				using (var reader = new BinaryReader(ms, FileEncoding.Default))
				{
					_sut.Deserialize(reader, _serializationContext);
				}
			};

			// Assert
			act.Should().Throw<SilentHunterParserException>();
		}

		[Fact]
		public void Given_too_much_data_when_deserializing_should_throw()
		{
			const string tooMuchData = "Too much data on stream";

			// Act
			Action act = () =>
			{
				using (var ms = new MemoryStream(FileEncoding.Default.GetBytes(tooMuchData)))
				using (var reader = new BinaryReader(ms, FileEncoding.Default))
				{
					_sut.Deserialize(reader, _serializationContext);
				}
			};

			// Assert
			act.Should().Throw<SilentHunterParserException>();
		}
	}
}
