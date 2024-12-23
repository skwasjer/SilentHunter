using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Numerics;
using System.Reflection;
using SilentHunter.Controllers.Compiler.BuildCache;

namespace SilentHunter.Controllers.Compiler;

/// <summary>
/// Represents a controller template assembly compiler.
/// </summary>
public class ControllerAssemblyCompiler : IControllerAssemblyCompiler
{
    private class Dependency
    {
        public Dependency(Type type, bool isLocal = false)
            : this(type.Assembly.Location, isLocal)
        {
        }

        public Dependency(string location, bool isLocal = false)
        {
            Location = location;
            IsLocal = isLocal;
        }

        public string Location { get; }
        public bool IsLocal { get; }
    }

    private static readonly List<Dependency> RequiredDependencies = new()
    {
#if NETFRAMEWORK
        new Dependency("System.dll"),
#else
        new Dependency(Assembly.Load("netstandard, Version=2.0.0.0").Location),
        new Dependency(typeof(object)),
        new Dependency(Assembly.Load("System.Runtime, PublicKeyToken=b03f5f7f11d50a3a").Location),
        new Dependency(Assembly.Load("System.Runtime.Extensions, PublicKeyToken=b03f5f7f11d50a3a").Location),
#endif
        new Dependency(typeof(Vector2)),
        new Dependency("SilentHunter.Core.dll", true)
    };

    private readonly IFileSystem _fileSystem;
    private readonly ICompilerBuildCacheSerializer _cacheSerializer;
    private readonly ICSharpCompiler _compiler;

    /// <summary>
    /// Initializes a new instance of the <see cref="ControllerAssemblyCompiler" /> class.
    /// </summary>
    /// <param name="compiler">The compiler to use.</param>
    /// <param name="applicationName">The application name. This is used in the temp path used to generate the assembly, as to not conflict with other applications that also dynamically compile SilentHunter controllers.</param>
    /// <param name="controllerDir">The path containing controller source files.</param>
    public ControllerAssemblyCompiler(ICSharpCompiler compiler, string applicationName, string controllerDir)
        : this(new FileSystem(), new CompilerBuildCacheJsonSerializer(), compiler, applicationName, controllerDir)
    {
    }

