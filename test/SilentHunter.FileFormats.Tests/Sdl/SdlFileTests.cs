using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions.Specialized;
using SilentHunter.Testing.Extensions;

namespace SilentHunter.FileFormats.Sdl;

public class SdlFileTests
{
    private readonly SdlFile _sut;

    public SdlFileTests()
    {
        _sut = [];
    }

    [Theory]
    [MemberData(nameof(TestCases))]
    public async Task Given_stream_with_items_when_loading_should_populate_sdl(Stream stream, params SoundInfo[] expectedSoundInfos)
    {
        // Act
        using (stream)
        {
            await _sut.LoadAsync(stream);
        }

        // Assert
        _sut.Should().BeEquivalentTo(expectedSoundInfos.ToList());
    }

    [Theory]
    [MemberData(nameof(TestCases))]
    public async Task Given_saved_items_when_loading_should_match(Stream stream, params SoundInfo[] expectedSoundInfos)
    {
        stream.Dispose(); // Don't need this.
        expectedSoundInfos?.ToList().ForEach(si => _sut.Add(si));

        using (var ms = new MemoryStream())
        {
            await _sut.SaveAsync(ms);
            _sut.Clear();
            ms.Position = 0;

            // Act
            await _sut.LoadAsync(ms);
        }

        // Assert
        _sut.Should().BeEquivalentTo(expectedSoundInfos.ToList());
    }

    [Fact]
    public async Task Given_sdl_file_already_has_items_when_loading_should_replace()
    {
        var existingSoundInfo = new SoundInfo();
        _sut.Add(new SoundInfo());

        // Act
        using (var ms = new MemoryStream())
        {
            await _sut.LoadAsync(ms);
        }

        // Assert
        _sut.Should().NotContain(existingSoundInfo);
    }

    [Fact]
    public async Task Given_null_stream_when_loading_should_throw()
    {
        // Act
        Func<Task> act = () => _sut.LoadAsync(null);

        // Assert
        await act.Should()
            .ThrowAsync<ArgumentNullException>()
            .WithParameterName("stream");
    }

    [Fact]
    public async Task Given_null_stream_when_saving_should_throw()
    {
        // Act
        Func<Task> act = () => _sut.SaveAsync(null);

        // Assert
        await act.Should()
            .ThrowAsync<ArgumentNullException>()
            .WithParameterName("stream");
    }

    [Fact]
    public async Task Given_stream_is_not_readable_when_loading_should_throw()
    {
        var unreadableStreamMock = new Mock<Stream>();
        unreadableStreamMock.Setup(m => m.CanRead).Returns(false);

        // Act
        Func<Task> act = () => _sut.LoadAsync(unreadableStreamMock.Object);

        // Assert
        await act.Should()
            .ThrowAsync<ArgumentException>()
            .WithMessage("The stream does not support reading.*")
            .WithParameterName("stream");
    }

    [Fact]
    public async Task Given_stream_is_not_writable_when_saving_should_throw()
    {
        var unwritableStreamMock = new Mock<Stream>();
        unwritableStreamMock.Setup(m => m.CanWrite).Returns(false);

        // Act
        Func<Task> act = () => _sut.SaveAsync(unwritableStreamMock.Object);

        // Assert
        await act.Should()
            .ThrowAsync<ArgumentException>()
            .WithMessage("The stream does not support writing.*")
            .WithParameterName("stream");
    }

    [Theory]
    [MemberData(nameof(TestCasesWithInvalidData))]
    public async Task Given_invalid_data_when_reading_should_throw(SoundInfoTestCase testCase)
    {
        // Act
        Func<Task> act = () =>
        {
            using var ms = new MemoryStream(testCase.Data);
            return _sut.LoadAsync(ms);
        };

        // Assert
        ExceptionAssertions<SdlFileException> ex = await act.Should().ThrowAsync<SdlFileException>();
        ex.WithInnerExceptionExactly<SilentHunterParserException>()
            .WithMessage(testCase.ExpectedError);
        ex.Which.ItemIndex.Should().Be(testCase.ExpectedIndex);
        ex.Which.FileOffset.Should().Be(testCase.ExpectedOffset);
    }

