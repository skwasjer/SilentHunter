using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SilentHunter.Formats;

namespace SilentHunter.Dat.Controllers.Serialization
{
	//public class ListValueSerializer : IControllerValueSerializer
	//{
	//	public bool IsSupported(IControllerSerializationContext context)
	//	{
	//		return context.Type.IsClosedTypeOf(typeof(IList<>));
	//	}

	//	public void Serialize(BinaryWriter writer, ControllerSerializationContext context)
	//	{
	//		throw new NotImplementedException();
	//	}

	//	public object Deserialize(BinaryReader reader, ControllerDeserializationContext context)
	//	{
	//		var list = (IList)Activator.CreateInstance(context.Type);

	//		Type[] typeArgs = context.Type.GetGenericArguments();
	//		Type elementType = typeArgs[0];

	//		while (reader.BaseStream.Position < reader.BaseStream.Length)
	//		{
	//			list.Add(ReadField(reader, elementType));
	//		}
	//	}
	//}
}
