﻿using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using SilentHunter.FileFormats.Extensions;

namespace SilentHunter.FileFormats.Graphics;

/// <summary>
/// Determines if an image is a TGA image.
/// </summary>
public class TgaImageFormatDetector : IImageFormatDetector
{
    /// <inheritdoc />
    public string GetImageFormat(Stream stream)
    {
        try
        {
            return ValidateTgaStream(stream, false) ? "tga" : null;
        }
        catch
        {
            return null;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    // ReSharper disable MemberCanBePrivate.Local
    // ReSharper disable FieldCanBeMadeReadOnly.Local
    // ReSharper disable IdentifierTypo
    // ReSharper disable InconsistentNaming
    private struct TGA_HEADER
    {
        /// <summary>
        /// Size of ID field that follows 18 byte header (0 usually).
        /// </summary>
        public byte identsize;

        /// <summary>Type of color map 0=none, 1=has palette.</summary>
        public byte colormaptype;

        /// <summary>
        /// Type of image 0=none, 1=indexed, 2=rgb, 3=grey, 9=indexedRLE, 10=rgbRLE, 11=greyRLE.
        /// </summary>
        public byte imagetype;

        /// <summary>First color map entry in palette.</summary>
        public short colormapstart;

        /// <summary>Number of colors in palette.</summary>
        public short colormaplength;

        /// <summary>Number of bits per palette entry 15, 16, 24, 32.</summary>
        public byte colormapbits;

        /// <summary>Image x origin.</summary>
        public short xstart;

        /// <summary>Image y origin.</summary>
        public short ystart;

        /// <summary>Image width in pixels.</summary>
        public short width;

        /// <summary>Image height in pixels.</summary>
        public short height;

        /// <summary>Image bits per pixel 8, 16, 24, 32</summary>
        public byte bits;

        /// <summary>Image descriptor bits (vh flip bits).</summary>
        public byte descriptor;

        public TgaPixelFormat PixelFormat => (TgaPixelFormat)(imagetype - (imagetype > (byte)8 ? 8 : 0));

        public bool HasPalette => colormaptype == 1;
    }
    // ReSharper restore InconsistentNaming
    // ReSharper restore IdentifierTypo
    // ReSharper restore FieldCanBeMadeReadOnly.Local
    // ReSharper restore MemberCanBePrivate.Local

    /// <summary>
    /// Specifies the format of the color data for each pixel in the image.
    /// </summary>
    private enum TgaPixelFormat
    {
        /// <summary>No image data present.</summary>
        None,

        /// <summary>Indexed (using palette).</summary>
        Indexed,

        /// <summary>True color.</summary>
        Rgb,

        /// <summary>Greyscale.</summary>
        Greyscale
    }

    /// <summary>
    /// Validates stream contains valid TGA data.
    /// </summary>
    private bool ValidateTgaStream(Stream stream, bool tryApplyFixes)
    {
        if (stream == null)
        {
            throw new ArgumentNullException();
        }

        if (!stream.CanSeek)
        {
            throw new ArgumentException("The stream does not support seeking.");
        }

        if (!stream.CanRead)
        {
            throw new ArgumentException("The stream does not support reading.");
        }

        if (stream.Length < Marshal.SizeOf(typeof(TGA_HEADER)))
        {
            return false;
        }

        // TODO: use ILogger instead of Debug.WriteLine.

        long startPosition = stream.Position;
        Encoding parseEncoding = Encoding.ASCII;
        TGA_HEADER tgaHeader;
        try
        {
            using (var reader = new BinaryReader(stream, parseEncoding, true))
            {
                tgaHeader = (TGA_HEADER)reader.ReadStruct(typeof(TGA_HEADER));
                if (tgaHeader.identsize > 0)
                {
                    string imageId = reader.ReadString(tgaHeader.identsize).TrimEnd('\0');
                    if (!string.IsNullOrEmpty(imageId))
                    {
                        Debug.WriteLine($"Has ImageID: {imageId}.");
                    }
                }

                if (!Enum.IsDefined(typeof(TgaPixelFormat), tgaHeader.PixelFormat))
                {
                    Debug.WriteLine($"Unknown image data type {tgaHeader.PixelFormat}.");
                    return false;
                }

                if (tgaHeader.PixelFormat == TgaPixelFormat.Indexed)
                {
                    if (!tgaHeader.HasPalette)
                    {
                        Debug.WriteLine("Palette is required but unavailable.");
                        return false;
                    }

                    // Check if all the color map data is available.
                    int paletteLength = tgaHeader.colormapbits / 8 * tgaHeader.colormaplength;
                    var colorMap = new byte[paletteLength];
                    if (reader.Read(colorMap, 0, paletteLength) != paletteLength)
                    {
                        Debug.WriteLine("Palette is not valid.");
                        return false;
                    }
                }
            }
        }
        finally
        {
            stream.Position = startPosition;
        }

        if (!CheckAndFixBugsIfPossible(stream, ref tgaHeader, tryApplyFixes))
        {
            return false;
        }

        return VerifyPixelFormat(tgaHeader);
    }

    private static bool VerifyPixelFormat(TGA_HEADER tgaHeader)
    {
        switch (tgaHeader.bits)
        {
            case 8:
                return tgaHeader.HasPalette && tgaHeader.PixelFormat == TgaPixelFormat.Indexed
                 || tgaHeader.PixelFormat == TgaPixelFormat.Greyscale;

            case 16:
            case 24:
            case 32:
                return true;

            default:
                return false;
        }
    }

    private static bool CheckAndFixBugsIfPossible(Stream stream, ref TGA_HEADER tgaHeader, bool tryApplyFixes)
    {
        bool doesNotUsePalette = !tgaHeader.HasPalette && tgaHeader.PixelFormat != TgaPixelFormat.Indexed;
        bool hasColorMapStartOrLength = tgaHeader.colormapstart != 0 || tgaHeader.colormaplength != 0;
        // Some mods have invalid values here.
        // Reason: for non indexed TGA's, the colormap start and length MUST not be anything other than 0.
        // http://www.ludorg.net/amnesia/TGA_File_Format_Spec.html
        if (!doesNotUsePalette || !hasColorMapStartOrLength)
        {
            return true;
        }

        if (!stream.CanWrite || !tryApplyFixes)
        {
            // Unable to patch header, so the stream is not considered a valid TGA stream.
            Debug.WriteLine("TGA header contains invalid data, but did not patch header.");
            return false;
        }

        long startPosition = stream.Position;
        try
        {
            tgaHeader.colormapstart = 0;
            tgaHeader.colormaplength = 0;
            using (var writer = new BinaryWriter(stream, Encoding.ASCII, true))
            {
                writer.WriteStruct(tgaHeader);
            }

            Debug.WriteLine("Patched TGA header with invalid data.");
        }
        finally
        {
            stream.Position = startPosition;
        }

        return true;
    }

    /// <summary>
    /// Applies fixes, returns true if the TGA is valid after fixes.
    /// </summary>
    internal bool TryApplyFixes(Stream stream)
    {
        return ValidateTgaStream(stream, true);
    }
}