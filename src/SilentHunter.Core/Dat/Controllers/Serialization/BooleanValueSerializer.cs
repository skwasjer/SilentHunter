using System.IO;

namespace SilentHunter.Dat.Controllers.Serialization
{
	/// <summary>
	/// 
	/// </summary>
	/// <remarks>
	/// Some files have int32 stored as boolean. This seems like a bug and we should resave the file properly.
	/// </remarks>
	public class BooleanValueSerializer : ControllerValueSerializer<bool>
	{
		public override void Serialize(BinaryWriter writer, ControllerSerializationContext context)
		{
			writer.Write((bool)context.Value);
		}

		public override object Deserialize(BinaryReader reader, ControllerDeserializationContext context)
		{
			long boolLen = reader.BaseStream.Length;
			switch (boolLen)
			{
				case 1:
					return reader.ReadByte() > 0;
				case 4:
					return reader.ReadInt32() > 0;
				default:
					return null;
			}
		}
	}
}