using System.Collections.Generic;

namespace SilentHunter.Testing.FluentAssertions;

public static class ByteCollectionAssertionsExtensions
{
    public static ByteCollectionAssertions Should(this IEnumerable<byte> byteCollection)
    {
        return new ByteCollectionAssertions(byteCollection);
    }
}