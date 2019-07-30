using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using skwas.IO;

// http://www.jalix.org/ressources/graphics/3DS/_unofficials/3ds-info.txt

namespace SilentHunter.Dat.Chunks
{
	public sealed class MaterialChunk : DatChunk
	{
		private long _creationTimeSinceEpoch;
		private static readonly DateTime Epoch = new DateTime(1970, 1, 1);

		public MaterialChunk()
			: base(DatFile.Magics.Material)
		{
			Attributes = MaterialAttributes.MagFilterLinear | MaterialAttributes.MinFilterLinear;
			CreationTime = DateTime.Now.ToUniversalTime();
			Diffuse = Color.FromArgb(149, 149, 149);
			Specular = Color.FromArgb(229, 229, 229);
			Opacity = 255;
		}

		/// <summary>
		/// Gets whether the chunk supports an id field.
		/// </summary>
		public override bool SupportsId => true;

		// http://people.scs.fsu.edu/~burkardt/data/mtl/mtl.html

		/// <summary>
		/// Gets or sets the material attributes.
		/// </summary>
		/// <remarks>Not all flags are supported/understood. Some flags are not used with non-explicit texture.</remarks>
		public MaterialAttributes Attributes { get; set; }

		/// <summary>
		/// Gets or sets the material opacity.
		/// </summary>
		public byte Opacity { get; set; }

		/// <summary>
		/// Gets or sets the file size of an explicit texture, when stored as TGA. Even if the file is DDS, the size indicated matches that of a TGA.
		/// </summary>
		/// <remarks>This property is not available for non-explicit textures, nor does it's value matter a whole lot anyway. It's not used by the game and probably a old/legacy attribute of Ubi's own exporter tools.</remarks>
		public int TgaTextureSize { get; set; }

		/// <summary>
		/// Gets whether the material is an explicit/internal texture (this is true when texture size &lt;&gt; 0)
		/// </summary>
		public bool IsInternalResource => TgaTextureSize != 0;

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
		/// Gets or sets the diffuse color.
		/// </summary>
		public Color Diffuse { get; set; }

		public SpecularMode SpecularMode { get; set; }

		/// <summary>
		/// Gets or sets the specular color.
		/// </summary>
		public Color Specular { get; set; }

		/// <summary>
		/// Gets or sets the specular intensity.
		/// </summary>
		public byte SpecularStrength { get; set; }

		/// <summary>
		/// Gets or sets the glosiness ('shininess') component for specular lighting.
		/// </summary>
		public byte Glossiness { get; set; }

		/// <summary>
		/// Gets or sets self illuminating factor.
		/// </summary>
		public byte Emission { get; set; }

		/// <summary>
		/// Gets or sets the file name of the texture. Must be null, when non-explicit texture is used.
		/// </summary>
		public string Texture { get; set; }

		/// <summary>
		/// Gets or sets flag indicating if z-buffer write mode is enabled. This is a shortcut for Attributes property.
		/// </summary>
		public bool ZBufferWriteEnabled
		{
			get => !Attributes.HasFlag(MaterialAttributes.DisableZBufferWrite);
			set
			{
				Attributes = Attributes | MaterialAttributes.DisableZBufferWrite;
				if (value)
				{
					Attributes ^= MaterialAttributes.DisableZBufferWrite;
				}
			}
		}

		/// <summary>
		/// Gets or sets flag indicating if counter clockwise culling is enabled. This is a shortcut for Attributes property.
		/// </summary>
		public bool UseCounterClockwiseCulling
		{
			get => !Attributes.HasFlag(MaterialAttributes.CullNone);
			set
			{
				Attributes = Attributes | MaterialAttributes.CullNone;
				if (value)
				{
					Attributes ^= MaterialAttributes.CullNone;
				}
			}
		}

		/// <summary>
		/// Gets or sets flag indicating if linear minification filter is enabled. This is a shortcut for Attributes property.
		/// </summary>
		public bool MinFilterLinear
		{
			get => Attributes.HasFlag(MaterialAttributes.MinFilterLinear);
			set
			{
				Attributes = Attributes | MaterialAttributes.MinFilterLinear;
				if (!value)
				{
					Attributes ^= MaterialAttributes.MinFilterLinear;
				}
			}
		}

		/// <summary>
		/// Gets or sets flag indicating if linear magnification filter is enabled. This is a shortcut for Attributes property.
		/// </summary>
		public bool MagFilterLinear
		{
			get => Attributes.HasFlag(MaterialAttributes.MagFilterLinear);
			set
			{
				Attributes = Attributes | MaterialAttributes.MagFilterLinear;
				if (!value)
				{
					Attributes ^= MaterialAttributes.MagFilterLinear;
				}
			}
		}

