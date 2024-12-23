using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using SilentHunter.FileFormats.Extensions;

namespace SilentHunter.FileFormats.Dat.Chunks;

/// <summary>
/// Represents a texture map chunk.
/// </summary>
[DebuggerDisplay("{ToString(),nq}: {MapType}, texture: {Texture})")]
public sealed class TextureMapChunk : DatChunk
{
    private const int TextureMapNameLength = 8;
    private static readonly DateTime Epoch = new DateTime(1970, 1, 1);

    private long _creationTimeSinceEpoch;
    private TextureMapType _mapType;
    private string _texture;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureMapChunk"/> class.
    /// </summary>
    public TextureMapChunk()
        : base(DatFile.Magics.TextureMap)
    {
        MapType = TextureMapType.AmbientOcclusionMap;
        MapChannel = 2; // Default to 2 because in 99% this is the case.
        Attributes = MaterialAttributes.MagFilterLinear | MaterialAttributes.MinFilterLinear;
        CreationTime = DateTime.UtcNow;
    }

    /// <inheritdoc />
    public override bool SupportsId => true;

    /// <inheritdoc />
    public override bool SupportsParentId => true;

    /// <summary>
    /// Gets or sets the map channel index.
    /// </summary>
    public int MapChannel { get; set; }

    /// <summary>
    /// Gets or sets the map type.
    /// </summary>
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
    public string Texture
    {
        get => _texture ?? string.Empty;
        set => _texture = value;
    }

    /// <inheritdoc />
    protected override Task DeserializeAsync(Stream stream)
    {
        using (var reader = new BinaryReader(stream, FileEncoding.Default, true))
        {
            // Read id and parent id.
            Id = reader.ReadUInt64();
            ParentId = reader.ReadUInt64();

            string name = reader.ReadString(TextureMapNameLength).TrimEnd('\0');

            switch (name)
            {
                case "specular":
                    MapType = TextureMapType.SpecularMap;
                    break;

                case "bump":
                    MapType = TextureMapType.NormalMap;
                    break;

                case "selfillu":
                    MapType = TextureMapType.AmbientOcclusionMap;
                    break;
                default:
                    MapType = TextureMapType.AmbientOcclusionMap;
                    break;
            }

            MapChannel = reader.ReadInt32();

            Attributes = reader.ReadStruct<MaterialAttributes>();

            TgaTextureSize = reader.ReadInt32();
            _creationTimeSinceEpoch = reader.ReadInt64();

            // The rest of the stream holds the texture name + terminating zero.
            if (stream.Length > stream.Position)
            {
                Texture = reader.ReadNullTerminatedString();
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
            // Write id and parent id.
            writer.Write(Id);
            writer.Write(ParentId);

            string mapTypeStr;
            switch (MapType)
            {
                case TextureMapType.SpecularMap:
                    mapTypeStr = "specular";
                    break;
                case TextureMapType.NormalMap:
                    mapTypeStr = "bump";
                    break;
                case TextureMapType.AmbientOcclusionMap:
                    mapTypeStr = "selfillu";
                    break;
                default:
                    throw new InvalidOperationException($"Unexpected map type {MapType}.");
            }

            writer.Write(mapTypeStr.PadRight(TextureMapNameLength, '\0'), false);

            writer.Write(MapChannel);

            writer.WriteStruct(Attributes);

            writer.Write(TgaTextureSize);
            writer.Write(_creationTimeSinceEpoch);

            writer.Write(Texture, '\0');
        }

        return Task.CompletedTask;
    }
}