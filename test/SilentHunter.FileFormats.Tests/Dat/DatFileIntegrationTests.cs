using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SilentHunter.FileFormats.Dat.Chunks;
using SilentHunter.FileFormats.Fixtures;
using SilentHunter.Testing.Extensions;
using SilentHunter.Testing.FluentAssertions;

namespace SilentHunter.FileFormats.Dat;

[Collection(nameof(CompiledControllers))]
public class DatFileIntegrationTests : IDisposable
{
    private const string S3DSignature = "\0Modified with S3D - Silent 3ditor (version 0.9.9.0). © 2007-2009 skwas";

    private readonly CompiledControllersFixture _compiledControllers;
    private readonly Stream _datFileStream;

    public DatFileIntegrationTests(CompiledControllersFixture compiledControllers)
    {
        _compiledControllers = compiledControllers ?? throw new ArgumentNullException(nameof(compiledControllers));

        _datFileStream = new MemoryStream(GetType().Assembly.GetManifestResourceStream(GetType(), "DatFileTest.dat").ToArray());
        // S3D appends S3DSettings chunk of 12 bytes. Remove it.
        _datFileStream.SetLength(_datFileStream.Length - 12);
    }

    public void Dispose()
    {
        _datFileStream.Dispose();
    }

    [Fact]
    public async Task Given_loaded_datFile_when_checking_all_controllerData_should_not_contain_byte_arrays()
    {
        DatFile datFile = _compiledControllers.ServiceProvider.GetRequiredService<DatFile>();

        // Act
        await datFile.LoadAsync(_datFileStream);

        // Assert
        datFile.Chunks
            .Should()
            .NotBeEmpty()
            .And.Subject.OfType<ControllerDataChunk>()
            .Select(cd => cd.ControllerData)
            .Should()
            .NotBeEmpty()
            .And
            .NotBeOfType<byte[]>("all controllers should deserialize properly");
    }

    [Fact]
    public async Task Given_datFile_when_reloading_saved_file_should_be_equivalent()
    {
        DatFile sourceDatFile = _compiledControllers.ServiceProvider.GetRequiredService<DatFile>();
        DatFile targetDatFile = _compiledControllers.ServiceProvider.GetRequiredService<DatFile>();

        await sourceDatFile.LoadAsync(_datFileStream);

        using (var ms = new MemoryStream())
        {
            await sourceDatFile.SaveAsync(ms);
            ms.Position = 0;

            // Act
            await targetDatFile.LoadAsync(ms);
        }

        // Assert
        targetDatFile.Chunks.Should()
            .BeEquivalentTo(sourceDatFile.Chunks,
                options => options
                    .RespectingRuntimeTypes()
                    .IncludingProperties()
                    .IncludingFields()
                    .Excluding(c => c.ParentFile)
                    .Excluding(c => c.UnknownData)
            );
    }

    [Fact]
    public async Task Given_loaded_datFile_when_saving_should_be_binary_equivalent()
    {
        DatFile sourceDatFile = _compiledControllers.ServiceProvider.GetRequiredService<DatFile>();

        await sourceDatFile.LoadAsync(_datFileStream);

        using var ms = new MemoryStream();
        // Act
        // In order to make binary comparison, inject S3D signature. Our test file was made with S3D, thus we need this in there.
        sourceDatFile.Chunks
            .OfType<AuthorInfoChunk>()
            .First()
            .Description += S3DSignature;
        await sourceDatFile.SaveAsync(ms);

        // Assert
        _datFileStream.Position = 0;
        byte[] sourceBytes = _datFileStream.ToArray();
        byte[] savedBytes = ms.ToArray();

        savedBytes.Should().BeEquivalentTo(sourceBytes);
    }
}
