using System;
using System.IO;
using System.Reflection;

namespace SilentHunter.Dat.Controllers.Serialization
{
	public interface IControllerSerializer
	{
		void Deserialize(Stream stream, Type controllerType, object instance);
		void Serialize(Stream stream, Type controllerType, object instance);
		void DeserializeFields(BinaryReader reader, Type typeOfValue, object instance);
		void SerializeFields(BinaryWriter writer, Type typeOfValue, object instance);
		object ReadField(BinaryReader reader, MemberInfo memberInfo);
		void WriteField(BinaryWriter writer, MemberInfo memberInfo, object instance);
		object ReadValue(BinaryReader reader, MemberInfo memberInfo);
		void WriteValue(BinaryWriter writer, MemberInfo memberInfo, object value);
	}
}
