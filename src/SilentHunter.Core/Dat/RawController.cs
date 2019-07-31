using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SilentHunter.Dat.Controllers.Serialization;
using SilentHunter.Extensions;
using SilentHunter.Formats;
using skwas.IO;

namespace SilentHunter.Dat
{
	/// <summary>
	/// Represents controller data. <see cref="RawController" /> implements basic (de)serialization rules, where each property is stored anonymously in a sequential order, much like C-structs in memory.
	/// </summary>
	/// <remarks>
	/// Raw controllers are typically stored where each property is serialized in binary form, in a sequential order.
	/// 
	/// Every type that inherits from <see cref="RawController" /> will be parsed following these rules.
	/// </remarks>
	public abstract class RawController : IRawController, IControllerSerializer
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
		protected virtual void Deserialize(Stream stream)
		{
			using (var reader = new BinaryReader(stream, Encoding.ParseEncoding, true))
			{
				DeserializeFields(reader, GetType(), this);

				stream.EnsureStreamPosition(stream.Length, GetType().Name);
			}
		}

		/// <summary>
		/// When implemented, serializes the controller to specified <paramref name="stream" />.
		/// </summary>
		/// <param name="stream">The stream.</param>
		protected virtual void Serialize(Stream stream)
		{
			using (var writer = new BinaryWriter(stream, Encoding.ParseEncoding, true))
			{
				SerializeFields(writer, GetType(), this);
			}
		}

		void IControllerSerializer.DeserializeFields(BinaryReader reader, Type typeOfValue, object instance)
		{
			DeserializeFields(reader, typeOfValue, instance);
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

			if (instance is IRawController serializable && serializable != this)
			{
				serializable.Deserialize(reader.BaseStream);
			}
			else
			{
				FieldInfo[] fields = typeOfValue.GetFields(BindingFlags.Public | BindingFlags.Instance);
				foreach (FieldInfo fld in fields)
				{
					object fieldValue = ReadField(reader, fld);
					// We have our value, set it.
					fld.SetValue(instance, fieldValue);
				}
			}
		}

		void IControllerSerializer.SerializeFields(BinaryWriter writer, Type typeOfValue, object instance)
		{
			SerializeFields(writer, typeOfValue, instance);
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

			if (instance is IRawController serializable && serializable != this)
			{
				serializable.Serialize(writer.BaseStream);
			}
			else
			{
				FieldInfo[] fields = typeOfValue.GetFields(BindingFlags.Public | BindingFlags.Instance);
				foreach (FieldInfo fld in fields)
				{
					object fieldValue = fld.GetValue(instance);
					// Write the value.
					WriteField(writer, fld, fieldValue);
				}
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

		object IControllerSerializer.ReadValue(BinaryReader reader, MemberInfo memberInfo)
		{
			return ReadValue(reader, memberInfo);
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

		void IControllerSerializer.WriteValue(BinaryWriter writer, MemberInfo memberInfo, object value)
		{
			WriteValue(writer, memberInfo, value);
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

		#region Implementation of IRawController

		/// <summary>
		/// When implemented, deserializes the controller from specified <paramref name="stream" />.
		/// </summary>
		/// <param name="stream">The stream.</param>
		void IRawSerializable.Deserialize(Stream stream)
		{
			Deserialize(stream);
		}

		/// <summary>
		/// When implemented, serializes the controller to specified <paramref name="stream" />.
		/// </summary>
		/// <param name="stream">The stream.</param>
		void IRawSerializable.Serialize(Stream stream)
		{
			Serialize(stream);
		}

		#endregion

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