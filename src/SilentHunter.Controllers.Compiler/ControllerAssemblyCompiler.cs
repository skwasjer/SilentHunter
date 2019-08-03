using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.Versioning;
using System.Xml.Serialization;

namespace SilentHunter.Controllers.Compiler
{
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

		private static readonly List<Dependency> RequiredDependencies = new List<Dependency>
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
		private readonly ICSharpCompiler _compiler;

		public ControllerAssemblyCompiler(ICSharpCompiler compiler, string applicationName, string controllerPath)
			: this(new FileSystem(), compiler, applicationName, controllerPath)
		{
		}

		public ControllerAssemblyCompiler(IFileSystem fileSystem, ICSharpCompiler compiler, string applicationName, string controllerPath)
		{
			_fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
			_compiler = compiler ?? throw new ArgumentNullException(nameof(compiler));
			ApplicationName = applicationName ?? throw new ArgumentNullException(nameof(applicationName));
			ControllerPath = _fileSystem.Path.GetFullPath(controllerPath ?? throw new ArgumentNullException(nameof(controllerPath)));
		}

		/// <summary>
		/// Gets the full controller path.
		/// </summary>
		public string ControllerPath { get; }

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
		public Func<string, bool> IgnorePaths { get; set; }

		/// <summary>
		/// Gets or sets dependency search paths.
		/// </summary>
		public IEnumerable<string> DependencySearchPaths { get; set; }

		public Assembly Compile(bool force = false)
		{
			string outputPath = GetTargetDir();
			if (!_fileSystem.Directory.Exists(outputPath))
			{
				_fileSystem.Directory.CreateDirectory(outputPath);
			}
			else if (force)
			{
				// Clean out existing artifacts, which forces a new build.
				CleanArtifacts();
				_fileSystem.Directory.CreateDirectory(outputPath);
			}

			// Copy local dependencies.
			CopyLocalDependencies(AppDomain.CurrentDomain.BaseDirectory, outputPath);

			string asmShortName = AssemblyName ?? _fileSystem.Path.GetFileNameWithoutExtension(ControllerPath);
			string asmOutputFile = _fileSystem.Path.Combine(outputPath, asmShortName + ".dll");
			string docFile = _fileSystem.Path.Combine(outputPath, asmShortName + ".xml");

			ICollection<CacheFileReference> sourceFiles = GetCSharpFiles(ControllerPath);
			if (!sourceFiles.Any())
			{
				throw new InvalidOperationException("No controller source files found.");
			}

			// Generate cache file for the specified controller path, which is used to determine if
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
				Dependencies = new HashSet<CacheFileReference>(
					RequiredDependencies
						.Select(rd =>
						{
							string path = rd.IsLocal ? _fileSystem.Path.Combine(outputPath, rd.Location) : rd.Location;
							bool includeDetails = rd.IsLocal && _fileSystem.File.Exists(path);
							return new CacheFileReference
							{
								Name = rd.Location,
								LastModified = includeDetails ? (DateTime?)_fileSystem.File.GetLastWriteTimeUtc(path) : null,
								Length = includeDetails ? (long?)_fileSystem.FileInfo.FromFileName(path).Length : null,
							};
						})
				),
				SourceFiles = new HashSet<CacheFileReference>(sourceFiles)
			};

			Compile(newCache, ControllerPath, asmOutputFile, docFile);

