using System.IO;

namespace SilentHunter.FileFormats.Dat.Controllers.Serialization;

/// <summary>
/// Describes a (de)serializer for controller fields.
/// </summary>
public interface IControllerFieldSerializer
{
    /// <summary>
    /// Reads a field from the <paramref name="reader"/> using the specified <paramref name="serializationContext"/>.
    /// </summary>
    /// <param name="reader">The reader to read from.</param>
    /// <param name="serializationContext">The context describing the type and name of the field to read.</param>
    /// <returns>The value that was read.</returns>
    object ReadField(BinaryReader reader, ControllerSerializationContext serializationContext);

    /// <summary>
    /// Writes a field to the <paramref name="writer"/> using the specified <paramref name="serializationContext"/>.
    /// </summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="serializationContext">The context describing the type and name of the field to read.</param>
    /// <param name="value">The value to write.</param>
    void WriteField(BinaryWriter writer, ControllerSerializationContext serializationContext, object value);
}