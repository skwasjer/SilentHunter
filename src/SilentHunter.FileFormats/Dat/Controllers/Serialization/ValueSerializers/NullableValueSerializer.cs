using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SilentHunter.FileFormats.Extensions;

namespace SilentHunter.FileFormats.Dat.Controllers.Serialization
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

			var innerContext = new ControllerSerializationContext(underlyingType, serializationContext.Controller);
			IControllerValueSerializer serializer = _serializers.FirstOrDefault(s => s.IsSupported(innerContext));

			return serializer != null
				? serializer.Deserialize(reader, serializationContext)
				: reader.ReadStruct(underlyingType);
		}
	}
}