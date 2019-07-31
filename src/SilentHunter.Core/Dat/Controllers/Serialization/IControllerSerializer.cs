using System;
using System.IO;
using System.Reflection;

namespace SilentHunter.Dat.Controllers.Serialization
{
	public interface IControllerSerializer
	{
		void Deserialize(Stream stream, IRawController controller);
		void Serialize(Stream stream, IRawController controller);
		object ReadField(BinaryReader reader, MemberInfo memberInfo);
		void WriteField(BinaryWriter writer, MemberInfo memberInfo, object instance);
	}
}
