using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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
	public abstract class RawController : IRawController
	{
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

		protected virtual object ReadValue(BinaryReader reader, MemberInfo memberInfo, long expectedEndPos = -1)
		{
			CheckArguments(reader, memberInfo);

			var field = memberInfo as FieldInfo;
			Type typeOfValue = field?.FieldType ?? (Type)memberInfo;

			object retVal = null;

			if (typeOfValue == typeof(string))
			{
				if (reader.ReadString(memberInfo, expectedEndPos, out string str))
				{
					return str;
				}
			}
			else if (typeOfValue.IsArray)
			{
				throw new NotSupportedException("Arrays are not supported. Use List<> instead.");
			}
			else if (typeOfValue == typeof(DateTime))
			{
				if (reader.ReadDateTime(field, expectedEndPos, out DateTime dt))
				{
					return dt;
				}
			}
			else if (typeOfValue == typeof(Color))
			{
				// Alpha component is ignored in SH.
				retVal = Color.FromArgb(byte.MaxValue, reader.ReadStruct<Color>());
			}
			else if (typeOfValue == typeof(bool))
			{
				if (reader.ReadBoolean(field, expectedEndPos, out bool bVal))
				{
					return bVal;
				}
			}
			else if (typeOfValue.IsNullable())
			{
				retVal = reader.ReadStruct(Nullable.GetUnderlyingType(typeOfValue));
			}
			else if (typeOfValue.IsGenericType)
			{
				Type[] typeArgs = typeOfValue.GetGenericArguments();

				if (typeof(IList).IsAssignableFrom(typeOfValue))
				{
					retVal = ReadList(reader, typeOfValue);
				}
				else
				{
					// SHUnion
					long dataSize = reader.BaseStream.Length - reader.BaseStream.Position;

					retVal = typeArgs
						.Where(type => Marshal.SizeOf(type) == dataSize)
						.Select(type =>
						{
							object union = Activator.CreateInstance(typeOfValue);
							typeOfValue.GetProperty("Type").SetValue(union, type, null);
							typeOfValue.GetProperty("Value").SetValue(union, reader.ReadStruct(type), null);
							return union;
						})
						.FirstOrDefault();

					if (retVal == null)
					{
						throw new IOException($"The available stream data does not match the size one of the two union types for property '{field?.Name ?? typeOfValue.FullName}'.");
					}
				}
			}
			// FIX: for types containing color, ReadStruct can't read the entire type. So in case a class, enumerate all class properties separately.
			else if (!typeOfValue.IsClass)
			{
				retVal = reader.ReadStruct(typeOfValue);
			}
			else
			{
				FieldInfo[] fields = typeOfValue.GetFields(BindingFlags.Public | BindingFlags.Instance);
				retVal = Activator.CreateInstance(typeOfValue);
				foreach (FieldInfo f in fields)
				{
					if (f.FieldType.IsClass)
					{
						// TODO: if field type is class, drill down, to support nesting.
					}

					f.SetValue(retVal, reader.ReadStruct(f.FieldType));
				}
			}

			if (retVal == null)
			{
				throw new NotImplementedException($"The specified type '{typeOfValue.FullName}' is not supported or implemented.");
			}

			return retVal;
		}

		protected virtual void WriteValue(BinaryWriter writer, MemberInfo memberInfo, object value)
		{
			var field = memberInfo as FieldInfo;
			Type typeOfValue = field?.FieldType ?? (Type)memberInfo;

			if (typeOfValue == typeof(string))
			{
				int fixedLength = memberInfo.GetAttribute<FixedStringAttribute>()?.Length ?? -1;

				var s = (string)value;
				if (fixedLength > 0)
				{
					if (s == null)
					{
						s = string.Empty;
					}

					if (s.Length > fixedLength)
					{
						throw new ArgumentException($"The string '{s}' for property '{memberInfo.Name}' exceeds the fixed length {fixedLength}", nameof(value));
					}

					// Write the fixed string with zeros at the end.
					writer.Write(s, fixedLength);
				}
				else
				{
					if (s != null)
						// Write the variable string with one zero.
					{
						writer.Write(s, '\0');
					}
				}
			}
			else if (typeOfValue == typeof(Color))
			{
				writer.WriteStruct(Color.FromArgb(0, (Color)value));
			}
			else if (typeOfValue.IsArray)
			{
				if (typeOfValue == typeof(byte[]))
				{
					var b = (byte[])value;
					writer.Write(b, 0, b.Length);
				}
				else
				{
					// Parse as a list.
					Type elementType = typeOfValue.GetElementType();
					var arr = (Array)value;
					for (var i = 0; i < arr.Length; i++)
					{
						WriteField(writer, elementType, arr.GetValue(i));
					}
				}
			}
			else if (typeOfValue == typeof(DateTime))
			{
				var dt = (DateTime)value;
				string sDate = dt.ToString("yyyyMMdd");
				int iDate = int.Parse(sDate);
				writer.Write(iDate);
			}
			else if (typeOfValue.IsNullable())
			{
				writer.WriteStruct(value);
			}
			else if (typeOfValue.IsGenericType)
			{
				if (value is IList list)
				{
					WriteList(writer, list);
				}
				else
				{
					// SHUnion
					writer.WriteStruct(typeOfValue.GetProperty("Value").GetValue(value, null));
				}
			}
			// FIX: for types containing color, ReadStruct can't read the entire type. So in case a class, enumerate all class properties separately.
			else if (!typeOfValue.IsClass)
			{
				writer.WriteStruct(value);
			}
			else
			{
				FieldInfo[] fields = typeOfValue.GetFields(BindingFlags.Public | BindingFlags.Instance);
				foreach (FieldInfo f in fields)
				{
					if (f.FieldType.IsClass)
					{
						// TODO: if field type is class, drill down, to support nesting.
					}

					writer.WriteStruct(f.GetValue(value));
				}
			}
		}

		protected virtual IList ReadList(BinaryReader reader, Type typeOfValue)
		{
			if (reader == null)
			{
				throw new ArgumentNullException(nameof(reader));
			}

			// There are two types of lists. The only difference is that for RawList, the number of items is stored right before each entry. The type of this count-field is determined by the 2nd generic parameter. See RawList for info.

			Type[] typeArgs = typeOfValue.GetGenericArguments();
			var col = (IList)Activator.CreateInstance(typeOfValue);
			Type elementType = typeArgs[0];

			if (typeof(IRawList).IsAssignableFrom(typeOfValue))
			{
				int count = Convert.ToInt32(reader.ReadStruct(typeArgs[1]));
				for (var i = 0; i < count; i++)
				{
					col.Add(ReadField(reader, elementType));
				}
			}
			else
			{
				while (reader.BaseStream.Position < reader.BaseStream.Length)
				{
					col.Add(ReadField(reader, elementType));
				}
			}

			return col;
		}

		protected virtual void WriteList(BinaryWriter writer, IList instance)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}

			// There are two types of lists. The only difference is that for RawList, the number of items is stored right before each entry. The type of this count-field is determined by the 2nd generic parameter. See RawList for info.

			Type typeOfValue = instance.GetType();
			Type[] typeArgs = typeOfValue.GetGenericArguments();
			Type elementType = typeArgs[0];

			if (typeof(IRawList).IsAssignableFrom(typeOfValue))
			{
				object count = Convert.ChangeType(instance.Count, typeArgs[1]);
				writer.WriteStruct(count);
			}

			foreach (object item in instance)
			{
				WriteField(writer, elementType, item);
			}
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