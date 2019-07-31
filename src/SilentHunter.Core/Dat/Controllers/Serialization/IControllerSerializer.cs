using System;
using System.IO;
using System.Reflection;

namespace SilentHunter.Dat.Controllers.Serialization
{
	public interface IControllerSerializer
	{
		void Deserialize(Stream stream, IRawController controller);
		void Serialize(Stream stream, IRawController controller);
		object ReadField(BinaryReader reader, Type elementType);
		void WriteField(BinaryWriter writer, Type elementType, object value);
	}
}
