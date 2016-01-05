using System;
using System.IO;
using System.Reflection;
using skwas.IO;
using SilentHunter.Formats;

namespace SilentHunter.Dat
{
	/// <summary>
	/// Represents controller data. <see cref="Controller"/> extends <see cref="RawController"/> by implementing the most common parsing rules for alot of the game controllers, where the name of each property is stored with the value. See remarks for more info.
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
	/// 
	/// Every type that inherits from <see cref="Controller"/> will be parsed following these rules.
	/// </remarks>
	public abstract class Controller
		: RawController, IController
	{
		#region Overrides of RawController

		/// <summary>
		/// When implemented, deserializes the controller from specified <paramref name="stream"/>.
		/// </summary>
		/// <param name="stream">The stream.</param>
		protected override void Deserialize(Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException(nameof(stream));

			using (var reader = new BinaryReader(stream, Encoding.ParseEncoding, true))
			{
				// Read the size of the controller.
				var size = reader.ReadInt32();
				// Save current position of stream. At the end, we compare the size with the number of bytes read for validation purposes.
				var startPos = stream.Position;

				var controllerType = GetType();
				var controllerName = controllerType.Name;
				reader.SkipMember(controllerType, controllerName);

				DeserializeFields(reader, controllerType, this);

				stream.EnsureStreamPosition(startPos + size, controllerName);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="typeOfValue"></param>
		/// <param name="instance"></param>
		/// <exception cref="ArgumentNullException">Thrown for arguments that can't be null.</exception>
		/// <exception cref="IOException">Thrown for any IO error or parsing error.</exception>
		/// <exception cref="NotSupportedException">Thrown when <paramref name="typeOfValue"/> is not supported.</exception>
		/// <exception cref="NotImplementedException">Thrown when <see cref="Array"/>s array used in the controller definitions.</exception>
		protected override void DeserializeFields(BinaryReader reader, Type typeOfValue, object instance)
		{
			if (reader == null)
				throw new ArgumentNullException(nameof(reader));
			if (typeOfValue == null)
				throw new ArgumentNullException(nameof(typeOfValue));
			if (instance == null)
				throw new ArgumentNullException(nameof(instance));

			if (!typeOfValue.IsControllerOrSHType())
				throw new NotSupportedException($"Unsupported type '{typeOfValue}'");

			var fields = typeOfValue.GetFields(BindingFlags.Public | BindingFlags.Instance);
			foreach (var fld in fields)
			{				
				var fieldValue = ReadField(reader, fld);
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
		/// <param name="memberInfo">A <see cref="FieldInfo"/> for field members or a <see cref="Type"/> for controllers or name prefixed types.</param>
		/// <returns>Returns the object read from stream.</returns>
		/// <exception cref="IOException">Thrown when an error occurred during parsing.</exception>
		/// <exception cref="NotImplementedException">Thrown when a type is not implemented.</exception>
		protected override object ReadField(BinaryReader reader, MemberInfo memberInfo)
		{
			CheckArguments(reader, memberInfo);

			var field = memberInfo as FieldInfo;
			var typeOfValue = field?.FieldType ?? (Type)memberInfo;

			if ((field?.HasAttribute<OptionalAttribute>() ?? false) && reader.BaseStream.Position == reader.BaseStream.Length)
			{
				// Exit because field is optional, and we are at end of stream.
				return null;
			}

			// The expected field or controller name to find on the stream. If null, then the name is not required and the object instead is stored as raw data, and name checking is ignored.
			var name = field?.GetAttribute<ParseNameAttribute>()?.Name ?? field?.Name;

			// Read the size of the data.
			var size = reader.ReadInt32();
			// Save current position of stream. At the end, we compare the size with the number of bytes read for validation purposes.
			var startPos = reader.BaseStream.Position;

			if (!string.IsNullOrEmpty(name) && reader.SkipMember(memberInfo, name))
			{
				// The property must be skipped, so revert stream back to the start position.
				reader.BaseStream.Position = startPos - 4;
				return null;
			}

			var expectedPosition = startPos + size;
			var dataSize = expectedPosition - reader.BaseStream.Position;
			using (var regionStream = new RegionStream(reader.BaseStream, dataSize))
			{
				using (var regionReader = new BinaryReader(regionStream, Encoding.ParseEncoding, true))
				{
					var retVal = base.ReadField(regionReader, memberInfo);
					reader.BaseStream.EnsureStreamPosition(expectedPosition, name ?? typeOfValue.FullName);
					return retVal;
				}
			}
		}

		/// <summary>
		/// When implemented, serializes the controller to specified <paramref name="stream"/>.
		/// </summary>
		/// <param name="stream">The stream.</param>
		protected override void Serialize(Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException(nameof(stream));

			using (var writer = new BinaryWriter(stream, Encoding.ParseEncoding, true))
			{
				// We don't know the size yet, so just write 0 for now.
				writer.Write(0);
				
				// Save current position of stream. At the end, we have to set the size.
				var startPos = stream.Position;

				var controllerType = GetType();
				var controllerName = controllerType.Name;
				writer.WriteNullTerminatedString(controllerName);

				SerializeFields(writer, controllerType, this);

				// After the object is written, determine and write the size.
				var currentPos = stream.Position;
				stream.Position = startPos - 4;
				writer.Write((int)(currentPos - startPos));

				// Restore position to the end of the controller.
				stream.Position = currentPos;
			}
		}

		protected override void SerializeFields(BinaryWriter writer, Type typeOfValue, object instance)
		{
			if (writer == null)
				throw new ArgumentNullException(nameof(writer));
			if (typeOfValue == null)
				throw new ArgumentNullException(nameof(typeOfValue));
			if (instance == null)
				throw new ArgumentNullException(nameof(instance));

			if (!typeOfValue.IsControllerOrSHType())
				throw new NotSupportedException($"Unsupported type '{typeOfValue}'");

			var fields = typeOfValue.GetFields(BindingFlags.Public | BindingFlags.Instance);
			foreach (var fld in fields)
			{
				var fieldValue = fld.GetValue(instance);

				// If the value is null, the property has be optional, otherwise throw error.
				if (fieldValue == null && fld.FieldType != typeof(string))
				{
					if (fld.HasAttribute<OptionalAttribute>()) continue;
					var fieldName = fld.GetAttribute<ParseNameAttribute>()?.Name ?? fld.Name;
					throw new IOException($"The field '{fieldName}' is not defined as optional.");
				}

				WriteField(writer, fld, fieldValue);
			}
		}

		protected override void WriteField(BinaryWriter writer, MemberInfo memberInfo, object instance)
		{
			// Write size 0. We don't know actual the size yet.
			writer.Write(0);

			var startPos = writer.BaseStream.Position;

			var field = memberInfo as FieldInfo;
			var typeOfValue = field?.FieldType ?? (Type)memberInfo;

			var name = field?.GetAttribute<ParseNameAttribute>()?.Name ?? field?.Name;
			if (!string.IsNullOrEmpty(name))
				writer.WriteNullTerminatedString(name);

			base.WriteField(writer, memberInfo, instance);

			var currentPos = writer.BaseStream.Position;
			writer.BaseStream.Position = startPos - 4;
			writer.Write((int)(currentPos - startPos));
			writer.BaseStream.Position = currentPos;
		}

		#endregion
	}
}
