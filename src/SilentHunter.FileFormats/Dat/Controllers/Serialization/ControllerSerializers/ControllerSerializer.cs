using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SilentHunter.Controllers;
using SilentHunter.FileFormats.Extensions;

namespace SilentHunter.FileFormats.Dat.Controllers.Serialization
{
	/// <summary>
	/// Represents controller data serializer, which implements basic (de)serialization rules where each property is stored anonymously in a sequential order, much like C-structs in memory.
	/// </summary>
	/// <remarks>
	/// Raw controllers are typically stored where each property is serialized in binary form, in a sequential order.
	/// </remarks>
	public class ControllerSerializer : IControllerSerializer, IControllerFieldSerializer
	{
		private readonly ICollection<IControllerValueSerializer> _serializers;

		public ControllerSerializer()
		{
			_serializers = new List<IControllerValueSerializer>
			{
				new FixedStringValueSerializer(),
				new StringValueSerializer(),
				new ColorValueSerializer(),
				new DateTimeValueSerializer(),
				new BooleanValueSerializer(),
				new NullableValueSerializer(),
				new PrimitiveArrayValueSerializer(),
				new ListValueSerializer(this),
				new SHUnionValueSerializer(),
				new ValueTypeSerializer()	// Should be last.
			};
		}

		public void Deserialize(Stream stream, Controller controller)
		{
			if (stream == null)
			{
				throw new ArgumentNullException(nameof(stream));
			}

			if (controller == null)
			{
				throw new ArgumentNullException(nameof(controller));
			}

			Type controllerType = controller.GetType();
			using (var reader = new BinaryReader(stream, FileEncoding.Default, true))
			{
				var ctx = new ControllerSerializationContext(controllerType, controller);
				Deserialize(reader, ctx, controller);

				reader.BaseStream.EnsureStreamPosition(reader.BaseStream.Length, controllerType.Name);
			}
		}

		protected virtual void Deserialize(BinaryReader reader, ControllerSerializationContext serializationContext, object instance)
		{
			DeserializeFields(reader, serializationContext, instance);
		}

		/// <summary>
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="instanceType"></param>
		/// <param name="instance"></param>
		/// <exception cref="ArgumentNullException">Thrown for arguments that can't be null.</exception>
		/// <exception cref="IOException">Thrown for any IO error or parsing error.</exception>
		/// <exception cref="NotSupportedException">Thrown when <paramref name="instanceType" /> is not supported.</exception>
		/// <exception cref="NotImplementedException">Thrown when <see cref="Array" />s array used in the controller definitions.</exception>
		private void DeserializeFields(BinaryReader reader, ControllerSerializationContext serializationContext, object instance)
		{
			if (reader == null)
			{
				throw new ArgumentNullException(nameof(reader));
			}

			if (serializationContext == null)
			{
				throw new ArgumentNullException(nameof(serializationContext));
			}

			if (instance == null)
			{
				throw new ArgumentNullException(nameof(instance));
			}
			
			FieldInfo[] fields = serializationContext.Type.GetFields(BindingFlags.Public | BindingFlags.Instance);
			foreach (FieldInfo field in fields)
			{
				var ctx = new ControllerSerializationContext(field, serializationContext.Controller);
				var value = DeserializeField(reader, ctx);
				// We have our value, set it.
				field.SetValue(instance, value);
			}
		}

		protected virtual object DeserializeField(BinaryReader reader, ControllerSerializationContext serializationContext)
		{
			return ReadField(reader, serializationContext);
		}

		object IControllerFieldSerializer.ReadField(BinaryReader reader, ControllerSerializationContext serializationContext)
		{
			return ReadField(reader, serializationContext);
		}

		protected virtual object ReadField(BinaryReader reader, ControllerSerializationContext serializationContext)
		{
			object retVal;

			if (serializationContext.Type.IsControllerOrObject())
			{
				// Create a new instance of this type.
				retVal = Activator.CreateInstance(serializationContext.Type);

				DeserializeFields(reader, serializationContext, retVal);
			}
			else
			{
				retVal = ReadValue(reader, serializationContext);
			}

			return retVal;
		}

		public void Serialize(Stream stream, Controller controller)
		{
			if (stream == null)
			{
				throw new ArgumentNullException(nameof(stream));
			}

			if (controller == null)
			{
				throw new ArgumentNullException(nameof(controller));
			}

			Type controllerType = controller.GetType();
			using (var writer = new BinaryWriter(stream, FileEncoding.Default, true))
			{
				var ctx = new ControllerSerializationContext(controllerType, controller);
				Serialize(writer, ctx, controller);
			}
		}

		protected virtual void Serialize(BinaryWriter writer, ControllerSerializationContext serializationContext, object instance)
		{
			SerializeFields(writer, serializationContext, instance);
		}

		private void SerializeFields(BinaryWriter writer, ControllerSerializationContext serializationContext, object instance)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}

			if (instance == null)
			{
				throw new ArgumentNullException(nameof(instance));
			}

			FieldInfo[] fields = serializationContext.Type.GetFields(BindingFlags.Public | BindingFlags.Instance);
			foreach (FieldInfo field in fields)
			{
				var ctx = new ControllerSerializationContext(field, serializationContext.Controller);
				object value = field.GetValue(instance);
				SerializeField(writer, ctx, value);
			}
		}

		protected virtual void SerializeField(BinaryWriter writer, ControllerSerializationContext serializationContext, object value)
		{
			WriteField(writer, serializationContext, value);
		}

		void IControllerFieldSerializer.WriteField(BinaryWriter writer, ControllerSerializationContext serializationContext, object value)
		{
			WriteField(writer, serializationContext, value);
		}

		protected virtual void WriteField(BinaryWriter writer, ControllerSerializationContext serializationContext, object value)
		{
			if (serializationContext.Type.IsControllerOrObject())
			{
				SerializeFields(writer, serializationContext, value);
			}
			else
			{
				WriteValue(writer, serializationContext, value);
			}
		}

		private object ReadValue(BinaryReader reader, ControllerSerializationContext ctx)
		{
			IControllerValueSerializer serializer = _serializers.FirstOrDefault(s => s.IsSupported(ctx));
			object result = serializer?.Deserialize(reader, ctx);
			if (result == null)
			{
				throw new NotImplementedException($"The specified type '{ctx.Type.FullName}' is not supported or implemented.");
			}

			return result;
		}

		private void WriteValue(BinaryWriter writer, ControllerSerializationContext ctx, object value)
		{
			IControllerValueSerializer serializer = _serializers.FirstOrDefault(s => s.IsSupported(ctx));
			if (serializer == null)
			{
				throw new NotImplementedException($"The specified type '{ctx.Type.FullName}' is not supported or implemented.");
			}

			serializer.Serialize(writer, ctx, value);
		}
		
		/// <summary>
		/// Checks that the specified arguments are valid.
		/// </summary>
		/// <param name="reader">The reader to check.</param>
		/// <param name="memberInfo">The member info to check.</param>
		protected static void CheckArguments(BinaryReader reader, MemberInfo memberInfo)
		{
			if (reader == null)
			{
				throw new ArgumentNullException(nameof(reader));
			}

			if (memberInfo == null)
			{
				throw new ArgumentNullException(nameof(memberInfo));
			}

			// Member info can be a Type or a FieldInfo.
			if (!(memberInfo is Type || memberInfo is FieldInfo))
			{
				throw new ArgumentException("Expected a Type or FieldInfo.", nameof(memberInfo));
			}
		}
	}
}