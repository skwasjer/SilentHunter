using System;
using System.Linq;

namespace SilentHunter.FileFormats.Dat
{
	public struct UvMap : IEquatable<UvMap>
	{
		public UvMap(byte channel, ushort[] textureIndices)
		{
			Channel = channel;
			TextureIndices = textureIndices ?? throw new ArgumentNullException(nameof(textureIndices));
		}

		public byte Channel;
		public ushort[] TextureIndices;

		public bool Equals(UvMap other)
		{
			return Channel == other.Channel && (TextureIndices == null && other.TextureIndices == null || TextureIndices != null && other.TextureIndices != null && TextureIndices.SequenceEqual(other.TextureIndices));
		}

		public override bool Equals(object obj)
		{
			return obj is UvMap other && Equals(other);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Channel.GetHashCode() * 397) ^ (TextureIndices != null ? TextureIndices.GetHashCode() : 0);
			}
		}
	}
}