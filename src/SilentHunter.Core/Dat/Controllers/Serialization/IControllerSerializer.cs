using System;
using System.IO;
using System.Reflection;

namespace SilentHunter.Dat.Controllers.Serialization
{
	public interface IControllerSerializer
	{
		void Deserialize(Stream stream, IRawController controller);
		void Serialize(Stream stream, IRawController controller);
		object ReadField(BinaryReader reader, ControllerDeserializationContext deserializationContext);
		void WriteField(BinaryWriter writer, ControllerSerializationContext serializationContext);
	}
}
