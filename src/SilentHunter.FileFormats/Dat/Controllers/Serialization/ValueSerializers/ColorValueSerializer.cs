using System.IO;
using SilentHunter.FileFormats.Extensions;

namespace SilentHunter.FileFormats.Dat.Controllers.Serialization;

/// <summary>
/// Serializes and deserializes color values.
/// </summary>
/// <remarks>Alpha component is not used, but generally stored as 0.</remarks>
public class ColorValueSerializer : ControllerValueSerializer<Color>
{
    /// <inheritdoc />
    public override void Serialize(BinaryWriter writer, ControllerSerializationContext serializationContext, Color value)
    {
        writer.WriteStruct(Color.FromArgb(0, value));
    }

    /// <inheritdoc />
    public override Color Deserialize(BinaryReader reader, ControllerSerializationContext serializationContext)
    {
        return Color.FromArgb(byte.MaxValue, reader.ReadStruct<Color>());
    }
}