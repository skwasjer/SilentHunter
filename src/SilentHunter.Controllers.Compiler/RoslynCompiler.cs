#if NETSTANDARD2_0
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;

namespace SilentHunter.Controllers.Compiler
{
    /// <summary>
    /// Wraps the Roslyn compiler for use by the controller compiler.
    /// </summary>
    public class RoslynCompiler : ICSharpCompiler
    {
        private readonly IFileSystem _fileSystem;
        private readonly CSharpParseOptions _parseOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoslynCompiler" />.
        /// </summary>
        public RoslynCompiler()
            : this(new FileSystem())
        {
        }

        // ReSharper disable once MemberCanBePrivate.Global - justification: used by unit test
        internal RoslynCompiler(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _parseOptions = new CSharpParseOptions(LanguageVersion.CSharp8, DocumentationMode.Diagnose);
        }

        /// <inheritdoc />
        public Assembly CompileCode(ICollection<string> fileNames, CompilerOptions options, bool loadAssembly = false)
        {
            if (fileNames == null)
            {
                throw new ArgumentNullException(nameof(fileNames));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (fileNames.Count == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(fileNames), "Expected at least one filename.");
            }

            List<PortableExecutableReference> references = options.ReferencedAssemblies?.Select(ra => MetadataReference.CreateFromFile(ra)).ToList() ?? new List<PortableExecutableReference>();

            var compilation = CSharpCompilation.Create(
                _fileSystem.Path.GetFileNameWithoutExtension(options.OutputFile),
                fileNames.Select(fn => CSharpSyntaxTree.ParseText(SourceText.From(_fileSystem.File.ReadAllText(fn), Encoding.UTF8), _parseOptions).WithFilePath(fn)),
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            string outputDir = _fileSystem.Path.GetDirectoryName(options.OutputFile);
            if (!_fileSystem.Directory.Exists(outputDir))
            {
                _fileSystem.Directory.CreateDirectory(outputDir);
            }

            string pdbPath = _fileSystem.Path.Combine(outputDir, _fileSystem.Path.GetFileNameWithoutExtension(options.OutputFile) + ".pdb");
            using (Stream dllStream = _fileSystem.File.Open(options.OutputFile, FileMode.Create))
            using (Stream pdbStream = _fileSystem.File.Open(pdbPath, FileMode.Create))
            using (Stream docStream = string.IsNullOrEmpty(options.DocFile) ? Stream.Null : _fileSystem.File.Open(options.DocFile, FileMode.Create))
            {
                EmitResult emitResult = compilation.Emit(dllStream, pdbStream, docStream);
                LogResults(options, emitResult);

                // Display a successful compilation message.
                Debug.WriteLine("Code built into assembly '{0}' successfully.", options.OutputFile);
            }

            return loadAssembly ? Assembly.Load(_fileSystem.File.ReadAllBytes(options.OutputFile)) : null;
        }

        private static void LogResults(CompilerOptions options, EmitResult results)
        {
            //			results.Output.Cast<string>().ToList().ForEach(s => Debug.WriteLine(s));
            if (!results.Success)
            {
                // Display compilation errors.
                var errorMsg = new StringBuilder();
                errorMsg.AppendLine("Errors building assembly...");
                int errorCount = 0;
                foreach (IGrouping<Location, Diagnostic> error in results.Diagnostics
                             .Where(d => d.Severity >= DiagnosticSeverity.Warning)
                             .Where(d => options.IgnoreCompilerErrors == null || !options.IgnoreCompilerErrors.Contains(d.Id))
                             .GroupBy(d => d.Location))
                {
                    foreach (Diagnostic diagnostic in error)
                    {
                        errorMsg.AppendFormat("  - {0}", diagnostic);
                        errorMsg.AppendLine();
                    }

                    errorCount++;
                }

                if (errorCount == 0)
                {
                    return;
                }

                Debug.Write(errorMsg.ToString());
                throw new CompileException(errorMsg.ToString());
            }
        }
    }
}
#endif
