using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SilentHunter.Controllers;
using SilentHunter.FileFormats.Extensions;

namespace SilentHunter.FileFormats.Dat.Controllers.Serialization;

/// <summary>
/// Represents controller data serializer, which implements basic (de)serialization rules where each property is stored anonymously in a sequential order, much like C-structs in memory.
/// </summary>
/// <remarks>
/// Raw controllers are typically stored where each property is serialized in binary form, in a sequential order.
/// </remarks>
public class ControllerSerializer : IControllerSerializer, IControllerFieldSerializer
{
    private readonly ICollection<IControllerValueSerializer> _serializers;

    /// <summary>
    /// Initializes a new instance of the <see cref="ControllerSerializer"/>.
    /// </summary>
    public ControllerSerializer()
    {
        _serializers = new List<IControllerValueSerializer>
        {
            new FixedStringValueSerializer(),
            new StringValueSerializer(),
            new ColorValueSerializer(),
            new DateTimeValueSerializer(),
            new BooleanValueSerializer(),
            new NullableValueSerializer(),
            new PrimitiveArrayValueSerializer(),
            new ListValueSerializer(this),
            new SHUnionValueSerializer(),
            new ValueTypeSerializer()	// Should be last.
        };
    }

    /// <summary>
    /// Deserializes specified <paramref name="controller"/> from the <paramref name="stream"/>.
    /// </summary>
    /// <param name="stream">The stream to deserialize from.</param>
    /// <param name="controller">The controller instance to populate with deserialized data.</param>
    public void Deserialize(Stream stream, Controller controller)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        if (controller == null)
        {
            throw new ArgumentNullException(nameof(controller));
        }

