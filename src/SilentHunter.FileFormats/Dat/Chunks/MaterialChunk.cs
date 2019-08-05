using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using SilentHunter.FileFormats.Extensions;

namespace SilentHunter.FileFormats.Dat.Chunks
{
	/// <summary>
	/// Represents the material chunk.
	/// </summary>
	[DebuggerDisplay("{ToString(),nq}: Opacity = {Opacity}, Diffuse = {Diffuse}, SpecularMode = {SpecularMode}, Specular = {Specular} Attributes = {Attributes}, Texture = {Texture}")]
	public sealed class MaterialChunk : DatChunk
	{
		private long _creationTimeSinceEpoch;
		private string _texture;
		private static readonly DateTime Epoch = new DateTime(1970, 1, 1);

		/// <summary>
		/// Initializes a new instance of the <see cref="MaterialChunk"/> class.
		/// </summary>
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

		/// <summary>
		/// Gets or sets the material attributes.
		/// </summary>
		/// <remarks>
		/// Not all flags are supported/understood. Some flags are not used with auto detected texture references.
		///
		/// OBJ spec regarding material attributes: http://people.scs.fsu.edu/~burkardt/data/mtl/mtl.html
		/// </remarks>
		public MaterialAttributes Attributes { get; set; }

		/// <summary>
		/// Gets or sets the material opacity.
		/// </summary>
		public byte Opacity { get; set; }

		/// <summary>
		/// Gets or sets the file size of a referenced texture, when stored as TGA. Even if the file is DDS, the size indicated matches that of a TGA.
		/// </summary>
		/// <remarks>This property is not available when <see cref="HasTextureReference" /> is <see langword="false" />, nor does it's value matter a whole lot anyway. It's not used by the game and probably a old/legacy attribute of Ubi's own exporter tools.</remarks>
		public int TgaTextureSize { get; set; }

		/// <summary>
		/// Gets whether the material references a texture by filename, or otherwise should autodetect from other meta data (like label chunk).
		/// </summary>
		public bool HasTextureReference => TgaTextureSize != 0;

		/// <summary>
		/// Gets or sets the file date/time of the texture.
		/// </summary>
		/// <remarks>This property is not available when <see cref="HasTextureReference" /> is <see langword="false" />, nor does it's value matter a whole lot anyway. It's not used by the game and probably a old/legacy attribute of Ubi's own exporter tools.</remarks>
		public DateTime CreationTime
		{
			get => Epoch + TimeSpan.FromSeconds(_creationTimeSinceEpoch);
			set => _creationTimeSinceEpoch = (long)value.Subtract(Epoch).TotalSeconds;
		}

		/// <summary>
		/// Gets or sets the diffuse color.
		/// </summary>
		public Color Diffuse { get; set; }

		/// <summary>
		/// Gets or sets the specular mode.
		/// </summary>
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
		/// Gets or sets the glossiness ('shininess') component for specular lighting.
		/// </summary>
		public byte Glossiness { get; set; }

		/// <summary>
		/// Gets or sets self illuminating factor.
		/// </summary>
		public byte Emission { get; set; }

		/// <summary>
		/// Gets or sets the file name of the texture. Must be null, when non-explicit texture is used.
		/// </summary>
		public string Texture
		{
			get => _texture ?? string.Empty;
			set => _texture = value;
		}

		/// <summary>
		/// Gets or sets flag indicating if z-buffer write mode is enabled. This is a shortcut for Attributes property.
		/// </summary>
		public bool ZBufferWriteEnabled
		{
			get => !Attributes.HasFlag(MaterialAttributes.DisableZBufferWrite);
			set => Attributes = SetOrUnset(Attributes, MaterialAttributes.DisableZBufferWrite, !value);
		}

		/// <summary>
		/// Gets or sets flag indicating if counter clockwise culling is enabled. This is a shortcut for Attributes property.
		/// </summary>
		public bool UseCounterClockwiseCulling
		{
			get => !Attributes.HasFlag(MaterialAttributes.CullNone);
			set => Attributes = SetOrUnset(Attributes, MaterialAttributes.CullNone, !value);
		}

		/// <summary>
		/// Gets or sets flag indicating if linear minification filter is enabled. This is a shortcut for Attributes property.
		/// </summary>
		public bool MinFilterLinear
		{
			get => Attributes.HasFlag(MaterialAttributes.MinFilterLinear);
			set => Attributes = SetOrUnset(Attributes, MaterialAttributes.MinFilterLinear, value);
		}

		/// <summary>
		/// Gets or sets flag indicating if linear magnification filter is enabled. This is a shortcut for Attributes property.
		/// </summary>
		public bool MagFilterLinear
		{
			get => Attributes.HasFlag(MaterialAttributes.MagFilterLinear);
			set => Attributes = SetOrUnset(Attributes, MaterialAttributes.MagFilterLinear, value);
		}

		/// <summary>
		/// Gets or sets flag indicating if DXT compression is enabled. This is a shortcut for Attributes property.
		/// </summary>
		public bool DxtCompression
		{
			get => !Attributes.HasFlag(MaterialAttributes.NoDxtCompression);
			set => Attributes = SetOrUnset(Attributes, MaterialAttributes.NoDxtCompression, !value);
		}

		/// <inheritdoc />
		protected override Task DeserializeAsync(Stream stream)
		{
			// Base material is 36 bytes.
			bool isExtendedMaterial = stream.Length >= 36;

			using (var reader = new BinaryReader(stream, FileEncoding.Default, true))
			{
				// Read the id.
				Id = reader.ReadUInt64();

				// ReSharper disable once JoinDeclarationAndInitializer
				Color clr;

				// Diffuse color and opacity.
				clr = reader.ReadStruct<Color>();
				Diffuse = Color.FromArgb(byte.MaxValue, clr);
				Opacity = Diffuse.A; // Opacity is in alpha component

				// Specular color and mode.
				clr = reader.ReadStruct<Color>();
				Specular = Color.FromArgb(byte.MaxValue, clr);
				SpecularMode = (SpecularMode)clr.A; // Mode is in alpha component.

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
				if (HasTextureReference || isExtendedMaterial)
				{
					if (!HasTextureReference)
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

				// Some files contain more data, problem seen mainly in some mods due to hex editing likely. Chunk will be correctly serialized upon next save.
				if (stream.Length > stream.Position)
				{
					stream.Position = stream.Length;
				}
			}

			return Task.CompletedTask;
		}

		/// <inheritdoc />
		protected override Task SerializeAsync(Stream stream)
		{
			using (var writer = new BinaryWriter(stream, FileEncoding.Default, true))
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

				if (HasTextureReference)
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

			return Task.CompletedTask;
		}

		private static MaterialAttributes SetOrUnset(MaterialAttributes flags, MaterialAttributes bit, bool condition)
		{
			return condition
				? flags | bit
				: flags & ~bit;
		}
	}
}