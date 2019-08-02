using System;
using System.IO;
using System.Reflection;
using SilentHunter.Controllers.Decoration;
using SilentHunter.Extensions;
using skwas.IO;

namespace SilentHunter.Dat.Controllers.Serialization
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
		protected override void Deserialize(BinaryReader reader, Type instanceType, object instance)
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

		protected override object DeserializeField(BinaryReader reader, FieldInfo fieldInfo)
		{
			object value = base.DeserializeField(reader, fieldInfo);
			// Strings can have null are always optional.
			if (value != null || fieldInfo.FieldType == typeof(string))
			{
				return value;
			}

			// If null is returned, check if the field is optional and if the type supports a nullable type.
			if (fieldInfo.FieldType.IsValueType && !fieldInfo.FieldType.IsNullable())
			{
				throw new IOException(
					$"The property '{fieldInfo.Name}' is defined as optional, but the type '{fieldInfo.FieldType}' does not support null values. Use Nullable<> if the property is a value type, or a class otherwise.");
			}

			return null;
		}

		/// <summary>
		/// Reads the next field or controller from the stream.
		/// </summary>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="memberInfo">A <see cref="FieldInfo" /> for field members or a <see cref="Type" /> for controllers or name prefixed types.</param>
		/// <returns>Returns the object read from stream.</returns>
		/// <exception cref="IOException">Thrown when an error occurred during parsing.</exception>
		/// <exception cref="NotImplementedException">Thrown when a type is not implemented.</exception>
		protected override object ReadField(BinaryReader reader, ControllerSerializationContext serializationContext)
		{
			string name = null;
			var field = serializationContext.Member as FieldInfo;
			if (field != null)
			{
				if (field.HasAttribute<OptionalAttribute>() && reader.BaseStream.Position == reader.BaseStream.Length)
				{
					// Exit because field is optional, and we are at end of stream.
					return null;
				}

				// The expected field or controller name to find on the stream. If null, then the name is not required and the object instead is stored as raw data, and name checking is ignored.
				name = field.GetCustomAttribute<ParseNameAttribute>()?.Name ?? field.Name;
			}

			// Read the size of the data.
			int size = reader.ReadInt32();
			// Save current position of stream. At the end, we compare the size with the number of bytes read for validation purposes.
			long startPos = reader.BaseStream.Position;

			if (field != null && !string.IsNullOrEmpty(name) && reader.SkipMember(field, name))
			{
				// The property must be skipped, so revert stream back to the start position.
				reader.BaseStream.Position = startPos - 4;
				return null;
			}

			long expectedPosition = startPos + size;
			long dataSize = expectedPosition - reader.BaseStream.Position;
			using (var regionStream = new RegionStream(reader.BaseStream, dataSize))
			{
				using (var regionReader = new BinaryReader(regionStream, FileEncoding.Default, true))
				{
					object retVal = base.ReadField(regionReader, serializationContext);
					reader.BaseStream.EnsureStreamPosition(expectedPosition, name ?? serializationContext.Name);
					return retVal;
				}
			}
		}

		/// <summary>
		/// When implemented, serializes the controller to specified <paramref name="stream" />.
		/// </summary>
		/// <param name="stream">The stream.</param>
		protected override void Serialize(BinaryWriter writer, Type instanceType, object instance)
		{
			// We don't know the size yet, so just write 0 for now.
			writer.Write(0);

			// Save current position of stream. At the end, we have to set the size.
			long startPos = writer.BaseStream.Position;

			string controllerName = instanceType.Name;
			writer.WriteNullTerminatedString(controllerName);

			base.Serialize(writer, instanceType, instance);

			// After the object is written, determine and write the size.
			long currentPos = writer.BaseStream.Position;
			writer.BaseStream.Position = startPos - 4;
			writer.Write((int)(currentPos - startPos));

			// Restore position to the end of the controller.
			writer.BaseStream.Position = currentPos;
		}

		protected override void SerializeField(BinaryWriter writer, FieldInfo field, object value)
		{
			// If the value is null, the property has be optional, otherwise throw error.
			if (value == null && field.FieldType != typeof(string))
			{
				if (field.HasAttribute<OptionalAttribute>())
				{
					return;
				}

				string fieldName = field.GetCustomAttribute<ParseNameAttribute>()?.Name ?? field.Name;
				throw new IOException($"The field '{fieldName}' is not defined as optional.");
			}

			base.SerializeField(writer, field, value);
		}

		protected override void WriteField(BinaryWriter writer, ControllerSerializationContext serializationContext, object value)
		{
			// Write size 0. We don't know actual the size yet.
			writer.Write(0);

			long startPos = writer.BaseStream.Position;

			if (serializationContext.Member is FieldInfo fieldInfo)
			{
				string name = fieldInfo.GetCustomAttribute<ParseNameAttribute>()?.Name ?? fieldInfo.Name;
				writer.WriteNullTerminatedString(name);
			}

			base.WriteField(writer, serializationContext, value);

			// Rewind and store size.
			long currentPos = writer.BaseStream.Position;
			writer.BaseStream.Position = startPos - 4;
			writer.Write((int)(currentPos - startPos));
			writer.BaseStream.Position = currentPos;
		}
	}
}