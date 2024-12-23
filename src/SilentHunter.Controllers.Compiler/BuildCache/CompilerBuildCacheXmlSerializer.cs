using System;
using System.IO;
using System.Xml.Serialization;

namespace SilentHunter.Controllers.Compiler.BuildCache;

[Obsolete("Should be removed. This was the old way of serializing build cache, but it requires our cache classes to be public.")]
internal class CompilerBuildCacheXmlSerializer : ICompilerBuildCacheSerializer
{
    private readonly XmlSerializer _serializer;

    public CompilerBuildCacheXmlSerializer()
    {
        _serializer = new XmlSerializer(typeof(CompilerBuildCache));
    }

    public void Serialize(Stream stream, CompilerBuildCache buildCache)
    {
        _serializer.Serialize(stream, buildCache);
    }

    public CompilerBuildCache Deserialize(Stream stream)
    {
        return (CompilerBuildCache)_serializer.Deserialize(stream);
    }
}
