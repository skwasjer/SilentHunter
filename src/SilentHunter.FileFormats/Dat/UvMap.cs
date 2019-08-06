using System;
using System.Linq;

namespace SilentHunter.FileFormats.Dat
{
	/// <summary>
	/// Represents an UV map for a channel.
	/// </summary>
	public struct UvMap : IEquatable<UvMap>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UvMap"/> type.
		/// </summary>
		/// <param name="channel">The map channel.</param>
		/// <param name="textureIndices">The texture indices.</param>
		public UvMap(byte channel, ushort[] textureIndices)
		{
			Channel = channel;
			TextureIndices = textureIndices ?? throw new ArgumentNullException(nameof(textureIndices));
		}

		/// <summary>
		/// Gets or sets the map channel.
		/// </summary>
		public byte Channel;

		/// <summary>
		/// Gets or sets the texture indices.
		/// </summary>
		public ushort[] TextureIndices;

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
}