using System.IO;
using SilentHunter.FileFormats.Extensions;

namespace SilentHunter.FileFormats.Dat.Controllers.Serialization
{
	/// <summary>
	/// 
	/// </summary>
	/// <remarks>Alpha component is not used, but generally stored as 0.</remarks>
	public class ColorValueSerializer : ControllerValueSerializer<Color>
	{
		public override void Serialize(BinaryWriter writer, ControllerSerializationContext serializationContext, Color value)
		{
			writer.WriteStruct(Color.FromArgb(0, value));
		}

		public override Color Deserialize(BinaryReader reader, ControllerSerializationContext serializationContext)
		{
			return Color.FromArgb(byte.MaxValue, reader.ReadStruct<Color>());
		}
	}
}