    [Fact]
    public void When_cloning_should_produce_copy()
    {
        var soundInfo = new SoundInfo
        {
            Category = SoundCategory.FxMain,
            Delay = 0,
            DopplerFactor = 0,
            Is3D = true,
            IsFolder = true,
            Loop = false,
            MaxRadius = 100,
            MinRadius = 10,
            Name = "Ship.Cargo",
            Pitch = 1,
            PitchVar = 0,
            Play = false,
            Priority = 1,
            Volume = 100F,
            VolumeVar = 0F,
            WaveName = "sample.wav"
        };

        // Act
        object cloned = soundInfo.Clone();

        // Assert
        cloned.Should()
            .BeOfType<SoundInfo>()
            .And.NotBeSameAs(soundInfo)
            .And.Be(soundInfo);
    }

    public static IEnumerable<object[]> TestCases()
    {
        Type thisType = typeof(SdlFileTests);
        Assembly thisAssembly = thisType.Assembly;

        yield return
        [
            thisAssembly.GetManifestResourceStream(thisType, "SdlWithOneItem.sdl"),
            new SoundInfo
            {
                Category = SoundCategory.FxMain,
                Delay = 0,
                DopplerFactor = 0,
                Is3D = true,
                IsFolder = true,
                Loop = false,
                MaxRadius = 100,
                MinRadius = 10,
                Name = "Ship.Cargo",
                Pitch = 1,
                PitchVar = 0,
                Play = false,
                Priority = 1,
                Volume = 100F,
                VolumeVar = 0F,
                WaveName = "sample.wav"
            }
        ];

        yield return
        [
            thisAssembly.GetManifestResourceStream(thisType, "SdlWithTwoItems.sdl"),
            new SoundInfo
            {
                Category = SoundCategory.FxMain,
                Delay = 0,
                DopplerFactor = 0,
                Is3D = true,
                IsFolder = true,
                Loop = false,
                MaxRadius = 100,
                MinRadius = 10,
                Name = "Air.Hurricane",
                Pitch = 1,
                PitchVar = 0,
                Play = false,
                Priority = 1,
                Volume = 99.9f,
                VolumeVar = 10,
                WaveName = "sample.wav"
            },
            new SoundInfo
            {
                Category = SoundCategory.Ambient,
                Delay = 15.1f,
                DopplerFactor = 8.3f,
                Is3D = true,
                IsFolder = false,
                Loop = true,
                MaxRadius = 51.1f,
                MinRadius = 8.2f,
                Name = "Animations.Multime01",
                Pitch = 1f,
                PitchVar = 1.7f,
                Play = true,
                Priority = 1,
                Volume = 94f,
                VolumeVar = 2.5f,
                WaveName = "anm_Crowd_01.wav"
            }
        ];

        yield return
        [
            // Only has S3D special header, and no items.
            thisAssembly.GetManifestResourceStream(thisType, "SdlWithS3DHeader.sdl")
        ];
    }

    public static IEnumerable<object[]> TestCasesWithInvalidData()
    {
        byte[] GetSoundInfo()
        {
            Type thisType = typeof(SdlFileTests);
            Assembly thisAssembly = thisType.Assembly;
            return thisAssembly.GetManifestResourceStream(thisType, "SdlWithOneItem.sdl").ToArray();
        }

        byte[] testData = GetSoundInfo();

        yield return [new SoundInfoTestCase { Data = testData.Concat(new byte[] { 0, 1, 2, 3, 4, 5 }).ToArray(), ExpectedError = "The file appears invalid. Unexpected size specifier encountered." }];

        testData = testData
            // Valid size.
            .Concat(new byte[] { 0x10, 0x01, 0x0C, 0x01, 0x00, 0x00 })
            .ToArray();

        yield return
        [
            new SoundInfoTestCase
            {
                // Expect 'SoundInfo'
                Data = testData.Concat(Encoding.UTF8.GetBytes("NotSoundInfo\0")).ToArray(), ExpectedError = "The file appears invalid. Unexpected item header encountered."
            }
        ];

        testData = testData.Concat(Encoding.UTF8.GetBytes("SoundInfo\0")).ToArray();

        yield return
        [
            new SoundInfoTestCase
            {
                // Expect 'Name' property
                Data = testData.Concat(new byte[] { 0, 1, 2, 3 }).Concat(Encoding.UTF8.GetBytes("NotN")).ToArray(), ExpectedError = "The file appears invalid. Expected property 'Name' but encountered 'NotN'."
            }
        ];
    }
}

public class SoundInfoTestCase
{
    public byte[] Data { get; set; }
    public int ExpectedIndex { get; set; } = 1;
    public int ExpectedOffset { get; set; } = 274;
    public string ExpectedError { get; set; }

    public override string ToString()
    {
        return ExpectedError;
    }
}
