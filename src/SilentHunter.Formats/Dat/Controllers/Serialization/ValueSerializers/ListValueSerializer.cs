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
		private readonly IControllerFieldSerializer _controllerFieldSerializer;

		public ListValueSerializer(IControllerFieldSerializer controllerFieldSerializer)
		{
			_controllerFieldSerializer = controllerFieldSerializer ?? throw new ArgumentNullException(nameof(controllerFieldSerializer));
		}

		public bool IsSupported(ControllerSerializationContext context)
		{
			return context.Type.IsClosedTypeOf(typeof(IList<>));
		}

		public void Serialize(BinaryWriter writer, ControllerSerializationContext serializationContext, object value)
		{
			var list = (IList)value;

			Type[] typeArgs = serializationContext.Type.GetGenericArguments();
			Type elementType = typeArgs[0];
			var innerContext = new ControllerSerializationContext(elementType);

			Type countType = serializationContext.Member.GetCustomAttribute<CountTypeAttribute>()?.SerializationType;
			if (countType != null)
			{
				// Output count field.
				object count = Convert.ChangeType(list.Count, countType);
				writer.WriteStruct(count);
			}

			foreach (object item in list)
			{
				_controllerFieldSerializer.WriteField(writer, innerContext, item);
			}
		}

		public object Deserialize(BinaryReader reader, ControllerSerializationContext serializationContext)
		{
			var list = (IList)Activator.CreateInstance(serializationContext.Type);

			Type[] typeArgs = serializationContext.Type.GetGenericArguments();
			Type elementType = typeArgs[0];
			var innerContext = new ControllerSerializationContext(elementType);

			Type countType = serializationContext.Member.GetCustomAttribute<CountTypeAttribute>()?.SerializationType;
			if (countType != null)
			{
				// Read n times, determined by prefixed count field.
				int count = Convert.ToInt32(reader.ReadStruct(countType));
				for (var i = 0; i < count; i++)
				{
					list.Add(_controllerFieldSerializer.ReadField(reader, innerContext));
				}
			}
			else
			{
				// Read until end of stream.
				while (reader.BaseStream.Position < reader.BaseStream.Length)
				{
					list.Add(_controllerFieldSerializer.ReadField(reader, innerContext));
				}
			}

			return list;
		}
	}
}