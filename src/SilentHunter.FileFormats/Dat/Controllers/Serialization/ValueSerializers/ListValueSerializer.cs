using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SilentHunter.Controllers;
using SilentHunter.Controllers.Decoration;
using SilentHunter.FileFormats.Extensions;

namespace SilentHunter.FileFormats.Dat.Controllers.Serialization
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
			var innerContext = new ControllerSerializationContext(elementType, serializationContext.Controller);

			if (serializationContext.Controller is AnimationController)
			{
				// Output count field.
				writer.Write(unchecked((ushort)list.Count));
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
			var innerContext = new ControllerSerializationContext(elementType, serializationContext.Controller);

			if (serializationContext.Controller is AnimationController)
			{
				// Read n times, determined by prefixed count field.
				int count = reader.ReadUInt16();
				for (int i = 0; i < count; i++)
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