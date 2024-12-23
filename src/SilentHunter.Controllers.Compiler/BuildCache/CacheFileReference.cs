using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SilentHunter.Controllers.Compiler.BuildCache;
// TODO: switch to DataContractSerializer so this does not have to be public.

/// <summary>
/// Represents a cached file reference.
/// </summary>
internal class CacheFileReference : IXmlSerializable, IEquatable<CacheFileReference>
{
    /// <summary>
    /// Gets or sets the file path.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets when the file was last modified.
    /// </summary>
    public DateTime? LastModified { get; set; }

    /// <summary>
    /// Gets or sets the length (size) of the file.
    /// </summary>
    public long? Length { get; set; }

    XmlSchema IXmlSerializable.GetSchema()
    {
        return null;
    }

    void IXmlSerializable.ReadXml(XmlReader reader)
    {
        reader.MoveToAttribute("name");
        Name = reader.Value;

        LastModified = null;
        if (reader.MoveToAttribute("dt"))
        {
            LastModified = XmlConvert.ToDateTime(reader.Value, XmlDateTimeSerializationMode.Utc);
        }

        if (reader.MoveToAttribute("len"))
        {
            if (long.TryParse(reader.Value, out long length))
            {
                Length = length;
            }
        }

        reader.Read();
    }

    void IXmlSerializable.WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString("name", Name);
        if (LastModified.HasValue)
        {
            writer.WriteAttributeString("dt",
                XmlConvert.ToString(LastModified.Value, XmlDateTimeSerializationMode.Utc)
            );
        }

        if (Length.HasValue)
        {
            writer.WriteAttributeString("len",
                Length.Value.ToString()
            );
        }
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return Name;
    }

    /// <summary>
    /// </summary>
    public bool Equals(CacheFileReference other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return string.Equals(Name, other.Name) && LastModified.Equals(other.LastModified) && Length == other.Length;
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        return Equals((CacheFileReference)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = Name != null ? Name.GetHashCode() : 0;
            hashCode = (hashCode * 397) ^ LastModified.GetHashCode();
            hashCode = (hashCode * 397) ^ Length.GetHashCode();
            return hashCode;
        }
    }

    /// <summary>
    /// </summary>
    public static bool operator ==(CacheFileReference left, CacheFileReference right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// </summary>
    public static bool operator !=(CacheFileReference left, CacheFileReference right)
    {
        return !Equals(left, right);
    }
}
