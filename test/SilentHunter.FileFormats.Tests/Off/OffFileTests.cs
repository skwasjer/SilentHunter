namespace SilentHunter.FileFormats.Off;

public class OffFileTests
{
    private readonly OffFile _sut = [];

    [Fact]
    public void Items_yields_same_collection()
    {
        (_sut as ISilentHunterFile<OffCharacter>).Items.Should().BeSameAs(_sut);
    }
}
