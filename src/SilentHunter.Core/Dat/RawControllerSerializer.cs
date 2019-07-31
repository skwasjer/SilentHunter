using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SilentHunter.Dat.Controllers.Serialization;
using SilentHunter.Extensions;
using SilentHunter.Formats;

namespace SilentHunter.Dat
{
	/// <summary>
	/// Represents raw controller data serializer, which implements basic (de)serialization rules where each property is stored anonymously in a sequential order, much like C-structs in memory.
	/// </summary>
	/// <remarks>
	/// Raw controllers are typically stored where each property is serialized in binary form, in a sequential order.
	/// </remarks>
	public class RawControllerSerializer : IControllerSerializer
	{
		private ICollection<IControllerValueSerializer> _serializers = new List<IControllerValueSerializer>
		{
			new FixedStringValueSerializer(),
			new StringValueSerializer(),
			new ColorValueSerializer(),
			new DateTimeValueSerializer(),
			new BooleanValueSerializer(),
			new NullableValueSerializer(),
			new PrimitiveArrayValueSerializer(),
			new ListValueSerializer(),
			new SHUnionValueSerializer(),
			new DefaultObjectSerializer()	// Should be last.
		};

		public void Deserialize(Stream stream, IRawController controller)
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
			using (var reader = new BinaryReader(stream, Encoding.ParseEncoding, true))
			{
				Deserialize(reader, controllerType, controller);

				reader.BaseStream.EnsureStreamPosition(reader.BaseStream.Length, controllerType.Name);
			}
		}

		public virtual void Deserialize(BinaryReader reader, Type instanceType, object instance)
		{
			DeserializeFields(reader, instanceType, instance);
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
		private void DeserializeFields(BinaryReader reader, Type instanceType, object instance)
		{
			if (reader == null)
			{
				throw new ArgumentNullException(nameof(reader));
			}

			if (instanceType == null)
			{
				throw new ArgumentNullException(nameof(instanceType));
			}

			if (instance == null)
			{
				throw new ArgumentNullException(nameof(instance));
			}
			
			FieldInfo[] fields = instanceType.GetFields(BindingFlags.Public | BindingFlags.Instance);
			foreach (FieldInfo field in fields)
			{
				var value = DeserializeField(reader, field);
				// We have our value, set it.
				field.SetValue(instance, value);
			}
		}

		protected virtual object DeserializeField(BinaryReader reader, FieldInfo field)
		{
			return ReadField(reader, field);
		}

		object IControllerSerializer.ReadField(BinaryReader reader, MemberInfo memberInfo)
		{
			return ReadField(reader, memberInfo);
		}

		protected virtual object ReadField(BinaryReader reader, MemberInfo memberInfo)
		{
			CheckArguments(reader, memberInfo);

			var ctx = new ControllerDeserializationContext(this, memberInfo);

			object retVal;

			if (ctx.Type.IsControllerOrSHType())
			{
				// Create a new instance of this type.
				retVal = Activator.CreateInstance(ctx.Type);

				DeserializeFields(reader, ctx.Type, retVal);
			}
			else
			{
				retVal = ReadValue(reader, ctx);
			}

			return retVal;
		}

		public void Serialize(Stream stream, IRawController controller)
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
			using (var writer = new BinaryWriter(stream, Encoding.ParseEncoding, true))
			{
				Serialize(writer, controllerType, controller);
			}
		}

		public virtual void Serialize(BinaryWriter writer, Type instanceType, object instance)
		{
			SerializeFields(writer, instanceType, instance);
		}

		private void SerializeFields(BinaryWriter writer, Type typeOfValue, object instance)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}

			if (instance == null)
			{
				throw new ArgumentNullException(nameof(instance));
			}

			FieldInfo[] fields = typeOfValue.GetFields(BindingFlags.Public | BindingFlags.Instance);
			foreach (FieldInfo field in fields)
			{
				object value = field.GetValue(instance);
				SerializeField(writer, field, value);
			}
		}

		protected virtual void SerializeField(BinaryWriter writer, FieldInfo field, object value)
		{
			// Write the value.
			WriteField(writer, field, value);
		}

		void IControllerSerializer.WriteField(BinaryWriter writer, MemberInfo memberInfo, object instance)
		{
			WriteField(writer, memberInfo, instance);
		}

		protected virtual void WriteField(BinaryWriter writer, MemberInfo memberInfo, object value)
		{
			var ctx = new ControllerSerializationContext(this, memberInfo, value);
			if (ctx.Type.IsControllerOrSHType())
			{
				SerializeFields(writer, ctx.Type, value);
			}
			else
			{
				WriteValue(writer, ctx);
			}
		}

		private object ReadValue(BinaryReader reader, ControllerDeserializationContext ctx)
		{
			IControllerValueSerializer serializer = _serializers.FirstOrDefault(s => s.IsSupported(ctx));
			object result = serializer?.Deserialize(reader, ctx);
			if (result == null)
			{
				throw new NotImplementedException($"The specified type '{ctx.Type.FullName}' is not supported or implemented.");
			}

			return result;
		}

		private void WriteValue(BinaryWriter writer, ControllerSerializationContext ctx)
		{
			IControllerValueSerializer serializer = _serializers.FirstOrDefault(s => s.IsSupported(ctx));
			if (serializer == null)
			{
				throw new NotImplementedException($"The specified type '{ctx.Type.FullName}' is not supported or implemented.");
			}

			serializer.Serialize(writer, ctx);
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