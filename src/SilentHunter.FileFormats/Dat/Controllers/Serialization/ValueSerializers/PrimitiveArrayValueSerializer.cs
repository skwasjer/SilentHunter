using System;
using System.IO;
using SilentHunter.FileFormats.Extensions;

namespace SilentHunter.FileFormats.Dat.Controllers.Serialization;

/// <summary>
/// </summary>
// FIX: I think we can remove this. Find examples.
public class PrimitiveArrayValueSerializer : IControllerValueSerializer
{
    /// <inheritdoc />
    public bool IsSupported(ControllerSerializationContext context)
    {
        return context.Type.IsArray && (context.Type.GetElementType()?.IsPrimitive ?? false);
    }

    /// <inheritdoc />
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
        for (int i = 0; i < array.Length; i++)
        {
            writer.WriteStruct(array.GetValue(i));
        }
    }

    /// <inheritdoc />
    public object Deserialize(BinaryReader reader, ControllerSerializationContext serializationContext)
    {
        throw new NotSupportedException("Arrays are not supported. Use List<> instead.");
    }
}
