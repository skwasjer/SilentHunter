using System;
using System.IO;
using skwas.IO;

namespace SilentHunter.Dat.Controllers.Serialization
{
	public class NullableValueSerializer : IControllerValueSerializer
	{
		private readonly BooleanValueSerializer _booleanValueSerializer = new BooleanValueSerializer();

		public bool IsSupported(ControllerSerializationContext context)
		{
			return Nullable.GetUnderlyingType(context.Type) != null;
		}

		public void Serialize(BinaryWriter writer, ControllerSerializationContext serializationContext, object value)
		{
			if (value == null)
			{
				// TODO: check if null we can actually write...
			}
			writer.WriteStruct(value);
		}

		public object Deserialize(BinaryReader reader, ControllerSerializationContext serializationContext)
		{
			Type underlyingType = Nullable.GetUnderlyingType(serializationContext.Type);
			return underlyingType == typeof(bool)
				? _booleanValueSerializer.Deserialize(reader, serializationContext)
				: reader.ReadStruct(underlyingType);
		}
	}
}
