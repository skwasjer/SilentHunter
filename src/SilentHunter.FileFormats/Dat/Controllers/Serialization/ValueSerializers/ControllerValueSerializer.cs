using System.IO;

namespace SilentHunter.FileFormats.Dat.Controllers.Serialization;

/// <summary>
/// Typed base class for serializing and deserializing values.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class ControllerValueSerializer<T> : IControllerValueSerializer
{
    /// <inheritdoc />
    public virtual bool IsSupported(ControllerSerializationContext context)
    {
        return context.Type == typeof(T);
    }

    void IControllerValueSerializer.Serialize(BinaryWriter writer, ControllerSerializationContext serializationContext, object value)
    {
        Serialize(writer, serializationContext, (T)value);
    }

    /// <summary>
    /// Serializes a field and value to the <paramref name="writer"/> using the specified <paramref name="serializationContext"/>.
    /// </summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="serializationContext">The context describing the type and name of the field to read.</param>
    /// <param name="value">The value to serialize.</param>
    public abstract void Serialize(BinaryWriter writer, ControllerSerializationContext serializationContext, T value);

    object IControllerValueSerializer.Deserialize(BinaryReader reader, ControllerSerializationContext serializationContext)
    {
        return Deserialize(reader, serializationContext);
    }

    /// <summary>
    /// Deserializes a field and value from the <paramref name="reader"/> using the specified <paramref name="serializationContext"/>.
    /// </summary>
    /// <param name="reader">The reader to read from.</param>
    /// <param name="serializationContext">The context describing the type and name of the field to read.</param>
    /// <returns>The value that was read.</returns>
    public abstract T Deserialize(BinaryReader reader, ControllerSerializationContext serializationContext);
}