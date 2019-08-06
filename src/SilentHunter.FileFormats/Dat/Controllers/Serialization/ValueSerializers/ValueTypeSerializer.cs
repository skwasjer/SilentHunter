using System.IO;
using SilentHunter.FileFormats.Extensions;

namespace SilentHunter.FileFormats.Dat.Controllers.Serialization
{
	/// <summary>
	/// Serializes and deserializes value types.
	/// </summary>
	public class ValueTypeSerializer : IControllerValueSerializer
	{
		/// <inheritdoc />
		public bool IsSupported(ControllerSerializationContext context)
		{
			return context.Type.IsValueType;
		}

		/// <inheritdoc />
		public void Serialize(BinaryWriter writer, ControllerSerializationContext serializationContext, object value)
		{
			writer.WriteStruct(value);
		}

		/// <inheritdoc />
		public object Deserialize(BinaryReader reader, ControllerSerializationContext serializationContext)
		{
			return reader.ReadStruct(serializationContext.Type);
		}
	}
}
