using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SilentHunter.FileFormats.Graphics;

namespace SilentHunter.FileFormats.Dat.Chunks;

/// <summary>
/// Represents an embedded image. The image can be a TGA or DDS.
/// </summary>
[DebuggerDisplay("{ToString(),nq}: {ImageFormat}, Length = {Length}")]
public sealed class EmbeddedImageChunk : DatChunk
{
    private readonly ImageFormatDetection _imageFormatDetection;
    private byte[] _buffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddedImageChunk" /> class.
    /// </summary>
    public EmbeddedImageChunk()
        : this(ImageFormatDetection.Default)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddedImageChunk" /> class.
    /// </summary>
    /// <param name="imageFormatDetection">The image format detector.</param>
    internal EmbeddedImageChunk(ImageFormatDetection imageFormatDetection)
        : base(DatFile.Magics.EmbeddedImage)
    {
        _imageFormatDetection = imageFormatDetection ?? throw new ArgumentNullException(nameof(imageFormatDetection));
    }

    /// <summary>
    /// Gets the image data length in bytes.
    /// </summary>
    public long Length { get => _buffer?.Length ?? 0; }

    /// <summary>
    /// Reads the image data as stream.
    /// </summary>
    /// <returns>the image data as stream.</returns>
    public Task<Stream> ReadAsStreamAsync()
    {
        return Task.FromResult<Stream>(new MemoryStream(_buffer ?? []));
    }

    /// <summary>
    /// Reads the image data as byte array.
    /// </summary>
    /// <returns>the image data as byte array.</returns>
    public Task<byte[]> ReadAsByteArrayAsync()
    {
        return Task.FromResult(_buffer?.ToArray() ?? []);
    }

    /// <summary>
    /// Writes new image data to the embedded image.
    /// </summary>
    /// <param name="imageData">The image data.</param>
    public async Task WriteAsync(Stream imageData)
    {
        if (imageData == null)
        {
            throw new ArgumentNullException(nameof(imageData));
        }

        if (!imageData.CanRead)
        {
            throw new ArgumentException("The stream does not support reading.", nameof(imageData));
        }

        long currentPos = imageData.Position;
        int bytesToRead = unchecked((int)(imageData.Length - currentPos));
        if (bytesToRead <= 0)
        {
            throw new ArgumentException("No data to read from stream.", nameof(imageData));
        }

        Stream localStream = imageData;
        try
        {
            if (!imageData.CanSeek)
            {
                localStream = new MemoryStream();
                await imageData.CopyToAsync(localStream).ConfigureAwait(false);
                localStream.Position = 0;
            }

            ImageFormat = _imageFormatDetection.GetFormat(localStream);
            if (ImageFormat == ImageFormat.Unknown)
            {
                // Attempt to apply fixes.
                TgaImageFormatDetector tgaDetector = _imageFormatDetection.Detectors.OfType<TgaImageFormatDetector>().FirstOrDefault();
                if (tgaDetector != null)
                {
                    using var ms = new MemoryStream();
                    await localStream.CopyToAsync(ms).ConfigureAwait(false);

                    ms.Position = 0;
                    if (tgaDetector.TryApplyFixes(ms))
                    {
                        ImageFormat = ImageFormat.Tga;
                    }

                    _buffer = ms.ToArray();
                    return;
                }
            }

            // Using indirection because we can't write directly to field, so we just use the same ref to the local var.
            byte[] buffer = _buffer = new byte[bytesToRead];
            await localStream.ReadAsync(buffer, 0, bytesToRead).ConfigureAwait(false);
        }
        finally
        {
            if (localStream != imageData)
            {
                localStream.Dispose();
            }
        }
    }

    /// <summary>
    /// Writes new image data to the embedded image.
    /// </summary>
    /// <param name="imageData">The image data.</param>
    public async Task WriteAsync(byte[] imageData)
    {
        if (imageData == null)
        {
            throw new ArgumentNullException(nameof(imageData));
        }

        using var ms = new MemoryStream(imageData);
        await WriteAsync(ms).ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override Task DeserializeAsync(Stream stream)
    {
        return WriteAsync(stream);
    }

    /// <inheritdoc />
    protected override async Task SerializeAsync(Stream stream)
    {
        using Stream imageData = await ReadAsStreamAsync().ConfigureAwait(false);
        await imageData.CopyToAsync(stream).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the detected image format of the image.
    /// </summary>
    public ImageFormat ImageFormat { get; private set; }
}
