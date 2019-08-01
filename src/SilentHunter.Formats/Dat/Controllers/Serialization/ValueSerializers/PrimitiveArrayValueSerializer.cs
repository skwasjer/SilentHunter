using System;
using System.IO;
using skwas.IO;

namespace SilentHunter.Dat.Controllers.Serialization
{
	// FIX: I think we can remove this.
	public class PrimitiveArrayValueSerializer : IControllerValueSerializer
	{
		public bool IsSupported(ControllerSerializationContext context)
		{
			return context.Type.IsArray && (context.Type.GetElementType()?.IsPrimitive ?? false);
		}

		public void Serialize(BinaryWriter writer, ControllerSerializationContext serializationContext, object value)
		{
			if (value is byte[] byteArray)
			{
				// Slightly more efficient to write directly.
				writer.Write(byteArray, 0, byteArray.Length);
				return;
			}

			// Otherwise write each primitive separately.
			var array = (Array)value;
			for (var i = 0; i < array.Length; i++)
			{
				writer.WriteStruct(array.GetValue(i));
			}
		}

		public object Deserialize(BinaryReader reader, ControllerSerializationContext serializationContext)
		{
			throw new NotSupportedException("Arrays are not supported. Use List<> instead.");
		}
	}
}
