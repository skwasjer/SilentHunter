using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SilentHunter.FileFormats.Dat.Chunks;
using SilentHunter.FileFormats.Fixtures;
using SilentHunter.FileFormats.Graphics;
using SilentHunter.Testing.Extensions;
using Xunit;

namespace SilentHunter.FileFormats.Dat;

[Collection(nameof(CompiledControllers))]
public class FixEmbeddedImageTests : IDisposable
{
    private readonly CompiledControllersFixture _compiledControllers;
    private readonly MemoryStream _datFileStream;

    public FixEmbeddedImageTests(CompiledControllersFixture compiledControllers)
    {
        _compiledControllers = compiledControllers ?? throw new ArgumentNullException(nameof(compiledControllers));

        _datFileStream = new MemoryStream(GetType().Assembly.GetManifestResourceStream(GetType(), "TgaHeaderErrors.dat").ToArray());
    }

    public void Dispose()
    {
        _datFileStream?.Dispose();
    }

    [Fact]
    public async Task Should_fix_tga()
    {
        const int chunkIndexThatIsFaulty = 24;
        var datFile = _compiledControllers.ServiceProvider.GetRequiredService<DatFile>();

        // Act
        await datFile.LoadAsync(_datFileStream);

        // Assert
        EmbeddedImageChunk embeddedImageChunk = datFile.Chunks[chunkIndexThatIsFaulty].Should().BeOfType<EmbeddedImageChunk>().Which;
        using (Stream imageData = await embeddedImageChunk.ReadAsStreamAsync())
        {
            var tgaDetector = new TgaImageFormatDetector();
            tgaDetector.GetImageFormat(imageData).Should().Be("tga", "it will return null if not a valid TGA");
        }
    }
}