using System.IO;

namespace SilentHunter.Controllers.Compiler.BuildCache;

internal interface ICompilerBuildCacheSerializer
{
    void Serialize(Stream stream, CompilerBuildCache buildCache);

    CompilerBuildCache Deserialize(Stream stream);
}