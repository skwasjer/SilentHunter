using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SilentHunter.Extensions;
using skwas.IO;

namespace SilentHunter.Dat.Controllers.Serialization
{
	/// <summary>
	/// 
	/// </summary>
	/// <remarks>
	/// There are two types of lists in the DAT-file format. One is prefixed by a count field indicating the number of items, where the other is not. When the <see cref="CountTypeAttribute"/> is present, we use the count-prefix mode, and otherwise, the default mode.
	///
	/// </remarks>
	public class ListValueSerializer : IControllerValueSerializer
	{
		public bool IsSupported(IControllerSerializationContext context)
		{
			return context.Type.IsClosedTypeOf(typeof(IList<>));
		}

		public void Serialize(BinaryWriter writer, ControllerSerializationContext serializationContext)
		{
			var list = (IList)serializationContext.Value;

			Type[] typeArgs = serializationContext.Type.GetGenericArguments();
			Type elementType = typeArgs[0];

			Type countType = serializationContext.Member.GetCustomAttribute<CountTypeAttribute>()?.SerializationType;
			if (countType != null)
			{
				// Output count field.
				object count = Convert.ChangeType(list.Count, countType);
				writer.WriteStruct(count);
			}

			foreach (object item in list)
			{
				var innerContext = new ControllerSerializationContext(serializationContext.Serializer, elementType, item);
				serializationContext.Serializer.WriteField(writer, innerContext);
			}
		}

		public object Deserialize(BinaryReader reader, ControllerDeserializationContext deserializationContext)
		{
			var list = (IList)Activator.CreateInstance(deserializationContext.Type);

			Type[] typeArgs = deserializationContext.Type.GetGenericArguments();
			Type elementType = typeArgs[0];
			var innerContext = new ControllerDeserializationContext(deserializationContext.Serializer, elementType);

			Type countType = deserializationContext.Member.GetCustomAttribute<CountTypeAttribute>()?.SerializationType;
			if (countType != null)
			{
				// Read n times, determined by prefixed count field.
				int count = Convert.ToInt32(reader.ReadStruct(countType));
				for (var i = 0; i < count; i++)
				{
					list.Add(deserializationContext.Serializer.ReadField(reader, innerContext));
				}
			}
			else
			{
				// Read until end of stream.
				while (reader.BaseStream.Position < reader.BaseStream.Length)
				{
					list.Add(deserializationContext.Serializer.ReadField(reader, innerContext));
				}
			}

			return list;
		}
	}
}