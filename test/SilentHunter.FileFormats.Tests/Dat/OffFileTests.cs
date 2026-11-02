using SilentHunter.FileFormats.ChunkedFiles;
using SilentHunter.FileFormats.Dat.Chunks;

namespace SilentHunter.FileFormats.Dat;

public class DatFileTests
{
    private readonly IChunkResolver<DatFile.Magics> _chunkResolver = Substitute.For<IChunkResolver<DatFile.Magics>>();
    private readonly IChunkActivator _chunkActivator = Substitute.For<IChunkActivator>();
    private readonly DatFile _sut;

    public DatFileTests()
    {
        _sut = new DatFile(_chunkResolver, _chunkActivator);
    }

    [Fact]
    public void Items_yields_same_collection()
    {
        (_sut as ISilentHunterFile<DatChunk>).Items.Should().BeSameAs(_sut.Chunks);
    }
}
