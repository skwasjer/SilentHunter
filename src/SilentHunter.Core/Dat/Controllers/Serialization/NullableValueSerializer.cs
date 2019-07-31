using System;
using System.IO;
using skwas.IO;

namespace SilentHunter.Dat.Controllers.Serialization
{
	public class NullableValueSerializer : IControllerValueSerializer
	{
		private readonly BooleanValueSerializer _booleanValueSerializer = new BooleanValueSerializer();

		public bool IsSupported(IControllerSerializationContext context)
		{
			return Nullable.GetUnderlyingType(context.Type) != null;
		}

		public void Serialize(BinaryWriter writer, ControllerSerializationContext serializationContext)
		{
			Type underlyingType = Nullable.GetUnderlyingType(serializationContext.Type);
			if (underlyingType == typeof(bool))
			{
				_booleanValueSerializer.Serialize(writer, serializationContext);
			}
			else
			{
				writer.WriteStruct(serializationContext.Value);
			}
		}

		public object Deserialize(BinaryReader reader, ControllerDeserializationContext deserializationContext)
		{
			Type underlyingType = Nullable.GetUnderlyingType(deserializationContext.Type);
			return underlyingType == typeof(bool) ? _booleanValueSerializer.Deserialize(reader, deserializationContext) : reader.ReadStruct(underlyingType);
		}
	}
}
