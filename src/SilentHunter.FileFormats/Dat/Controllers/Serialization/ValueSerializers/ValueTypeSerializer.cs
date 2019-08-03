using System.IO;
using skwas.IO;

namespace SilentHunter.FileFormats.Dat.Controllers.Serialization
{
	/// <summary>
	/// Serializer that reads/writes data as structs.
	/// </summary>
	/// <remarks>
	/// Because value types can contain primitives that are not (de)serializable with ReadStruct/WriteStruct, we have to enumerate all fields separately then.
	///
	/// </remarks>
	public class ValueTypeSerializer : IControllerValueSerializer
	{
		public bool IsSupported(ControllerSerializationContext context)
		{
			return context.Type.IsPrimitive || context.Type.IsEnum || context.Type.IsValueType;
		}

		public void Serialize(BinaryWriter writer, ControllerSerializationContext serializationContext, object value)
		{
			writer.WriteStruct(value);
		}

		public object Deserialize(BinaryReader reader, ControllerSerializationContext serializationContext)
		{
			return reader.ReadStruct(serializationContext.Type);
		}
	}
}
