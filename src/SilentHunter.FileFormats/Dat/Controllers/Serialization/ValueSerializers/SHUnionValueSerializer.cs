using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using SilentHunter.FileFormats.Extensions;

namespace SilentHunter.FileFormats.Dat.Controllers.Serialization;

/// <summary>
/// Serializes and deserializes <see cref="SHUnion{TTypeA,TTypeB}"/>.
/// </summary>
public class SHUnionValueSerializer : IControllerValueSerializer
{
    // ReSharper disable once InconsistentNaming
    private class SHUnionPropertyCache
    {
        public PropertyInfo TypeProperty { get; set; }
        public PropertyInfo ValueProperty { get; set; }
    }

    private static readonly IDictionary<Type, SHUnionPropertyCache> PropertyCache = new ConcurrentDictionary<Type, SHUnionPropertyCache>();

    /// <inheritdoc />
    public bool IsSupported(ControllerSerializationContext context)
    {
        return context.Type.IsClosedTypeOf(typeof(SHUnion<,>));
    }

    /// <inheritdoc />
    public void Serialize(BinaryWriter writer, ControllerSerializationContext serializationContext, object value)
    {
        SHUnionPropertyCache propertyCache = GetCachedProperties(serializationContext.Type);
        writer.WriteStruct(propertyCache.ValueProperty.GetValue(value, null));
    }

    /// <inheritdoc />
    public object Deserialize(BinaryReader reader, ControllerSerializationContext serializationContext)
    {
        long dataSize = reader.BaseStream.Length - reader.BaseStream.Position;

        SHUnionPropertyCache propertyCache = GetCachedProperties(serializationContext.Type);

        Type[] typeArgs = serializationContext.Type.GetGenericArguments();
        // Using size of data to determine which of the SHUnion generic type arguments to use.
        Type valueType = typeArgs.FirstOrDefault(type => Marshal.SizeOf(type) == dataSize);
        if (valueType == null)
        {
            throw new SilentHunterParserException($"The available stream data does not match the size one of the two union types for property '{serializationContext.Name}'.");
        }

        object union = Activator.CreateInstance(serializationContext.Type);
        propertyCache.TypeProperty.SetValue(union, valueType, null);
        propertyCache.ValueProperty.SetValue(union, reader.ReadStruct(valueType), null);
        return union;

    }

    private static SHUnionPropertyCache GetCachedProperties(Type type)
    {
        if (PropertyCache.ContainsKey(type))
        {
            return PropertyCache[type];
        }

        var x = new SHUnionPropertyCache
        {
            TypeProperty = type.GetProperty("Type"),
            ValueProperty = type.GetProperty("Value")
        };
        PropertyCache.Add(type, x);
        return x;
    }
}