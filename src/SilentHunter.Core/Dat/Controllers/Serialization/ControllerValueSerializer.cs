using System.IO;

namespace SilentHunter.Dat.Controllers.Serialization
{
	public abstract class ControllerValueSerializer<T> : IControllerValueSerializer
	{
		public virtual bool IsSupported(IControllerSerializationContext context)
		{
			return context.Type == typeof(T);
		}

		public abstract void Serialize(BinaryWriter writer, ControllerSerializationContext serializationContext);

		public abstract object Deserialize(BinaryReader reader, ControllerDeserializationContext deserializationContext);
	}
}