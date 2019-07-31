using System;
using System.IO;
using System.Reflection;
using SilentHunter.Extensions;
using skwas.IO;

namespace SilentHunter.Dat.Controllers.Serialization
{
	public class FixedStringValueSerializer : ControllerValueSerializer<string>
	{
		public override bool IsSupported(IControllerSerializationContext context)
		{
			return base.IsSupported(context) && context.Member.HasAttribute<FixedStringAttribute>();
		}

		public override void Serialize(BinaryWriter writer, ControllerSerializationContext context)
		{
			int fixedLength = context.Member.GetCustomAttribute<FixedStringAttribute>().Length;
			string s = (string)context.Value ?? string.Empty;

			if (s.Length > fixedLength)
			{
				throw new InvalidOperationException($"The string '{s}' for property '{context.Member.Name}' exceeds the fixed length {fixedLength}");
			}

			// Write the fixed string with zeros at the end.
			writer.Write(s, fixedLength);
		}

		public override object Deserialize(BinaryReader reader, ControllerDeserializationContext context)
		{
			int fixedLength = context.Member.GetCustomAttribute<FixedStringAttribute>().Length;
			if (reader.BaseStream.Length > fixedLength)
			{
				throw new InvalidOperationException($"The stream contains more data than expected for '{context.Member.Name}', length {fixedLength}.");
			}

			string s = reader.ReadString(fixedLength);
			// Take care of possible '\0' char in middle of string.
			return s.Split(new[] { '\0' }, 2, StringSplitOptions.None)[0];
		}
	}
}