using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using skwas.IO;

namespace SilentHunter.Dat.Controllers.Serialization
{
	public class NullableValueSerializer : IControllerValueSerializer
	{
		private readonly IList<IControllerValueSerializer> _serializers = new List<IControllerValueSerializer>
		{
			new BooleanValueSerializer(),
			new ColorValueSerializer(),
			new DateTimeValueSerializer()
		};

		public bool IsSupported(ControllerSerializationContext context)
		{
			return Nullable.GetUnderlyingType(context.Type) != null;
		}

		public void Serialize(BinaryWriter writer, ControllerSerializationContext serializationContext, object value)
		{
			if (value == null)
			{
				// TODO: check if null we can actually write...
				return;
			}

			writer.WriteStruct(value);
		}

		public object Deserialize(BinaryReader reader, ControllerSerializationContext serializationContext)
		{
			Type underlyingType = Nullable.GetUnderlyingType(serializationContext.Type);

			IControllerValueSerializer serializer = _serializers.FirstOrDefault(s => s.IsSupported(new ControllerSerializationContext(underlyingType)));

			return serializer != null
				? serializer.Deserialize(reader, serializationContext)
				: reader.ReadStruct(underlyingType);
		}
	}
}