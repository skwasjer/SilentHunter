#if NETCOREAPP
using System.Collections.Generic;

namespace SilentHunter.Controllers.Compiler.Tests
{
    public class RoslynCompilerTests : CompilerTests<RoslynCompiler>
    {
        protected override List<string> GetReferencedAssemblies()
        {
            return [typeof(object).Assembly.Location];
        }
    }
}
#endif
