using System.Text;

namespace SilentHunter.FileFormats;

/// <summary>
/// Defines the text encoding of Silent Hunter game files.
/// </summary>
public static class FileEncoding
{
    /// <summary>
    /// The default text encoding to use for parsing Silent Hunter game files.
    /// </summary>
    public static Encoding Default { get; } = Encoding.GetEncoding("ISO-8859-1");
}
