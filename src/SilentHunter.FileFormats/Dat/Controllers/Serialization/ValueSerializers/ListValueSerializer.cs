using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SilentHunter.Controllers;
using SilentHunter.FileFormats.Extensions;

namespace SilentHunter.FileFormats.Dat.Controllers.Serialization
{
	/// <summary>
	/// Serializes and deserializes lists/arrays.
	/// </summary>
	/// <remarks>
	/// There are two types of lists in the DAT-file format. One is prefixed by a count field indicating the number of items, where the other is not.
	///
	/// The first method is specifically for animation controllers.
	/// </remarks>
	public class ListValueSerializer : IControllerValueSerializer
	{
		private readonly IControllerFieldSerializer _controllerFieldSerializer;

		/// <summary>
		/// Initializes a new instance of the <see cref="ListValueSerializer"/>.
		/// </summary>
		/// <param name="controllerFieldSerializer">The field serializer.</param>
		public ListValueSerializer(IControllerFieldSerializer controllerFieldSerializer)
		{
			_controllerFieldSerializer = controllerFieldSerializer ?? throw new ArgumentNullException(nameof(controllerFieldSerializer));
		}

		/// <inheritdoc />
		public bool IsSupported(ControllerSerializationContext context)
		{
			return context.Type.IsClosedTypeOf(typeof(IList<>));
		}

		/// <inheritdoc />
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

		/// <inheritdoc />
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