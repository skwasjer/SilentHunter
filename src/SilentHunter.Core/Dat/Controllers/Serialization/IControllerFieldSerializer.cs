using System.IO;

namespace SilentHunter.Dat.Controllers.Serialization
{
	public interface IControllerFieldSerializer
	{
		object ReadField(BinaryReader reader, ControllerSerializationContext serializationContext);
		void WriteField(BinaryWriter writer, ControllerSerializationContext serializationContext, object value);
	}
}