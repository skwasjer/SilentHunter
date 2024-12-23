using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using SilentHunter.FileFormats.Extensions;

namespace SilentHunter.FileFormats.Dat.Chunks;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// The type name is somewhat poor choice.
/// </remarks>
public sealed class PlacementChunk : DatChunk
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlacementChunk"/>.
    /// </summary>
    public PlacementChunk()
        : base(DatFile.Magics.Placement)
    {
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
    /// Gets or sets the world offset.
    /// </summary>
    public Vector3 Offset { get; set; }

    /// <summary>
    /// Gets or sets the rotation (or well, I think it is rotation).
    /// </summary>
    public Vector3 Rotation { get; set; }

    /// <summary>
    /// Gets or sets the id of the node that this chunk defines placement for. This is usually an id in an external dat-file/library.
    /// </summary>
    public ulong NodeId { get; set; }

    /// <inheritdoc />
    protected override Task DeserializeAsync(Stream stream)
    {
        using (var reader = new BinaryReader(stream, FileEncoding.Default, true))
        {
            Id = reader.ReadUInt64();
            ParentId = reader.ReadUInt64();

            NodeId = reader.ReadUInt64();

            Offset = reader.ReadStruct<Vector3>();
            Rotation = reader.ReadStruct<Vector3>();
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override Task SerializeAsync(Stream stream)
    {
        using (var writer = new BinaryWriter(stream, FileEncoding.Default, true))
        {
            writer.Write(Id);
            writer.Write(ParentId);

            writer.Write(NodeId);

            writer.WriteStruct(Offset);
            writer.WriteStruct(Rotation);
        }

        return Task.CompletedTask;
    }
}