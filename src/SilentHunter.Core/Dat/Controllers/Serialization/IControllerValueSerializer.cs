using System.IO;

namespace SilentHunter.Dat.Controllers.Serialization
{
	public interface IControllerValueSerializer
	{
		bool IsSupported(IControllerSerializationContext context);

		void Serialize(BinaryWriter writer, ControllerSerializationContext context);

		object Deserialize(BinaryReader reader, ControllerDeserializationContext context);
	}
}