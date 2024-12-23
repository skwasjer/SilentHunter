using System;
using System.Collections.Generic;
using System.Linq;

namespace SilentHunter.FileFormats.Dat;

/// <summary>
/// Represents an UV map for a channel.
/// </summary>
public struct UvMap : IEquatable<UvMap>
{
    private byte _channel;

    /// <summary>
    /// Initializes a new instance of the <see cref="UvMap"/> type.
    /// </summary>
    /// <param name="channel">The map channel.</param>
    /// <param name="textureIndices">The texture indices.</param>
    public UvMap(byte channel, ushort[] textureIndices)
    {
        if (channel <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(channel));
        }

        _channel = channel;
        TextureIndices = textureIndices ?? throw new ArgumentNullException(nameof(textureIndices));
    }

    /// <summary>
    /// Gets or sets the map channel (1 or higher).
    /// </summary>
    public byte Channel
    {
        get => _channel;
        set
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Map channels must be 1 or higher.");
            }
            _channel = value;
        }
    }

    /// <summary>
    /// Gets or sets the texture indices.
    /// </summary>
    public IList<ushort> TextureIndices { get; set; }

    /// <summary>
    /// </summary>
    public bool Equals(UvMap other)
    {
        return Channel == other.Channel && (TextureIndices == null && other.TextureIndices == null || TextureIndices != null && other.TextureIndices != null && TextureIndices.SequenceEqual(other.TextureIndices));
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is UvMap other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            return (Channel.GetHashCode() * 397) ^ (TextureIndices != null ? TextureIndices.GetHashCode() : 0);
        }
    }
}