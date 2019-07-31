using System;
using System.IO;
using System.Reflection;

namespace SilentHunter.Dat.Controllers.Serialization
{
	public interface IControllerSerializer
	{
		void Deserialize(Stream stream, Type typeOfValue, object instance);
		void Serialize(Stream stream, Type valueOfType, object instance);
		object ReadField(BinaryReader reader, MemberInfo memberInfo);
		void WriteField(BinaryWriter writer, MemberInfo memberInfo, object instance);
	}
}
