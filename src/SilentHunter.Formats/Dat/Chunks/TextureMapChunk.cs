using System;
using System.IO;
using skwas.IO;

namespace SilentHunter.Dat.Chunks
{
	public sealed class TextureMapChunk : DatChunk
	{
		private const int TexmapNameLength = 8;

		private long _creationTimeSinceEpoch;
		private static readonly DateTime Epoch = new DateTime(1970, 1, 1);

		private string _originalTexture;
		private TextureMapType _mapType;

		public TextureMapChunk()
			: base(DatFile.Magics.TextureMap)
		{
			_mapType = TextureMapType.AmbientOcclusionMap;
			MapChannel = 2; // Default to 2 because in 99% this is the case.
			Attributes = MaterialAttributes.MagFilterLinear | MaterialAttributes.MinFilterLinear;

			CreationTime = DateTime.Now.ToUniversalTime();
		}

		/// <summary>
		/// Gets whether the chunk supports an id field.
		/// </summary>
		public override bool SupportsId => true;

		/// <summary>
		/// Gets whether the chunk supports a parent id field.
		/// </summary>
		public override bool SupportsParentId => true;

		/// <summary>
		/// Deserializes the chunk.
		/// </summary>
		/// <param name="stream">The stream to read from.</param>
		protected override void Deserialize(Stream stream)
		{
			using (var reader = new BinaryReader(stream, FileEncoding.Default, true))
			{
				// Read id and parent id.
				Id = reader.ReadUInt64();
				ParentId = reader.ReadUInt64();

				string name = reader.ReadString(TexmapNameLength).TrimEnd('\0');

				switch (name)
				{
					case "specular":
						_mapType = TextureMapType.SpecularMap;
						break;

					case "bump":
						_mapType = TextureMapType.NormalMap;
						break;

					case "selfillu":
						_mapType = TextureMapType.AmbientOcclusionMap;
						break;
					default:
						_mapType = TextureMapType.AmbientOcclusionMap;
						break;
				}

				MapChannel = reader.ReadInt32();
#if DEBUG
				/*	if (_mapChannel != 2)
				{
					Console.Write(GetBaseStreamName(stream) + "  " + ParentFile.Chunks.Count + "  ");
					Console.WriteLine(_mapChannel);
				}*/
#endif

				Attributes = (MaterialAttributes)reader.ReadInt32();

				TgaTextureSize = reader.ReadInt32();
				_creationTimeSinceEpoch = reader.ReadInt64();

				// The rest of the stream holds the texture name + terminating zero.
				if (stream.Length > stream.Position)
				{
					Texture = reader.ReadNullTerminatedString();
				}

				_originalTexture = Texture;
			}
		}

		/// <summary>
		/// Serializes the chunk.
		/// </summary>
		/// <param name="stream">The stream to write to.</param>
		protected override void Serialize(Stream stream)
		{
			using (var writer = new BinaryWriter(stream, FileEncoding.Default, true))
			{
				// Write id and parent id.
				writer.Write(Id);
				writer.Write(ParentId);

				string mapTypeStr;
				switch (_mapType)
				{
					case TextureMapType.AmbientOcclusionMap:
						mapTypeStr = "selfillu";
						break;
					case TextureMapType.SpecularMap:
						mapTypeStr = "specular";
						break;
					case TextureMapType.NormalMap:
						mapTypeStr = "bump";
						break;
					default:
						throw new InvalidOperationException($"Unexpected map type {_mapType}.");
				}

				writer.Write(mapTypeStr.PadRight(TexmapNameLength, '\0'), false);

				writer.Write(MapChannel);

				writer.WriteStruct((int)Attributes);

				writer.Write(TgaTextureSize);
				writer.Write(_creationTimeSinceEpoch);

				// Write texture + terminating zero.
				if (!string.IsNullOrEmpty(Texture))
				{
					writer.Write(Texture, '\0');
				}
				else
				{
					writer.Write(byte.MinValue); // Write terminating 0.
				}
			}
		}

		public int MapChannel { get; set; }

		public TextureMapType MapType
		{
			get => _mapType;
			set
			{
				if (!Enum.IsDefined(typeof(TextureMapType), value))
				{
					throw new ArgumentException(@"Specified value is not a valid enumeration value.", nameof(value));
				}

				_mapType = value;
			}
		}

		/// <summary>
		/// Gets or sets the material attributes.
		/// </summary>
		/// <remarks>Not all flags are supported/understood. Some flags are not used with non-explicit texture.</remarks>
		public MaterialAttributes Attributes { get; set; }

		/// <summary>
		/// Gets or sets the file size of an explicit texture, when stored as TGA. Even if the file is DDS, the size indicated matches that of a TGA.
		/// </summary>
		/// <remarks>This property is not available for non-explicit textures, nor does it's value matter a whole lot anyway. It's not used by the game and probably a old/legacy attribute of Ubi's own exporter tools.</remarks>
		public int TgaTextureSize { get; set; }

		/// <summary>
		/// Gets or sets the file date/time of the texture.
		/// </summary>
		/// <remarks>This property is not available for non-explicit textures, nor does it's value matter a whole lot anyway. It's not used by the game and probably a old/legacy attribute of Ubi's own exporter tools.</remarks>
		public DateTime CreationTime
		{
			get => Epoch + TimeSpan.FromSeconds(_creationTimeSinceEpoch);
			set => _creationTimeSinceEpoch = (long)value.Subtract(Epoch).TotalSeconds;
		}

		/// <summary>
		/// Gets or sets the (file)name of the texture.
		/// </summary>
		public string Texture { get; set; }

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>
		/// A string that represents the current object.
		/// </returns>
		public override string ToString()
		{
			return base.ToString() + ": " + MapType + ", texture: " + Texture;
		}
	}
}