using System.IO;

namespace SilentHunter.Controllers.Compiler.BuildCache;

internal interface ICompilerBuildCacheSerializer
{
    public void Serialize(Stream stream, CompilerBuildCache buildCache);

    public CompilerBuildCache Deserialize(Stream stream);
}
