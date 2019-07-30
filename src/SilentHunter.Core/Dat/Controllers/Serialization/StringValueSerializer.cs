using System;
using System.IO;
using skwas.IO;

namespace SilentHunter.Dat.Controllers.Serialization
{
	public class StringValueSerializer : ControllerValueSerializer<string>
	{
		public override void Serialize(BinaryWriter writer, ControllerSerializationContext context)
		{
			if (context.Value != null)
			{
				// Write the variable string with one zero.
				writer.Write((string)context.Value, '\0');
			}
		}

		public override object Deserialize(BinaryReader reader, ControllerDeserializationContext context)
		{
			if (reader.BaseStream.Position == reader.BaseStream.Length)
			{
				return null;
			}

			return reader.ReadNullTerminatedString();
		}
	}
}