using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using skwas.IO;
using SilentHunter.Formats;

namespace SilentHunter.Dat
{
	/// <summary>
	/// Represents controller data. <see cref="RawController"/> implements basic (de)serialization rules, where each property is stored anonymously in a sequential order, much like C-structs in memory.
	/// </summary>
	/// <remarks>
	/// Raw controllers are typically stored where each property is serialized in binary form, in a sequential order.
	/// 
	/// Every type that inherits from <see cref="RawController"/> will be parsed following these rules.
	/// </remarks>
	public abstract class RawController
		: IRawController
	{
		/// <summary>
		/// When implemented, deserializes the controller from specified <paramref name="stream"/>.
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
		/// When implemented, serializes the controller to specified <paramref name="stream"/>.
		/// </summary>
		/// <param name="stream">The stream.</param>
		protected virtual void Serialize(Stream stream)
		{
			using (var writer = new BinaryWriter(stream, Encoding.ParseEncoding, true))
				SerializeFields(writer, GetType(), this);
		}

		protected virtual void DeserializeFields(BinaryReader reader, Type typeOfValue, object instance)
		{
			if (reader == null)
				throw new ArgumentNullException(nameof(reader));
			if (typeOfValue == null)
				throw new ArgumentNullException(nameof(typeOfValue));
			if (instance == null)
				throw new ArgumentNullException(nameof(instance));

			var serializable = instance as IRawController;
			if (serializable != null && serializable != this)
			{
				serializable.Deserialize(reader.BaseStream);
			}
			else
			{
				var fields = typeOfValue.GetFields(BindingFlags.Public | BindingFlags.Instance);
				foreach (var fld in fields)
				{
					var fieldValue = ReadField(reader, fld);
					// We have our value, set it.
					fld.SetValue(instance, fieldValue);
				}
			}
		}

		protected virtual void SerializeFields(BinaryWriter writer, Type typeOfValue, object instance)
		{
			if (writer == null)
				throw new ArgumentNullException(nameof(writer));
			if (instance == null)
				throw new ArgumentNullException(nameof(instance));

			var serializable = instance as IRawController;
			if (serializable != null && serializable != this)
			{
				serializable.Serialize(writer.BaseStream);
			}
			else
			{
				var fields = typeOfValue.GetFields(BindingFlags.Public | BindingFlags.Instance);
				foreach (var fld in fields)
				{
					var fieldValue = fld.GetValue(instance);
					// Write the value.
					WriteField(writer, fld, fieldValue);
				}
			}
		}


		protected virtual object ReadField(BinaryReader reader, MemberInfo memberInfo)
		{
			CheckArguments(reader, memberInfo);

			var field = memberInfo as FieldInfo;
			var typeOfValue = field?.FieldType ?? (Type)memberInfo;

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
			var typeOfValue = field?.FieldType ?? (Type)memberInfo;

			if (typeOfValue.IsControllerOrSHType())
				SerializeFields(writer, typeOfValue, instance);
			else
				WriteValue(writer, memberInfo, instance);
		}

		protected virtual object ReadValue(BinaryReader reader, MemberInfo memberInfo, long expectedEndPos = -1)
		{
			CheckArguments(reader, memberInfo);

			var field = memberInfo as FieldInfo;
			var typeOfValue = field?.FieldType ?? (Type)memberInfo;

			object retVal = null;

			if (typeOfValue == typeof(string))
			{
				string str;
				if (reader.ReadString(memberInfo, expectedEndPos, out str)) return str;
			}
			else if (typeOfValue.IsArray)
			{
				throw new NotSupportedException("Arrays are not supported. Use List<> instead.");
			}
			else if (typeOfValue == typeof(DateTime))
			{
				DateTime dt;
				if (reader.ReadDateTime(field, expectedEndPos, out dt)) return dt;
			}
			else if (typeOfValue == typeof(Color))
			{
				// Alpha component is ignored in SH.
				retVal = Color.FromArgb(byte.MaxValue, reader.ReadStruct<Color>());
			}
			else if (typeOfValue == typeof(bool))
			{
				bool bVal;
				if (reader.ReadBoolean(field, expectedEndPos, out bVal)) return bVal;
			}
			else if (typeOfValue.IsNullable())
			{
				retVal = reader.ReadStruct(Nullable.GetUnderlyingType(typeOfValue));
			}
			else if (typeOfValue.IsGenericType)
			{
				var typeArgs = typeOfValue.GetGenericArguments();

				if (typeof(IList).IsAssignableFrom(typeOfValue))
				{
					retVal = ReadList(reader, typeOfValue);
				}
				else
				{
					// SHUnion
					var dataSize = reader.BaseStream.Length - reader.BaseStream.Position;

					retVal = typeArgs
						.Where(type => Marshal.SizeOf(type) == dataSize)
						.Select(type =>
						{
							var union = Activator.CreateInstance(typeOfValue);
							typeOfValue.GetProperty("Type").SetValue(union, type, null);
							typeOfValue.GetProperty("Value").SetValue(union, reader.ReadStruct(type), null);
							return union;
						})
						.FirstOrDefault();

					if (retVal == null)
						throw new IOException($"The available stream data does not match the size one of the two union types for property '{field?.Name ?? typeOfValue.FullName}'.");
				}
			}
			// FIX: for types containing color, ReadStruct can't read the entire type. So in case a class, enumerate all class properties seperately.
			else if (!typeOfValue.IsClass)
				retVal = reader.ReadStruct(typeOfValue);
			else
			{
				var fields = typeOfValue.GetFields(BindingFlags.Public | BindingFlags.Instance);
				retVal = Activator.CreateInstance(typeOfValue);
				foreach (var f in fields)
				{
					if (f.FieldType.IsClass)
					{
						// TODO: if field type is class, drill down, to support nesting.
					}
					f.SetValue(retVal, reader.ReadStruct(f.FieldType));
				}
			}

			if (retVal == null)
				throw new NotImplementedException($"The specified type '{typeOfValue.FullName}' is not supported or implemented.");

			return retVal;
		}

		protected virtual void WriteValue(BinaryWriter writer, MemberInfo memberInfo, object value)
		{
			var field = memberInfo as FieldInfo;
			var typeOfValue = field?.FieldType ?? (Type) memberInfo;

			if (typeOfValue == typeof (string))
			{
				var fixedLength = memberInfo.GetAttribute<FixedStringAttribute>()?.Length ?? -1;

				var s = (string) value;
				if (fixedLength > 0)
				{
					if (s == null) s = string.Empty;
					if (s.Length > fixedLength)
					{
						throw new ArgumentException(string.Format("The string '{0}' for property '{1}' exceeds the fixed length {2}", s, memberInfo.Name, fixedLength), nameof(value));
					}
					// Write the fixed string with zeros at the end.
					writer.Write(s, fixedLength);
				}
				else
				{
					if (s != null)
						// Write the variable string with one zero.
						writer.Write(s, '\0');
				}
			}
			else if (typeOfValue == typeof (Color))
			{
				writer.WriteStruct(Color.FromArgb(0, (Color)value));
			}
			else if (typeOfValue.IsArray)
			{
				if (typeOfValue == typeof (byte[]))
				{
					var b = (byte[]) value;
					writer.Write(b, 0, b.Length);
				}
				else
				{
					// Parse as a list.
					var elementType = typeOfValue.GetElementType();
					var arr = (Array) value;
					for (var i = 0; i < arr.Length; i++)
					{
						WriteField(writer, elementType, arr.GetValue(i));
					}
				}
			}
			else if (typeOfValue == typeof (DateTime))
			{
				var dt = (DateTime) value;
				var sDate = dt.ToString("yyyyMMdd");
				var iDate = int.Parse(sDate);
				writer.Write(iDate);
			}
			else if (typeOfValue.IsNullable())
			{
				writer.WriteStruct(value);
			}
			else if (typeOfValue.IsGenericType)
			{
				var list = value as IList;
				if (list != null)
				{
					WriteList(writer, list);
				}
				else
					writer.WriteStruct(typeOfValue.GetProperty("Value").GetValue(value, null));
			}
			// FIX: for types containing color, ReadStruct can't read the entire type. So in case a class, enumerate all class properties seperately.
			else if (!typeOfValue.IsClass)
				writer.WriteStruct(value);
			else
			{
				var fields = typeOfValue.GetFields(BindingFlags.Public | BindingFlags.Instance);
				foreach (var f in fields)
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
				throw new ArgumentNullException(nameof(reader));

			// There are two types of lists. The only difference is that for RawList, the number of items is stored right before each entry. The type of this count-field is determined by the 2nd generic parameter. See RawList for info.

			var typeArgs = typeOfValue.GetGenericArguments();
			var col = (IList) Activator.CreateInstance(typeOfValue);
			var elementType = typeArgs[0];

			if (typeof (IRawList).IsAssignableFrom(typeOfValue))
			{
				var count = Convert.ToInt32(reader.ReadStruct(typeArgs[1]));
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
				throw new ArgumentNullException(nameof(writer));

			// There are two types of lists. The only difference is that for RawList, the number of items is stored right before each entry. The type of this count-field is determined by the 2nd generic parameter. See RawList for info.

			var typeOfValue = instance.GetType();
			var typeArgs = typeOfValue.GetGenericArguments();
			var elementType = typeArgs[0];

			if (typeof(IRawList).IsAssignableFrom(typeOfValue))
			{
				var count = Convert.ChangeType(instance.Count, typeArgs[1]);
				writer.WriteStruct(count);
			}

			foreach (var item in instance)
			{
				WriteField(writer, elementType, item);
			}
		}

		#region Implementation of IRawController

		/// <summary>
		/// When implemented, deserializes the controller from specified <paramref name="stream"/>.
		/// </summary>
		/// <param name="stream">The stream.</param>
		void IRawController.Deserialize(Stream stream)
		{
			Deserialize(stream);
		}

		/// <summary>
		/// When implemented, serializes the controller to specified <paramref name="stream"/>.
		/// </summary>
		/// <param name="stream">The stream.</param>
		void IRawController.Serialize(Stream stream)
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
				throw new ArgumentNullException(nameof(reader));
			if (memberInfo == null)
				throw new ArgumentNullException(nameof(memberInfo));
			// Member info can be a Type or a FieldInfo.
			if (!(memberInfo is Type || memberInfo is FieldInfo))
				throw new ArgumentException("Expected a Type or FieldInfo.", nameof(memberInfo));
		}
	}
}