        Type controllerType = controller.GetType();
        using (var reader = new BinaryReader(stream, FileEncoding.Default, true))
        {
            var ctx = new ControllerSerializationContext(controllerType, controller);
            Deserialize(reader, ctx, controller);

            reader.BaseStream.EnsureStreamPosition(reader.BaseStream.Length, controllerType.Name);
        }
    }

    /// <summary>
    /// Deserializes specified <paramref name="instance"/> using the <paramref name="reader"/>.
    /// </summary>
    /// <param name="reader">The reader to deserialize from.</param>
    /// <param name="serializationContext">The serialization context.</param>
    /// <param name="instance">The instance to populate with deserialized data.</param>
    protected virtual void Deserialize(BinaryReader reader, ControllerSerializationContext serializationContext, object instance)
    {
        DeserializeFields(reader, serializationContext, instance);
    }

    private void DeserializeFields(BinaryReader reader, ControllerSerializationContext serializationContext, object instance)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        if (serializationContext == null)
        {
            throw new ArgumentNullException(nameof(serializationContext));
        }

        if (instance == null)
        {
            throw new ArgumentNullException(nameof(instance));
        }

        FieldInfo[] fields = serializationContext.Type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (FieldInfo field in fields)
        {
            var ctx = new ControllerSerializationContext(field, serializationContext.Controller);
            var value = DeserializeField(reader, ctx);
            // We have our value, set it.
            field.SetValue(instance, value);
        }
    }

    /// <summary>
    /// Deserializes a field from the <paramref name="reader"/>.
    /// </summary>
    /// <param name="reader">The reader to deserialize from.</param>
    /// <param name="serializationContext">The serialization context.</param>
    protected virtual object DeserializeField(BinaryReader reader, ControllerSerializationContext serializationContext)
    {
        return ReadField(reader, serializationContext);
    }

    object IControllerFieldSerializer.ReadField(BinaryReader reader, ControllerSerializationContext serializationContext)
    {
        return ReadField(reader, serializationContext);
    }

    /// <summary>
    /// Reads a field from the <paramref name="reader"/>.
    /// </summary>
    /// <param name="reader">The reader to deserialize from.</param>
    /// <param name="serializationContext">The serialization context.</param>
    protected virtual object ReadField(BinaryReader reader, ControllerSerializationContext serializationContext)
    {
        object retVal;

        if (serializationContext.Type.IsControllerOrObject())
        {
            // Create a new instance of this type.
            retVal = Activator.CreateInstance(serializationContext.Type);

            DeserializeFields(reader, serializationContext, retVal);
        }
        else
        {
            retVal = ReadValue(reader, serializationContext);
        }

        return retVal;
    }

    /// <summary>
    /// Serializes specified <paramref name="controller"/> to the <paramref name="stream"/>.
    /// </summary>
    /// <param name="stream">The stream to serialize to.</param>
    /// <param name="controller">The controller instance to serialize.</param>
    public void Serialize(Stream stream, Controller controller)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        if (controller == null)
        {
            throw new ArgumentNullException(nameof(controller));
        }

        Type controllerType = controller.GetType();
        using (var writer = new BinaryWriter(stream, FileEncoding.Default, true))
        {
            var ctx = new ControllerSerializationContext(controllerType, controller);
            Serialize(writer, ctx, controller);
        }
    }

    /// <summary>
    /// Serializes specified <paramref name="instance"/> using the <paramref name="writer"/>.
    /// </summary>
    /// <param name="writer">The writer to serialize to.</param>
    /// <param name="serializationContext">The serialization context.</param>
    /// <param name="instance">The instance to serialize.</param>
    protected virtual void Serialize(BinaryWriter writer, ControllerSerializationContext serializationContext, object instance)
    {
        SerializeFields(writer, serializationContext, instance);
    }

    private void SerializeFields(BinaryWriter writer, ControllerSerializationContext serializationContext, object instance)
    {
        if (writer == null)
        {
            throw new ArgumentNullException(nameof(writer));
        }

        if (instance == null)
        {
            throw new ArgumentNullException(nameof(instance));
        }

        FieldInfo[] fields = serializationContext.Type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (FieldInfo field in fields)
        {
            var ctx = new ControllerSerializationContext(field, serializationContext.Controller);
            object value = field.GetValue(instance);
            SerializeField(writer, ctx, value);
        }
    }

    /// <summary>
    /// Serializes specified <paramref name="value"/> using the <paramref name="writer"/>.
    /// </summary>
    /// <param name="writer">The writer to serialize to.</param>
    /// <param name="serializationContext">The serialization context.</param>
    /// <param name="value">The value to serialize.</param>
    protected virtual void SerializeField(BinaryWriter writer, ControllerSerializationContext serializationContext, object value)
    {
        WriteField(writer, serializationContext, value);
    }

    void IControllerFieldSerializer.WriteField(BinaryWriter writer, ControllerSerializationContext serializationContext, object value)
    {
        WriteField(writer, serializationContext, value);
    }

    /// <summary>
    /// Writes specified <paramref name="value"/> using the <paramref name="writer"/>.
    /// </summary>
    /// <param name="writer">The writer to serialize to.</param>
    /// <param name="serializationContext">The serialization context.</param>
    /// <param name="value">The value to serialize.</param>
    protected virtual void WriteField(BinaryWriter writer, ControllerSerializationContext serializationContext, object value)
    {
        if (serializationContext.Type.IsControllerOrObject())
        {
            SerializeFields(writer, serializationContext, value);
        }
        else
        {
            WriteValue(writer, serializationContext, value);
        }
    }

    private object ReadValue(BinaryReader reader, ControllerSerializationContext ctx)
    {
        IControllerValueSerializer serializer = _serializers.FirstOrDefault(s => s.IsSupported(ctx));
        object result = serializer?.Deserialize(reader, ctx);
        if (result == null)
        {
            throw new NotImplementedException($"The specified type '{ctx.Type.FullName}' is not supported or implemented.");
        }

        return result;
    }

    private void WriteValue(BinaryWriter writer, ControllerSerializationContext ctx, object value)
    {
        IControllerValueSerializer serializer = _serializers.FirstOrDefault(s => s.IsSupported(ctx));
        if (serializer == null)
        {
            throw new NotImplementedException($"The specified type '{ctx.Type.FullName}' is not supported or implemented.");
        }

        serializer.Serialize(writer, ctx, value);
    }
}