using System;
using System.IO;
using System.Reflection;
using FluentAssertions;
using Moq;
using SilentHunter.Controllers;
using Xunit;

namespace SilentHunter.FileFormats.Dat.Controllers.Serialization
{
	public class ValueTypeSerializerTests
	{
		private struct TestClass<T>
			where T : struct
		{
#pragma warning disable 649
			public T Supported;

			public string NotSupported;
#pragma warning restore 649
		}

		private readonly ValueTypeSerializer _sut;

		public ValueTypeSerializerTests()
		{
			_sut = new ValueTypeSerializer();
		}

		[Theory]
		[InlineData(typeof(int))]
		[InlineData(typeof(Axis))]
		[InlineData(typeof(TestClass<float>))]
		public void When_member_type_is_value_type_should_return_true(Type type)
		{
			// Act & assert
			_sut.IsSupported(new ControllerSerializationContext(type, new Mock<Controller>().Object))
				.Should().BeTrue();
		}

		[Fact]
		public void When_member_is_not_supported_should_return_false()
		{
			FieldInfo field = typeof(TestClass<float>).GetField(nameof(TestClass<float>.NotSupported));

			// Act & assert
			_sut.IsSupported(new ControllerSerializationContext(field, new Mock<Controller>().Object))
				.Should().BeFalse();
		}

		[Theory]
		[InlineData(typeof(TestClass<int>), 123)]
		[InlineData(typeof(TestClass<Axis>), Axis.Y)]
		public void Given_value_when_serializing_should_write_value_type(Type typeWithSupportedField, object value)
		{
			FieldInfo field = typeWithSupportedField.GetField("Supported");
			var serializationContext = new ControllerSerializationContext(field, new Mock<Controller>().Object);

			using (var ms = new MemoryStream())
			using (var writer = new BinaryWriter(ms, FileEncoding.Default))
			{
				// Act
				_sut.Serialize(writer, serializationContext, value);

				// Assert
				BitConverter.GetBytes((int)value).Should().Equal(ms.ToArray());
			}
		}

		[Theory]
		[InlineData(123, typeof(TestClass<int>))]
		[InlineData(Axis.Y, typeof(TestClass<Axis>))]
		public void Given_serialized_value_type_when_deserializing_should_return_typed_value_type(object value, Type expectedType)
		{
			FieldInfo field = expectedType.GetField("Supported");
			var serializationContext = new ControllerSerializationContext(field, new Mock<Controller>().Object);

			using (var ms = new MemoryStream(BitConverter.GetBytes((int)value)))
			using (var reader = new BinaryReader(ms, FileEncoding.Default))
			{
				// Act
				object actual = _sut.Deserialize(reader, serializationContext);

				// Assert
				actual.Should()
					.BeOfType(field.FieldType)
					.And.Be(value);
			}
		}
	}
}
