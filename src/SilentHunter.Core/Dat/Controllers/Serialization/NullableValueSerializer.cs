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

		public void Serialize(BinaryWriter writer, ControllerSerializationContext context)
		{
			Type underlyingType = Nullable.GetUnderlyingType(context.Type);
			if (underlyingType == typeof(bool))
			{
				_booleanValueSerializer.Serialize(writer, context);
			}
			else
			{
				writer.WriteStruct(context.Value);
			}
		}

		public object Deserialize(BinaryReader reader, ControllerDeserializationContext context)
		{
			Type underlyingType = Nullable.GetUnderlyingType(context.Type);
			return underlyingType == typeof(bool) ? _booleanValueSerializer.Deserialize(reader, context) : reader.ReadStruct(underlyingType);
		}
	}
}
