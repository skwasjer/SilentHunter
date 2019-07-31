using System.IO;
using SilentHunter.Extensions;
using skwas.IO;

namespace SilentHunter.Dat.Controllers.Serialization
{
	public class StringValueSerializer : ControllerValueSerializer<string>
	{
		public override bool IsSupported(ControllerSerializationContext context)
		{
			return base.IsSupported(context) && !context.Member.HasAttribute<FixedStringAttribute>();
		}

		public override void Serialize(BinaryWriter writer, ControllerSerializationContext serializationContext, string value)
		{
			if (value != null)
			{
				// Write the variable string with one zero.
				writer.Write((string)value, '\0');
			}
		}

		public override string Deserialize(BinaryReader reader, ControllerSerializationContext serializationContext)
		{
			if (reader.BaseStream.Position == reader.BaseStream.Length)
			{
				return null;
			}

			return reader.ReadNullTerminatedString();
		}
	}
}