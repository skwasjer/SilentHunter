using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using SilentHunter.FileFormats.Extensions;
using SilentHunter.FileFormats.IO;

namespace SilentHunter.FileFormats.Sdl;

/// <summary>
/// Represents a sound info entry for <see cref="SdlFile" />s.
/// </summary>
[DebuggerDisplay("Name = {Name}, WaveName = {WaveName}")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public sealed class SoundInfo : IRawSerializable, ICloneable, IEquatable<SoundInfo>
{
    private static readonly IReadOnlyCollection<PropertyInfo> PropertyInfoCache = typeof(SoundInfo)
        .GetProperties(BindingFlags.Instance | BindingFlags.Public);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string _name, _waveName;

    /// <summary>
    /// Initializes a new instance of the <see cref="SoundInfo" /> class.
    /// </summary>
    public SoundInfo()
    {
        Volume = 90;
        Pitch = 1;
    }

    /// <summary>
    /// Gets or sets the entry name.
    /// </summary>
    public string Name
    {
        get => _name ?? string.Empty;
        set => _name = value;
    }

    /// <summary>
    /// Gets or sets the name of the wave file.
    /// </summary>
    public string WaveName
    {
        get => _waveName ?? string.Empty;
        set => _waveName = value;
    }

    /// <summary>
    /// Gets or sets whether this represents a folder instead of a sound.
    /// </summary>
    public bool IsFolder { get; set; }

    /// <summary>
    /// Gets or sets whether the sound should be looped.
    /// </summary>
    public bool Loop { get; set; }

    /// <summary>
    /// Gets or sets whether the sound auto plays.
    /// </summary>
    public bool Play { get; set; }

    /// <summary>
    /// Gets or sets the volume.
    /// </summary>
    public float Volume { get; set; }

    /// <summary>
    /// Gets or sets the volume variation.
    /// </summary>
    public float VolumeVar { get; set; }

    /// <summary>
    /// Gets or sets the pitch.
    /// </summary>
    public float Pitch { get; set; }

    /// <summary>
    /// Gets or sets the pitch variation.
    /// </summary>
    public float PitchVar { get; set; }

    /// <summary>
    /// Gets or sets whether the sound is played as 3D.
    /// </summary>
    public bool Is3D { get; set; }

    /// <summary>
    /// Gets or sets the doppler factor.
    /// </summary>
    public float DopplerFactor { get; set; }

    /// <summary>
    /// Gets or sets the min radius (3D).
    /// </summary>
    public float MinRadius { get; set; }

    /// <summary>
    /// Gets or sets the max radius (3D).
    /// </summary>
    public float MaxRadius { get; set; }

    /// <summary>
    /// Gets or set the category the sound effect belongs to.
    /// </summary>
    public SoundCategory Category { get; set; }

    /// <summary>
    /// Gets or sets the priority (in case too many sound effects are playing).
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// Gets or sets the playback delay.
    /// </summary>
    public float Delay { get; set; }

    Task IRawSerializable.DeserializeAsync(Stream stream)
    {
        using (var reader = new BinaryReader(stream, FileEncoding.Default, true))
        {
            ushort size = reader.ReadUInt16();
            uint subSize = reader.ReadUInt32();
            if (size != subSize + 4)
            {
                throw new SilentHunterParserException("The file appears invalid. Unexpected size specifier encountered.");
            }

            long currentPos = reader.BaseStream.Position;

            string soundInfoHeader = reader.ReadNullTerminatedString();
            if (soundInfoHeader != nameof(SoundInfo))
            {
                throw new SilentHunterParserException("The file appears invalid. Unexpected item header encountered.");
            }

            foreach (PropertyInfo propertyInfo in PropertyInfoCache)
            {
                propertyInfo.SetValue(this, DeserializeProperty(reader, propertyInfo.Name, propertyInfo.PropertyType));
            }

            if (reader.BaseStream.Position != currentPos + subSize)
            {
                throw new SilentHunterParserException("The file appears invalid. Unexpected item length.");
            }
        }

        return Task.CompletedTask;
    }

    Task IRawSerializable.SerializeAsync(Stream stream)
    {
        using (var writer = new BinaryWriter(stream, FileEncoding.Default, true))
        {
            long currentPos = writer.BaseStream.Position;
            writer.BaseStream.Position += 6;

            writer.Write(nameof(SoundInfo), '\0');

            foreach (PropertyInfo propertyInfo in PropertyInfoCache)
            {
                SerializeProperty(writer, propertyInfo.Name, propertyInfo.GetValue(this));
            }

            long endPos = writer.BaseStream.Position;
            writer.BaseStream.Position = currentPos;
            writer.Write((ushort)(endPos - currentPos - 2));
            writer.Write((uint)(endPos - currentPos - 6));
            writer.BaseStream.Position = endPos;
        }

        return Task.CompletedTask;
    }

    private static object DeserializeProperty(BinaryReader reader, string propertyName, Type propertyType)
    {
        uint size = reader.ReadUInt32();
        long currentPos = reader.BaseStream.Position;
        string name = reader.ReadString(propertyName.Length);
        if (propertyName != name)
        {
            throw new SilentHunterParserException($"The file appears invalid. Expected property '{propertyName}' but encountered '{name}'.");
        }

        // Skip past the terminating null.
        reader.ReadByte();

        // Read value.
        object value = propertyType == typeof(string)
            ? reader.ReadNullTerminatedString()
            : reader.ReadStruct(propertyType);

        if (reader.BaseStream.Position != currentPos + size)
        {
            throw new SilentHunterParserException("The file appears invalid. Unexpected property length.");
        }

        return value;
    }

    /// <summary>
    /// Serializes a property using specified <paramref name="writer" />.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="propertyName">The name of the property.</param>
    /// <param name="value">The property value.</param>
    private static void SerializeProperty(BinaryWriter writer, string propertyName, object value)
    {
        long currentPos = writer.BaseStream.Position;

        // Write size descriptor. We don't know final size yet.
        writer.Write(0);

        // Write name.
        writer.Write(propertyName, '\0');

        // Write value.
        if (value is string str)
        {
            writer.Write(str, '\0');
        }
        else
        {
            writer.WriteStruct(value);
        }

        // Overwrite size descriptor with total size.
        long endPos = writer.BaseStream.Position;
        writer.BaseStream.Position = currentPos;
        writer.Write((int)(endPos - currentPos - 4));
        writer.BaseStream.Position = endPos;
    }

    /// <inheritdoc />
    public object Clone()
    {
        return new SoundInfo
        {
            Name = Name,
            WaveName = WaveName,
            IsFolder = IsFolder,
            Loop = Loop,
            Play = Play,
            Volume = Volume,
            VolumeVar = VolumeVar,
            Pitch = Pitch,
            PitchVar = PitchVar,
            Is3D = Is3D,
            DopplerFactor = DopplerFactor,
            MinRadius = MinRadius,
            MaxRadius = MaxRadius,
            Category = Category,
            Priority = Priority,
            Delay = Delay
        };
    }

    /// <summary>
    /// </summary>
    public bool Equals(SoundInfo other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return string.Equals(_name, other._name) && string.Equals(_waveName, other._waveName) && Is3D == other.Is3D && Play == other.Play && Loop == other.Loop && IsFolder == other.IsFolder && Delay.Equals(other.Delay) && MaxRadius.Equals(other.MaxRadius) && MinRadius.Equals(other.MinRadius) && DopplerFactor.Equals(other.DopplerFactor) && PitchVar.Equals(other.PitchVar) && Pitch.Equals(other.Pitch) && VolumeVar.Equals(other.VolumeVar) && Volume.Equals(other.Volume) && Priority == other.Priority && Category == other.Category;
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return ReferenceEquals(this, obj) || (obj is SoundInfo other && Equals(other));
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            // ReSharper disable NonReadonlyMemberInGetHashCode
            int hashCode = _name != null ? _name.GetHashCode() : 0;
            hashCode = (hashCode * 397) ^ (_waveName != null ? _waveName.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ Is3D.GetHashCode();
            hashCode = (hashCode * 397) ^ Play.GetHashCode();
            hashCode = (hashCode * 397) ^ Loop.GetHashCode();
            hashCode = (hashCode * 397) ^ IsFolder.GetHashCode();
            hashCode = (hashCode * 397) ^ Delay.GetHashCode();
            hashCode = (hashCode * 397) ^ MaxRadius.GetHashCode();
            hashCode = (hashCode * 397) ^ MinRadius.GetHashCode();
            hashCode = (hashCode * 397) ^ DopplerFactor.GetHashCode();
            hashCode = (hashCode * 397) ^ PitchVar.GetHashCode();
            hashCode = (hashCode * 397) ^ Pitch.GetHashCode();
            hashCode = (hashCode * 397) ^ VolumeVar.GetHashCode();
            hashCode = (hashCode * 397) ^ Volume.GetHashCode();
            hashCode = (hashCode * 397) ^ Priority;
            hashCode = (hashCode * 397) ^ (int)Category;
            // ReSharper restore NonReadonlyMemberInGetHashCode
            return hashCode;
        }
    }

    /// <summary>
    /// </summary>
    public static bool operator ==(SoundInfo left, SoundInfo right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// </summary>
    public static bool operator !=(SoundInfo left, SoundInfo right)
    {
        return !Equals(left, right);
    }
}
