using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SilentHunter.Controllers.Compiler.BuildCache;

internal class CompilerBuildCacheJsonSerializer : ICompilerBuildCacheSerializer
{
    private readonly JsonSerializer _serializer;

    public CompilerBuildCacheJsonSerializer()
    {
        _serializer = new JsonSerializer
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented,
            ContractResolver = new CamelCasePropertyNamesContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            }
        };
    }

    public void Serialize(Stream stream, CompilerBuildCache buildCache)
    {
        using (var sw = new StreamWriter(stream))
        using (var writer = new JsonTextWriter(sw))
        {
            _serializer.Serialize(writer, buildCache);
        }
    }

    public CompilerBuildCache Deserialize(Stream stream)
    {
        using (var sr = new StreamReader(stream))
        using (var reader = new JsonTextReader(sr))
        {
            return _serializer.Deserialize<CompilerBuildCache>(reader);
        }
    }
}