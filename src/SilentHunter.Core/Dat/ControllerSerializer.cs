using System;
using System.IO;
using System.Reflection;
using SilentHunter.Extensions;
using SilentHunter.Formats;
using skwas.IO;

namespace SilentHunter.Dat
{
	/// <summary>
	/// Represents raw controller data serializer, which implements (de)serialization rules where properties are stored together with its name.
	/// </summary>
	/// <remarks>
	/// Controllers are typically stored in a specific binary format like so:
	/// 
	///     [int32] size in bytes of property
	///     [string] null terminated string indicating the name of the property.
	///     [array of bytes] property data
	/// 
	/// The array of bytes is typically dictated by the (predefined) type of the property.
	/// F.ex. for simple types a float takes 4 bytes, a boolean 1 byte. For nested more complex types, it contains
	/// the data for the entire type, which can again be enumerated following the same rules.
	/// Lastly, there are also value type structures, which are parsed in binary sequential order.
	/// 
	/// Controller data is stored as an entire tree in this layout.
	/// </remarks>
	public class ControllerSerializer : RawControllerSerializer
	{
		/// <summary>
		/// When implemented, deserializes the controller from specified <paramref name="stream" />.
		/// </summary>
		public override void Deserialize(BinaryReader reader, Type instanceType, object instance)
		{
			// Read the size of the controller.
			int size = reader.ReadInt32();
			// Save current position of stream. At the end, we compare the size with the number of bytes read for validation purposes.
			long startPos = reader.BaseStream.Position;

			string controllerName = instanceType.Name;
			reader.SkipMember(instanceType, controllerName);

			base.Deserialize(reader, instanceType, instance);

			reader.BaseStream.EnsureStreamPosition(startPos + size, controllerName);
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
		protected override void DeserializeFields(BinaryReader reader, Type instanceType, object instance)
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
			foreach (FieldInfo fld in fields)
			{
				object fieldValue = ReadField(reader, fld);
				// If null is returned, the field was optional and not a string, and thus should be skipped.
				if (fieldValue == null && fld.FieldType != typeof(string))
				{
					// Check if the type actually supports a nullable type.
					if (fld.FieldType.IsValueType && !fld.FieldType.IsNullable())
					{
						throw new IOException(
							$"The property '{fld.Name}' is defined as optional, but the type '{fld.FieldType}' does not support null values. Use Nullable<> if the property is a value type, or a class otherwise.");
					}

					continue;
				}

				// We have our value, set it.
				fld.SetValue(instance, fieldValue);
			}
		}

		/// <summary>
		/// Reads the next field or controller from the stream.
		/// </summary>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="memberInfo">A <see cref="FieldInfo" /> for field members or a <see cref="Type" /> for controllers or name prefixed types.</param>
		/// <returns>Returns the object read from stream.</returns>
		/// <exception cref="IOException">Thrown when an error occurred during parsing.</exception>
		/// <exception cref="NotImplementedException">Thrown when a type is not implemented.</exception>
		protected override object ReadField(BinaryReader reader, MemberInfo memberInfo)
		{
			CheckArguments(reader, memberInfo);

			var field = memberInfo as FieldInfo;
			Type typeOfValue = field?.FieldType ?? (Type)memberInfo;

			if ((field?.HasAttribute<OptionalAttribute>() ?? false) && reader.BaseStream.Position == reader.BaseStream.Length)
			{
				// Exit because field is optional, and we are at end of stream.
				return null;
			}

			// The expected field or controller name to find on the stream. If null, then the name is not required and the object instead is stored as raw data, and name checking is ignored.
			string name = field?.GetAttribute<ParseNameAttribute>()?.Name ?? field?.Name;

			// Read the size of the data.
			int size = reader.ReadInt32();
			// Save current position of stream. At the end, we compare the size with the number of bytes read for validation purposes.
			long startPos = reader.BaseStream.Position;

			if (!string.IsNullOrEmpty(name) && reader.SkipMember(memberInfo, name))
			{
				// The property must be skipped, so revert stream back to the start position.
				reader.BaseStream.Position = startPos - 4;
				return null;
			}

			long expectedPosition = startPos + size;
			long dataSize = expectedPosition - reader.BaseStream.Position;
			using (var regionStream = new RegionStream(reader.BaseStream, dataSize))
			{
				using (var regionReader = new BinaryReader(regionStream, Encoding.ParseEncoding, true))
				{
					object retVal = base.ReadField(regionReader, memberInfo);
					reader.BaseStream.EnsureStreamPosition(expectedPosition, name ?? typeOfValue.FullName);
					return retVal;
				}
			}
		}

		/// <summary>
		/// When implemented, serializes the controller to specified <paramref name="stream" />.
		/// </summary>
		/// <param name="stream">The stream.</param>
		public override void Serialize(BinaryWriter writer, Type instanceType, object instance)
		{
			// We don't know the size yet, so just write 0 for now.
			writer.Write(0);

			// Save current position of stream. At the end, we have to set the size.
			long startPos = writer.BaseStream.Position;

			string controllerName = instanceType.Name;
			writer.WriteNullTerminatedString(controllerName);

			SerializeFields(writer, instanceType, instance);

			// After the object is written, determine and write the size.
			long currentPos = writer.BaseStream.Position;
			writer.BaseStream.Position = startPos - 4;
			writer.Write((int)(currentPos - startPos));

			// Restore position to the end of the controller.
			writer.BaseStream.Position = currentPos;
		}

		protected override void SerializeFields(BinaryWriter writer, Type typeOfValue, object instance)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}

			if (typeOfValue == null)
			{
				throw new ArgumentNullException(nameof(typeOfValue));
			}

			if (instance == null)
			{
				throw new ArgumentNullException(nameof(instance));
			}

			if (!typeOfValue.IsControllerOrSHType())
			{
				throw new NotSupportedException($"Unsupported type '{typeOfValue}'");
			}

			FieldInfo[] fields = typeOfValue.GetFields(BindingFlags.Public | BindingFlags.Instance);
			foreach (FieldInfo fld in fields)
			{
				object fieldValue = fld.GetValue(instance);

				// If the value is null, the property has be optional, otherwise throw error.
				if (fieldValue == null && fld.FieldType != typeof(string))
				{
					if (fld.HasAttribute<OptionalAttribute>())
					{
						continue;
					}

					string fieldName = fld.GetAttribute<ParseNameAttribute>()?.Name ?? fld.Name;
					throw new IOException($"The field '{fieldName}' is not defined as optional.");
				}

				WriteField(writer, fld, fieldValue);
			}
		}

		protected override void WriteField(BinaryWriter writer, MemberInfo memberInfo, object instance)
		{
			// Write size 0. We don't know actual the size yet.
			writer.Write(0);

			long startPos = writer.BaseStream.Position;

			var field = memberInfo as FieldInfo;
			//Type typeOfValue = field?.FieldType ?? (Type)memberInfo;

			string name = field?.GetAttribute<ParseNameAttribute>()?.Name ?? field?.Name;
			if (!string.IsNullOrEmpty(name))
			{
				writer.WriteNullTerminatedString(name);
			}

			base.WriteField(writer, memberInfo, instance);

			long currentPos = writer.BaseStream.Position;
			writer.BaseStream.Position = startPos - 4;
			writer.Write((int)(currentPos - startPos));
			writer.BaseStream.Position = currentPos;
		}
	}
}