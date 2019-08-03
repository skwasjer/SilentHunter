using System.IO;

namespace SilentHunter.FileFormats.Dat.Controllers.Serialization
{
	public interface IControllerFieldSerializer
	{
		object ReadField(BinaryReader reader, ControllerSerializationContext serializationContext);
		void WriteField(BinaryWriter writer, ControllerSerializationContext serializationContext, object value);
	}
}