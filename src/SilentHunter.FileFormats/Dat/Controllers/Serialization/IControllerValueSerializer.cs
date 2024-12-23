using System.IO;

namespace SilentHunter.FileFormats.Dat.Controllers.Serialization;

/// <summary>
/// Describes a (de)serializer for controller fields and values.
/// </summary>
public interface IControllerValueSerializer
{
    /// <summary>
    /// Determines if the current field is supported by this serializer.
    /// </summary>
    /// <param name="context">The context describing the field name and type.</param>
    /// <returns></returns>
    public bool IsSupported(ControllerSerializationContext context);

    /// <summary>
    /// Deserializes a field and value from the <paramref name="reader" /> using the specified <paramref name="serializationContext" />.
    /// </summary>
    /// <param name="reader">The reader to read from.</param>
    /// <param name="serializationContext">The context describing the type and name of the field to read.</param>
    /// <returns>The value that was read.</returns>
    public object Deserialize(BinaryReader reader, ControllerSerializationContext serializationContext);

    /// <summary>
    /// Serializes a field and value to the <paramref name="writer" /> using the specified <paramref name="serializationContext" />.
    /// </summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="serializationContext">The context describing the type and name of the field to read.</param>
    /// <param name="value">The value to serialize.</param>
    public void Serialize(BinaryWriter writer, ControllerSerializationContext serializationContext, object value);
}
