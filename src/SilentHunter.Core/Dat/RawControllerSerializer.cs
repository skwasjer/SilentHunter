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

		/// <summary>
		/// When implemented, deserializes the controller from specified <paramref name="stream" />.
		/// </summary>
		/// <param name="stream">The stream.</param>
		public void Deserialize(Stream stream, Type controllerType, object instance)
		{
			using (var reader = new BinaryReader(stream, Encoding.ParseEncoding, true))
			{
				Deserialize(reader, controllerType, instance);

				reader.BaseStream.EnsureStreamPosition(reader.BaseStream.Length, controllerType.Name);
			}
		}

		public virtual void Deserialize(BinaryReader reader, Type controllerType, object instance)
		{
			DeserializeFields(reader, controllerType, instance);
		}

		/// <summary>
		/// When implemented, serializes the controller to specified <paramref name="stream" />.
		/// </summary>
		/// <param name="stream">The stream.</param>
		public void Serialize(Stream stream, Type controllerType, object instance)
		{
			using (var writer = new BinaryWriter(stream, Encoding.ParseEncoding, true))
			{
				Serialize(writer, controllerType, instance);
			}
		}

		public virtual void Serialize(BinaryWriter writer, Type controllerType, object instance)
		{
			SerializeFields(writer, controllerType, instance);
		}

		protected virtual void DeserializeFields(BinaryReader reader, Type typeOfValue, object instance)
		{
			if (reader == null)
			{
				throw new ArgumentNullException(nameof(reader));
			}

			if (typeOfValue == null)
			{
				throw new ArgumentNullException(nameof(typeOfValue));
			}

			if (instance == null)
			{
				throw new ArgumentNullException(nameof(instance));
			}
			
			FieldInfo[] fields = typeOfValue.GetFields(BindingFlags.Public | BindingFlags.Instance);
			foreach (FieldInfo fld in fields)
			{
				object fieldValue = ReadField(reader, fld);
				// We have our value, set it.
				fld.SetValue(instance, fieldValue);
			}
		}

		protected virtual void SerializeFields(BinaryWriter writer, Type typeOfValue, object instance)
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
			foreach (FieldInfo fld in fields)
			{
				object fieldValue = fld.GetValue(instance);
				// Write the value.
				WriteField(writer, fld, fieldValue);
			}
		}

		object IControllerSerializer.ReadField(BinaryReader reader, MemberInfo memberInfo)
		{
			return ReadField(reader, memberInfo);
		}

		protected virtual object ReadField(BinaryReader reader, MemberInfo memberInfo)
		{
			CheckArguments(reader, memberInfo);

			var field = memberInfo as FieldInfo;
			Type typeOfValue = field?.FieldType ?? (Type)memberInfo;

			object retVal;

			if (typeOfValue.IsControllerOrSHType())
			{
				// Create a new instance of this type.
				retVal = Activator.CreateInstance(typeOfValue);

				DeserializeFields(reader, typeOfValue, retVal);
			}
			else
			{
				retVal = ReadValue(reader, memberInfo);
			}

			return retVal;
		}

		void IControllerSerializer.WriteField(BinaryWriter writer, MemberInfo memberInfo, object instance)
		{
			WriteField(writer, memberInfo, instance);
		}

		protected virtual void WriteField(BinaryWriter writer, MemberInfo memberInfo, object instance)
		{
			var field = memberInfo as FieldInfo;
			Type typeOfValue = field?.FieldType ?? (Type)memberInfo;

			if (typeOfValue.IsControllerOrSHType())
			{
				SerializeFields(writer, typeOfValue, instance);
			}
			else
			{
				WriteValue(writer, memberInfo, instance);
			}
		}

		protected virtual object ReadValue(BinaryReader reader, MemberInfo memberInfo)
		{
			var ctx = new ControllerDeserializationContext(this, memberInfo);
			IControllerValueSerializer serializer = _serializers.FirstOrDefault(s => s.IsSupported(ctx));
			object result = serializer?.Deserialize(reader, ctx);
			if (result == null)
			{
				throw new NotImplementedException($"The specified type '{ctx.Type.FullName}' is not supported or implemented.");
			}

			return result;
		}

		protected virtual void WriteValue(BinaryWriter writer, MemberInfo memberInfo, object value)
		{
			var ctx = new ControllerSerializationContext(this, memberInfo, value);
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