		/// <summary>
		/// Gets or sets flag indicating if DXT compression is enabled. This is a shortcut for Attributes property.
		/// </summary>
		public bool DxtCompression
		{
			get => !Attributes.HasFlag(MaterialAttributes.NoDxtCompression);
			set
			{
				Attributes = Attributes | MaterialAttributes.NoDxtCompression;
				if (value)
				{
					Attributes ^= MaterialAttributes.NoDxtCompression;
				}
			}
		}

		/// <summary>
		/// Deserializes the chunk.
		/// </summary>
		/// <param name="stream">The stream to read from.</param>
		protected override void Deserialize(Stream stream)
		{
			bool isExtendedMaterial = stream.Length >= 36;

			using (var reader = new BinaryReader(stream, Encoding.ParseEncoding, true))
			{
				// Read the id.
				Id = reader.ReadUInt64();

				// Material opacity.
				Opacity = reader.ReadByte();

				// Colors are flipped. So, read them inverted.
				byte[] color;

				color = reader.ReadBytes(3);
				Diffuse = Color.FromArgb(color[2], color[1], color[0]);

				SpecularMode = (SpecularMode)reader.ReadByte();

				// Again a 'flipped' color.
				color = reader.ReadBytes(3);
				Specular = Color.FromArgb(color[2], color[1], color[0]);

				// Other material properties.
				SpecularStrength = reader.ReadByte();
				Glossiness = reader.ReadByte();
				Emission = reader.ReadByte();

				byte always0 = reader.ReadByte();
				Debug.Assert(always0 == byte.MinValue, "Material always0, excepted 0.");

				// External texture resource.
				Attributes = (MaterialAttributes)reader.ReadInt32();

				// A texture size indicates if the referenced texture is internal. If 0, then it's an external texture.
				TgaTextureSize = reader.ReadInt32();

				// For support of incorrect files, we still look for more bytes, even if the resource is external.
				if (IsInternalResource || isExtendedMaterial)
				{
					if (!IsInternalResource)
					{
						TgaTextureSize = -1; // If extended material (stream len >= 36).
					}

					if (isExtendedMaterial)
					{
						_creationTimeSinceEpoch = reader.ReadInt64();
						// The rest of the stream holds the name + terminating zero.
						if (stream.Length > stream.Position)
						{
							Texture = reader.ReadNullTerminatedString();
						}
					}
					else
					{
						// Fix SH3 bug on modded files, where remaining stream size is invalid, probably due to hex editing.
						TgaTextureSize = 0;
					}
				}
			}
		}

		/// <summary>
		/// Serializes the chunk.
		/// </summary>
		/// <param name="stream">The stream to write to.</param>
		protected override void Serialize(Stream stream)
		{
			using (var writer = new BinaryWriter(stream, Encoding.ParseEncoding, true))
			{
				writer.Write(Id);

				writer.Write(Opacity);
				writer.Write(Diffuse.B);
				writer.Write(Diffuse.G);
				writer.Write(Diffuse.R);

				writer.Write((byte)SpecularMode);
				writer.Write(Specular.B);
				writer.Write(Specular.G);
				writer.Write(Specular.R);

				writer.Write(SpecularStrength);
				writer.Write(Glossiness);
				writer.Write(Emission);

				writer.Write(byte.MinValue);
				writer.Write((int)Attributes);

				if (IsInternalResource)
				{
/*				if (_tgaTextureSize == -1)
				{
					// Get size of EmbeddedImage.
					if (ParentFile != null)
					{
						int myIndex = ParentFile.Chunks.IndexOf(this);
						if (myIndex <= ParentFile.Chunks.Count)
						{
							EmbeddedImage ei = ParentFile.Chunks[myIndex + 1] as EmbeddedImage;
							if (ei != null)
								_tgaTextureSize = ei.Buffer.Length;
						}
					}
					if (_tgaTextureSize == -1)
						_tgaTextureSize = 0;
				}*/
					writer.Write(TgaTextureSize);
					writer.Write(_creationTimeSinceEpoch);
					if (!string.IsNullOrEmpty(Texture))
					{
						writer.Write(Texture, '\0');
					}
				}
				else
				{
					writer.Write(0); // Texture size is 0.
				}
			}
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>
		/// A string that represents the current object.
		/// </returns>
		public override string ToString()
		{
			return base.ToString() + (Texture != null ? ": " + Texture : null);
		}
	}
}