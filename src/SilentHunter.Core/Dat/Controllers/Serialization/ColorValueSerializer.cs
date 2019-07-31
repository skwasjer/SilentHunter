using System.Drawing;
using System.IO;
using skwas.IO;

namespace SilentHunter.Dat.Controllers.Serialization
{
	/// <summary>
	/// 
	/// </summary>
	/// <remarks>Alpha component is not used, but generally stored as 0.</remarks>
	public class ColorValueSerializer : ControllerValueSerializer<Color>
	{
		public override void Serialize(BinaryWriter writer, ControllerSerializationContext serializationContext)
		{
			writer.WriteStruct(Color.FromArgb(0, (Color)serializationContext.Value));
		}

		public override object Deserialize(BinaryReader reader, ControllerDeserializationContext deserializationContext)
		{
			return Color.FromArgb(byte.MaxValue, reader.ReadStruct<Color>());
		}
	}
}
