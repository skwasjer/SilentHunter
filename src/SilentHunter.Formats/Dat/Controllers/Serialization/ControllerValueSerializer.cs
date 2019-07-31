using System.IO;

namespace SilentHunter.Dat.Controllers.Serialization
{
	public abstract class ControllerValueSerializer<T> : IControllerValueSerializer
	{
		public virtual bool IsSupported(ControllerSerializationContext context)
		{
			return context.Type == typeof(T);
		}

		void IControllerValueSerializer.Serialize(BinaryWriter writer, ControllerSerializationContext serializationContext, object value)
		{
			Serialize(writer, serializationContext, (T)value);
		}

		public abstract void Serialize(BinaryWriter writer, ControllerSerializationContext serializationContext, T value);

		object IControllerValueSerializer.Deserialize(BinaryReader reader, ControllerSerializationContext serializationContext)
		{
			return Deserialize(reader, serializationContext);
		}

		public abstract T Deserialize(BinaryReader reader, ControllerSerializationContext serializationContext);
	}
}