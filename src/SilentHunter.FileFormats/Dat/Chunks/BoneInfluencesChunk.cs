using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SilentHunter.FileFormats.Extensions;

namespace SilentHunter.FileFormats.Dat.Chunks;

/// <summary>
/// </summary>
/// <remarks>
/// Probably poorly named.
/// </remarks>
public sealed class BoneInfluencesChunk : DatChunk
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BoneInfluencesChunk" /> class.
    /// </summary>
    public BoneInfluencesChunk()
        : base(DatFile.Magics.BoneInfluences)
    {
        WeightsAndIndices = new List<BoneInfluence>();
    }

    /// <summary>
    /// Gets or sets the chunk id.
    /// </summary>
    public override ulong Id
    {
        get => base.Id;
        set
        {
            if (value > uint.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "The id for this chunk is only 4 bytes in length (UInt32).");
            }

            base.Id = value;
        }
    }

    /// <summary>
    /// Gets or sets the chunk its parent id.
    /// </summary>
    public override ulong ParentId
    {
        get => base.ParentId;
        set
        {
            if (value > uint.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "The parent id for this chunk is only 4 bytes in length (UInt32).");
            }

            base.ParentId = value;
        }
    }

    /// <summary>
    /// Gets whether the chunk supports an id field.
    /// </summary>
    public override bool SupportsId { get => true; }

    /// <summary>
    /// Gets whether the chunk supports a parent id field.
    /// </summary>
    public override bool SupportsParentId { get => true; }

    /// <summary>
    /// </summary>
    public List<BoneInfluence> WeightsAndIndices { get; }

    /// <inheritdoc />
    protected override Task DeserializeAsync(Stream stream)
    {
        using (var reader = new BinaryReader(stream, FileEncoding.Default, true))
        {
            Id = reader.ReadUInt32(); // Read an id as an uint.
            ParentId = reader.ReadUInt32(); // Read an id as an uint.

            WeightsAndIndices.Clear();

            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                WeightsAndIndices.Add(reader.ReadStruct<BoneInfluence>());
            }
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override Task SerializeAsync(Stream stream)
    {
        using (var writer = new BinaryWriter(stream, FileEncoding.Default, true))
        {
            writer.Write((uint)Id);
            writer.Write((uint)ParentId);

            writer.Write(WeightsAndIndices.Count);
            foreach (BoneInfluence wi in WeightsAndIndices)
            {
                writer.WriteStruct(wi);
            }
        }

        return Task.CompletedTask;
    }
}
