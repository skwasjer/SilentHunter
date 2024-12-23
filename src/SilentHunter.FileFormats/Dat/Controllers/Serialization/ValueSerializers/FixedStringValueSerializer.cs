﻿using System;
using System.IO;
using System.Reflection;
using SilentHunter.Controllers.Decoration;
using SilentHunter.FileFormats.Extensions;

namespace SilentHunter.FileFormats.Dat.Controllers.Serialization;

/// <summary>
/// Serializes and deserializes fixed length strings.
/// </summary>
public class FixedStringValueSerializer : ControllerValueSerializer<string>
{
    /// <inheritdoc />
    public override bool IsSupported(ControllerSerializationContext context)
    {
        return base.IsSupported(context) && context.Member.HasAttribute<FixedStringAttribute>();
    }

    /// <inheritdoc />
    public override void Serialize(BinaryWriter writer, ControllerSerializationContext serializationContext, string value)
    {
        int fixedLength = serializationContext.Member.GetCustomAttribute<FixedStringAttribute>().Length;
        string s = (string)value ?? string.Empty;

        if (s.Length > fixedLength)
        {
            throw new InvalidOperationException($"The string '{s}' for property '{serializationContext.Member.Name}' exceeds the fixed length {fixedLength}");
        }

        // Write the fixed string with zeros at the end.
        writer.Write(s, fixedLength);
    }

    /// <inheritdoc />
    public override string Deserialize(BinaryReader reader, ControllerSerializationContext serializationContext)
    {
        int fixedLength = serializationContext.Member.GetCustomAttribute<FixedStringAttribute>().Length;
        if (reader.BaseStream.Length > fixedLength)
        {
            throw new SilentHunterParserException($"The stream contains more data than expected for '{serializationContext.Member.Name}', length {fixedLength}.");
        }

        if (reader.BaseStream.Length < fixedLength)
        {
            throw new SilentHunterParserException($"The stream does not contain enough data for '{serializationContext.Member.Name}', length {fixedLength}.");
        }

        string s = reader.ReadString(fixedLength);
        // Take care of possible '\0' char in middle of string.
        return s.Split(['\0'], 2, StringSplitOptions.None)[0];
    }
}
