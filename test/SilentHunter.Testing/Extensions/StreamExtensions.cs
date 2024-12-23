using System.IO;

namespace SilentHunter.Testing.Extensions;

public static class StreamExtensions
{
    public static byte[] ToArray(this Stream stream)
    {
        if (stream is MemoryStream ms)
        {
            return ms.ToArray();
        }

        using (var ms1 = new MemoryStream())
        {
            stream.CopyTo(ms1);
            return ms1.ToArray();
        }
    }
}