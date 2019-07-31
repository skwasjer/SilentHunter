using System;
using System.IO;
using skwas.IO;

namespace SilentHunter.Dat.Controllers.Serialization
{
	// FIX: I think we can remove this.
	public class PrimitiveArrayValueSerializer : IControllerValueSerializer
	{
		public bool IsSupported(IControllerSerializationContext context)
		{
			return context.Type.IsArray && (context.Type.GetElementType()?.IsPrimitive ?? false);
		}

		public void Serialize(BinaryWriter writer, ControllerSerializationContext context)
		{
			if (context.Value is byte[] byteArray)
			{
				// Slightly more efficient to write directly.
				writer.Write(byteArray, 0, byteArray.Length);
				return;
			}

			// Otherwise write each primitive separately.
			var array = (Array)context.Value;
			for (var i = 0; i < array.Length; i++)
			{
				writer.WriteStruct(array.GetValue(i));
			}
		}

		public object Deserialize(BinaryReader reader, ControllerDeserializationContext context)
		{
			throw new NotSupportedException("Arrays are not supported. Use List<> instead.");
		}
	}
}
