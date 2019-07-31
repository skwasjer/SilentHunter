using System.IO;

namespace SilentHunter.Dat.Controllers.Serialization
{
	public interface IControllerValueSerializer
	{
		bool IsSupported(ControllerSerializationContext context);

		void Serialize(BinaryWriter writer, ControllerSerializationContext serializationContext, object value);

		object Deserialize(BinaryReader reader, ControllerSerializationContext serializationContext);
	}
}