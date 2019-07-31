using System.IO;

namespace SilentHunter.Dat.Controllers.Serialization
{
	public interface IControllerFieldSerializer
	{
		object ReadField(BinaryReader reader, ControllerDeserializationContext deserializationContext);
		void WriteField(BinaryWriter writer, ControllerSerializationContext serializationContext);
	}
}