    internal ControllerAssemblyCompiler(IFileSystem fileSystem, ICompilerBuildCacheSerializer cacheSerializer, ICSharpCompiler compiler, string applicationName, string controllerDir)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _cacheSerializer = cacheSerializer ?? throw new ArgumentNullException(nameof(cacheSerializer));
        _compiler = compiler ?? throw new ArgumentNullException(nameof(compiler));
        ApplicationName = applicationName ?? throw new ArgumentNullException(nameof(applicationName));
        ControllerDir = _fileSystem.Path.GetFullPath(controllerDir ?? throw new ArgumentNullException(nameof(controllerDir)));
    }

    /// <summary>
    /// Gets the full controller path.
    /// </summary>
    public string ControllerDir { get; }

    /// <summary>
    /// Gets the application name. This is used to determine the output location of the compiled assembly.
    /// </summary>
    public string ApplicationName { get; }

    /// <summary>
    /// Gets or sets the assembly name. If null, uses the last directory from controller path.
    /// </summary>
    public string AssemblyName { get; set; }

    /// <summary>
    /// Gets or sets a filter to ignore specific paths while searching for source files.
    /// </summary>
    public Func<string, bool> IgnoreDirs { get; set; }

    /// <summary>
    /// Gets or sets dependency search paths.
    /// </summary>
    public IEnumerable<string> DependencySearchDirs { get; set; }

    /// <inheritdoc />
    public Assembly Compile(bool force = false)
    {
        string outputDir = GetTargetDir();
        if (!_fileSystem.Directory.Exists(outputDir))
        {
            _fileSystem.Directory.CreateDirectory(outputDir);
        }
        else if (force)
        {
            // Clean out existing artifacts, which forces a new build.
            CleanArtifacts();
            _fileSystem.Directory.CreateDirectory(outputDir);
        }

        // Copy local dependencies.
        CopyLocalDependencies(AppDomain.CurrentDomain.BaseDirectory, outputDir);

        string asmShortName = AssemblyName ?? _fileSystem.Path.GetFileNameWithoutExtension(ControllerDir);
        string asmOutputFile = _fileSystem.Path.Combine(outputDir, asmShortName + ".dll");
        string docFile = _fileSystem.Path.Combine(outputDir, asmShortName + ".xml");

        ICollection<CacheFileReference> sourceFiles = GetCSharpFiles(ControllerDir);
        if (!sourceFiles.Any())
        {
            throw new InvalidOperationException("No controller source files found.");
        }

        CompilerBuildCache newCache = PrepareBuildCache(sourceFiles, outputDir);

        Compile(newCache, asmOutputFile, docFile);

        // Load controller assembly also into local domain. You normally wouldn't want this, because you A) couldn't unload it and B) would create security issues. May have to look at using isolated domain.
        return Assembly.LoadFile(asmOutputFile);
    }

    private CompilerBuildCache PrepareBuildCache(IEnumerable<CacheFileReference> sourceFiles, string outputDir)
    {
        // Generate cache file for the specified controller dir, which is used to determine if
        // we have to rebuild the controller assembly. If one of the files has changed since last build
        // or a file is missing/added, the assembly will be rebuilt.
        var newCache = new CompilerBuildCache
        {
#if DEBUG
            BuildConfiguration = "debug",
#else
            BuildConfiguration = "release",
#endif
            Version = FileVersionInfo.GetVersionInfo(typeof(ControllerAssemblyCompiler).Assembly.Location).FileVersion,
            Dependencies =
            [

                ..RequiredDependencies
                    .Select(rd =>
                    {
                        string path = rd.IsLocal ? _fileSystem.Path.Combine(outputDir, rd.Location) : rd.Location;
                        bool includeDetails = rd.IsLocal && _fileSystem.File.Exists(path);
                        return new CacheFileReference
                        {
                            Name = rd.Location,
                            LastModified = includeDetails ? (DateTime?)_fileSystem.File.GetLastWriteTimeUtc(path) : null,
                            Length = includeDetails ? (long?)_fileSystem.FileInfo.FromFileName(path).Length : null
                        };
                    })

            ],
            SourceFiles = [..sourceFiles]
        };
        return newCache;
    }

    private void CopyLocalDependencies(string baseDirectory, string outputDir)
    {
        foreach (Dependency requiredDependency in RequiredDependencies.Where(rd => rd.IsLocal))
        {
            // Find the dependency.
            string dependencyFullPath = (DependencySearchDirs ?? new List<string>())
                .Union([baseDirectory]) // Always include base dir
                .Select(p =>
                {
                    string depFilename = _fileSystem.Path.Combine(p, requiredDependency.Location);
                    return _fileSystem.File.Exists(depFilename) ? depFilename : null;
                })
                .FirstOrDefault(p => p != null);

            if (dependencyFullPath == null || !_fileSystem.File.Exists(dependencyFullPath))
            {
                throw new InvalidOperationException($"Unable to compiler controllers, the dependency {_fileSystem.Path.GetFileName(requiredDependency.Location)} is required but not found in any of the search paths.");
            }

            CopyDependencyIfModified(dependencyFullPath, outputDir);
        }
    }

    private string GetTargetDir()
    {
        return _fileSystem.Path.Combine(
            _fileSystem.Path.GetTempPath(),
            ApplicationName,
            "SilentHunter.Controllers",
            EnvironmentUtilities.GetCurrentTargetFramework()
        );
    }

    /// <summary>
    /// Gets *.cs files in specified folder and sorts them where Silent Hunter specific versions come first.
    /// </summary>
    /// <param name="sourceFileDir">The path where the controllers are located.</param>
    /// <returns>A list of controller files.</returns>
    private ICollection<CacheFileReference> GetCSharpFiles(string sourceFileDir)
    {
        return new CacheFileReferenceEnumerator(_fileSystem, sourceFileDir)
            .Where(f => IgnoreDirs == null || !IgnoreDirs(f.Name))
            .ToList();
    }

    private void Compile(CompilerBuildCache assemblyCache, string outputFile, string docFile)
    {
        // If no changes in source files, return.
        if (!GetIfMustRecompile(assemblyCache, outputFile))
        {
            return;
        }

        // NOTE: Ensure LoaderLock Managed Debugging Assistant in Exception settings is disabled, to allow VS to run dynamic compilation within IDE.

        // Compile the source files, etc.
        string outputDir = _fileSystem.Path.GetDirectoryName(outputFile);
        var compilerOptions = new CompilerOptions
        {
            OutputFile = outputFile,
            DocFile = docFile,
            ReferencedAssemblies = assemblyCache.Dependencies.Select(d =>
                {
                    string localFilePath = _fileSystem.Path.Combine(outputDir, d.Name);
                    return _fileSystem.File.Exists(localFilePath) ? localFilePath : d.Name;
                })
                .ToArray()
        };

        _compiler.CompileCode(assemblyCache.SourceFiles.Select(cs => _fileSystem.Path.Combine(ControllerDir, cs.Name)).ToArray(), compilerOptions);

        SaveBuildCache(assemblyCache, outputFile);
    }

    private void SaveBuildCache(CompilerBuildCache assemblyCache, string outputFile)
    {
        // Save the cache file.
        string cacheFile = GetBuildCacheFileName(outputFile);
        using Stream fs = _fileSystem.File.Open(cacheFile, FileMode.Create, FileAccess.Write, FileShare.Read);
        _cacheSerializer.Serialize(fs, assemblyCache);
    }

    private bool GetIfMustRecompile(CompilerBuildCache assemblyCache, string outputFile)
    {
        string cacheFile = GetBuildCacheFileName(outputFile);
        if (!_fileSystem.File.Exists(cacheFile))
        {
            return true;
        }

        CompilerBuildCache oldCache;
        try
        {
            // Load the cache file.
            using Stream fs = _fileSystem.File.OpenRead(cacheFile);
            oldCache = _cacheSerializer.Deserialize(fs);
        }
        catch
        {
            return true;
        }

        // Check if cache is not out of sync and that all files exist.
        return !_fileSystem.File.Exists(outputFile)
         || !oldCache.Equals(assemblyCache)
         || oldCache.SourceFiles.Any(src => !_fileSystem.File.Exists(_fileSystem.Path.Combine(ControllerDir, src.Name)));
    }

    private static string GetBuildCacheFileName(string outputFile)
    {
        return outputFile + ".cache";
    }

    private void CopyDependencyIfModified(string sourceFile, string destDirectory)
    {
        if (sourceFile == null)
        {
            throw new ArgumentNullException(nameof(sourceFile));
        }

        if (destDirectory == null)
        {
            throw new ArgumentNullException(nameof(destDirectory));
        }

        string destPath = _fileSystem.Path.Combine(destDirectory, _fileSystem.Path.GetFileName(sourceFile));
        if (!_fileSystem.File.Exists(destPath)
         || _fileSystem.File.GetLastWriteTimeUtc(destPath) != _fileSystem.File.GetLastWriteTimeUtc(sourceFile))
        {
            _fileSystem.File.Copy(sourceFile, destPath, true);
        }
    }

    /// <summary>
    /// Cleans the assembly output directory.
    /// </summary>
    /// <remarks>
    /// This removes the all files from previous builds, including cache.
    /// </remarks>
    public void CleanArtifacts()
    {
        string dir = GetTargetDir();
        if (_fileSystem.Directory.Exists(dir))
        {
            _fileSystem.Directory.Delete(dir, true);
        }
    }
}
