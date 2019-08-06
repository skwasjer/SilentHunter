using System;
using System.IO;
using System.Reflection;
using FluentAssertions;
using Moq;
using SilentHunter.Controllers;
using SilentHunter.Testing.FluentAssertions;
using Xunit;

namespace SilentHunter.FileFormats.Dat.Controllers.Serialization
{
	public class ColorValueSerializerTests
	{
		private readonly ColorValueSerializer _sut;

		private class TestClass
		{
#pragma warning disable 649
			public Color Supported;

			public string NotSupported;
#pragma warning restore 649
		}

		private readonly ControllerSerializationContext _serializationContext;

		public ColorValueSerializerTests()
		{
			_sut = new ColorValueSerializer();

			FieldInfo field = typeof(TestClass).GetField(nameof(TestClass.Supported));
			_serializationContext = new ControllerSerializationContext(field, new Mock<Controller>().Object);
		}

		[Fact]
		public void When_member_is_color_should_return_true()
		{
			// Act & assert
			_sut.IsSupported(_serializationContext)
				.Should().BeTrue();
		}

		[Fact]
		public void When_member_is_not_color_should_return_false()
		{
			FieldInfo field = typeof(TestClass).GetField(nameof(TestClass.NotSupported));

			// Act & assert
			_sut.IsSupported(new ControllerSerializationContext(field, new Mock<Controller>().Object))
				.Should().BeFalse();
		}

		[Theory]
		[InlineData(0xFFFFFFFF, 0xFFFFFF00)]
		[InlineData(0xCCFFFFFF, 0xFFFFFF00)]
		[InlineData(0xAA123456, 0x12345600)]
		[InlineData(0x00000000, 0x00000000)]
		public void Given_color_when_serializing_should_write_color_without_alpha(uint value, uint expectedSerialized)
		{
			Color color = Color.FromArgb(unchecked((int)value));

			using (var ms = new MemoryStream())
			using (var writer = new BinaryWriter(ms, FileEncoding.Default))
			{
				// Act
				_sut.Serialize(writer, _serializationContext, color);

				// Assert
				BitConverter.ToInt32(ms.ToArray(), 0).Should().Be(unchecked((int)expectedSerialized));
			}
		}

		[Theory]
		[InlineData(0xFFFFFFFF, 0xFFFFFF00)]
		[InlineData(0xFFFFFFFF, 0xFFFFFFCC)]
		[InlineData(0xFF123456, 0x12345600)]
		[InlineData(0xFF000000, 0x00000000)]
		public void Given_serialized_color_when_deserializing_should_read_correct_color(uint expectedColor, uint serialized)
		{
			Color color = Color.FromArgb(unchecked((int)expectedColor));

			using (var ms = new MemoryStream(BitConverter.GetBytes(serialized)))
			using (var reader = new BinaryReader(ms, FileEncoding.Default))
			{
				// Act
				Color actual = _sut.Deserialize(reader, _serializationContext);

				// Assert
				actual.Should().Be(color);
				ms.Should().BeEof();
			}
		}
	}
}