			// Load controller assembly also into local domain. You normally wouldn't want this, because you A) couldn't unload it and B) would create security issues. May have to look at using isolated domain.
			return Assembly.LoadFile(asmOutputFile);
		}

		private void CopyLocalDependencies(string baseDirectory, string outputPath)
		{
			foreach (Dependency requiredDependency in RequiredDependencies.Where(rd => rd.IsLocal))
			{
				// Find the dependency.
				string dependencyFullPath = (DependencySearchPaths ?? new List<string>())
					.Union(new[] { baseDirectory })
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

				CopyDependencyIfModified(dependencyFullPath, outputPath);
			}
		}

		private string GetTargetDir()
		{
			string frameworkVersion = Assembly.GetExecutingAssembly()
				.GetCustomAttribute<TargetFrameworkAttribute>()
				.FrameworkName.ToLowerInvariant()
				.Replace(",version=v", string.Empty)
				.TrimStart('.');

			if (frameworkVersion.StartsWith("netframework"))
			{
				frameworkVersion = frameworkVersion
					.Replace("netframework", "net")
					.Replace(".", string.Empty);
			}

			return _fileSystem.Path.Combine(_fileSystem.Path.GetTempPath(), ApplicationName, "SilentHunter.Controllers", frameworkVersion);
		}

		/// <summary>
		/// Gets *.cs files in specified folder and sorts them where Silent Hunter specific versions come first.
		/// </summary>
		/// <param name="path">The path where the controllers are located.</param>
		/// <returns>A list of controller files.</returns>
		private ICollection<CacheFileReference> GetCSharpFiles(string path)
		{
			string p = _fileSystem.Path.GetFullPath(_fileSystem.Path.Combine(_fileSystem.Directory.GetCurrentDirectory(), path));
			return _fileSystem.DirectoryInfo.FromDirectoryName(p)
				.GetFiles("*.cs", SearchOption.AllDirectories)
				.Where(f => IgnorePaths == null || !IgnorePaths(f.FullName))
				.Select(f =>
					new CacheFileReference
					{
						// Save relative name.
						Name = f.FullName.Substring(p.Length + 1),
						LastModified = f.LastWriteTimeUtc,
						Length = f.Length
					}
				)
				.ToList();
		}

		private void Compile(CompilerBuildCache assemblyCache, string controllerPath, string outputFile, string docFile)
		{
			string cacheFile = outputFile + ".cache";
			string outputPath = _fileSystem.Path.GetDirectoryName(outputFile);

			bool mustCompile = true;
			var serializer = new XmlSerializer(typeof(CompilerBuildCache));

			// Load the cache file.
			if (_fileSystem.File.Exists(cacheFile))
			{
				CompilerBuildCache oldCache;
				using (Stream fs = _fileSystem.File.OpenRead(cacheFile))
				{
					oldCache = (CompilerBuildCache)serializer.Deserialize(fs);
				}

				// Check if cache is out of sync and that all files exist.
				mustCompile = !_fileSystem.File.Exists(outputFile)
				 || !oldCache.Equals(assemblyCache)
				 || oldCache.SourceFiles.Any(src => !_fileSystem.File.Exists(_fileSystem.Path.Combine(controllerPath, src.Name)));
			}

			// If no changes in source files, return.
			if (!mustCompile)
			{
				return;
			}

			// NOTE: Ensure LoaderLock Managed Debugging Assistant in Exception settings is disabled, to allow VS to run dynamic compilation within IDE.

			// Compile the source files, etc.
			var compilerOptions = new CompilerOptions
			{
				OutputPath = outputFile,
				DocFile = docFile,
				ReferencedAssemblies = assemblyCache.Dependencies.Select(d =>
					{
						string localFilePath = _fileSystem.Path.Combine(outputPath, d.Name);
						return _fileSystem.File.Exists(localFilePath) ? localFilePath : d.Name;
					})
					.ToArray()
			};

			_compiler.CompileCode(assemblyCache.SourceFiles.Select(cs => _fileSystem.Path.Combine(controllerPath, cs.Name)).ToArray(), compilerOptions);

			// Save the cache file.
			using (Stream fs = _fileSystem.File.Open(cacheFile, FileMode.Create, FileAccess.Write, FileShare.Read))
			{
				serializer.Serialize(fs, assemblyCache);
			}
		}

		private void CopyDependencyIfModified(string sourceFile, string destDirectory)
		{
			if (sourceFile == null)
			{
				throw new ArgumentNullException(nameof(sourceFile));
			}

			string destPath = _fileSystem.Path.Combine(destDirectory, _fileSystem.Path.GetFileName(sourceFile));
			if (!_fileSystem.File.Exists(destPath) || _fileSystem.File.GetLastWriteTimeUtc(destPath) != _fileSystem.File.GetLastWriteTimeUtc(sourceFile))
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
}