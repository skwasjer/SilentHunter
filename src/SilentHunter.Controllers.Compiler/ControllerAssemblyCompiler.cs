using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

		private readonly ICSharpCompiler _compiler;

		public ControllerAssemblyCompiler(ICSharpCompiler compiler, string applicationName, string controllerPath)
		{
			_compiler = compiler ?? throw new ArgumentNullException(nameof(compiler));
			ApplicationName = applicationName ?? throw new ArgumentNullException(nameof(applicationName));
			ControllerPath = Path.GetFullPath(controllerPath ?? throw new ArgumentNullException(nameof(controllerPath)));
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
			if (!Directory.Exists(outputPath))
			{
				Directory.CreateDirectory(outputPath);
			}
			else if (force)
			{
				// Clean out existing artifacts, which forces a new build.
				CleanArtifacts();
				Directory.CreateDirectory(outputPath);
			}

			// Copy local dependencies.
			CopyLocalDependencies(AppDomain.CurrentDomain.BaseDirectory, outputPath);

			string asmShortName = AssemblyName ?? Path.GetFileNameWithoutExtension(ControllerPath);
			string asmOutputFile = Path.Combine(outputPath, asmShortName + ".dll");
			string docFile = Path.Combine(outputPath, asmShortName + ".xml");

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
							string path = rd.IsLocal ? Path.Combine(outputPath, rd.Location) : rd.Location;
							bool includeDetails = rd.IsLocal && File.Exists(path);
							return new CacheFileReference
							{
								Name = rd.Location,
								LastModified = includeDetails ? (DateTime?)File.GetLastWriteTimeUtc(path) : null,
								Length = includeDetails ? (long?)new FileInfo(path).Length : null,
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
						string depFilename = Path.Combine(p, requiredDependency.Location);
						return File.Exists(depFilename) ? depFilename : null;
					})
					.FirstOrDefault(p => p != null);

				if (dependencyFullPath == null || !File.Exists(dependencyFullPath))
				{
					throw new InvalidOperationException($"Unable to compiler controllers, the dependency {Path.GetFileName(requiredDependency.Location)} is required but not found in any of the search paths.");
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

			return Path.Combine(Path.GetTempPath(), ApplicationName, "SilentHunter.Controllers", frameworkVersion);
		}

		/// <summary>
		/// Gets *.cs files in specified folder and sorts them where Silent Hunter specific versions come first.
		/// </summary>
		/// <param name="path">The path where the controllers are located.</param>
		/// <returns>A list of controller files.</returns>
		private ICollection<CacheFileReference> GetCSharpFiles(string path)
		{
			string p = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), path));
			return new DirectoryInfo(p)
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
			string outputPath = Path.GetDirectoryName(outputFile);

			bool mustCompile = true;
			var serializer = new XmlSerializer(typeof(CompilerBuildCache));

			// Load the cache file.
			if (File.Exists(cacheFile))
			{
				CompilerBuildCache oldCache;
				using (FileStream fs = File.OpenRead(cacheFile))
				{
					oldCache = (CompilerBuildCache)serializer.Deserialize(fs);
				}

				// Check if cache is out of sync and that all files exist.
				mustCompile = !File.Exists(outputFile)
				 || !oldCache.Equals(assemblyCache)
				 || oldCache.SourceFiles.Any(src => !File.Exists(Path.Combine(controllerPath, src.Name)));
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
						string localFilePath = Path.Combine(outputPath, d.Name);
						return File.Exists(localFilePath) ? localFilePath : d.Name;
					})
					.ToArray()
			};

			_compiler.CompileCode(assemblyCache.SourceFiles.Select(cs => Path.Combine(controllerPath, cs.Name)).ToArray(), compilerOptions);

			// Save the cache file.
			using (FileStream fs = File.Open(cacheFile, FileMode.Create, FileAccess.Write, FileShare.Read))
			{
				serializer.Serialize(fs, assemblyCache);
			}
		}

		private static void CopyDependencyIfModified(string sourceFile, string destDirectory)
		{
			if (sourceFile == null)
			{
				throw new ArgumentNullException(nameof(sourceFile));
			}

			string destPath = Path.Combine(destDirectory, Path.GetFileName(sourceFile));
			if (!File.Exists(destPath) || File.GetLastWriteTimeUtc(destPath) != File.GetLastWriteTimeUtc(sourceFile))
			{
				File.Copy(sourceFile, destPath, true);
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
			if (Directory.Exists(dir))
			{
				Directory.Delete(dir, true);
			}
		}
	}
}