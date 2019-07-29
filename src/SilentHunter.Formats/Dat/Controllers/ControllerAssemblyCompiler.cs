using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using SilentHunter.Dat.Controllers.Compiler;

namespace SilentHunter.Dat.Controllers
{
	public class ControllerAssemblyCompiler
	{
		private static readonly List<string> RequiredDependencies = new List<string>
		{
			"System.dll",
			"System.Windows.Forms.dll",
			"System.Drawing.dll",
			"skwas.IO.dll",
			"SilentHunter.Core.dll"
		};

		private string _controllerPath;
		private string _applicationName;
		private string _assemblyName;
		private ICollection<string> _dependencySearchPaths = new List<string>();

		public ControllerAssemblyCompiler(string controllerPath)
			: this(controllerPath, "S3D.Controllers")
		{
			ControllerPath(controllerPath);
			ApplicationName(_applicationName);
		}

		internal ControllerAssemblyCompiler(string controllerPath, string applicationName)
		{
			ControllerPath(controllerPath);
			ApplicationName(applicationName);
		}

		public ControllerAssemblyCompiler ControllerPath(string controllerPath)
		{
			_controllerPath = controllerPath ?? throw new ArgumentNullException(nameof(controllerPath));
			return this;
		}

		public ControllerAssemblyCompiler ApplicationName(string applicationName)
		{
			_applicationName = applicationName ?? throw new ArgumentNullException(nameof(applicationName));
			return this;
		}

		public ControllerAssemblyCompiler AssemblyName(string assemblyName)
		{
			_assemblyName = assemblyName ?? throw new ArgumentNullException(nameof(assemblyName));
			return this;
		}

		public ControllerAssemblyCompiler DependencySearchPaths(params string[] paths)
		{
			_dependencySearchPaths = paths ?? throw new ArgumentNullException(nameof(paths));
			return this;
		}

		public ControllerAssembly Compile()
		{
			AppDomain tempDomain = CreateTempAppDomain();
			string outputPath = tempDomain.DynamicDirectory;

			if (!Directory.Exists(outputPath))
			{
				Directory.CreateDirectory(outputPath);
			}

			// Copy local dependencies.
			CopyDependencies(tempDomain.BaseDirectory, outputPath);

			string asmShortName = _assemblyName ?? Path.GetFileNameWithoutExtension(_controllerPath);
			string asmOutputFile = Path.Combine(outputPath, asmShortName + ".dll");
			string docFile = Path.Combine(outputPath, asmShortName + ".xml");

			// Generate cache file for the specified controller path, which is used to determine if
			// we have to rebuild the controller assembly. If one of the files has changed since last build
			// or a file is missing/added, the assembly will be rebuilt.
			var newCache = new CSharpBuildCache
			{
#if DEBUG
				BuildConfiguration = "debug",
#else
				BuildConfiguration = "release",
#endif
				Version = FileVersionInfo.GetVersionInfo(typeof(ControllerAssemblyCompiler).Assembly.Location).FileVersion,
				Dependencies = new HashSet<CacheFileReference>(
					RequiredDependencies
						.Select(rd => new CacheFileReference
						{
							Name = rd,
							LastModified = File.GetLastWriteTimeUtc(Path.Combine(outputPath, rd))
						})
				),
				SourceFiles = new HashSet<CacheFileReference>(GetCSharpFiles(_controllerPath))
			};

			Compile(newCache, _controllerPath, asmOutputFile, docFile);

			// Load controller assembly also into local domain. You normally wouldn't want this, because you A) couldn't unload it and B) would create security issues. May have to look at using isolated domain.
			AssemblyName asmName = System.Reflection.AssemblyName.GetAssemblyName(asmOutputFile);
			return new ControllerAssembly(AppDomain.CurrentDomain.Load(asmName));
		}

		private void CopyDependencies(string baseDirectory, string outputPath)
		{
			foreach (string requiredDependency in RequiredDependencies)
			{
				// Find the dependency.
				string dependencyFullPath = _dependencySearchPaths
					.Union(new[]
					{
						baseDirectory
					})
					.Select(p =>
					{
						string depFilename = Path.Combine(p, requiredDependency);
						return File.Exists(depFilename) ? depFilename : null;
					})
					.FirstOrDefault(p => p != null);

				if (dependencyFullPath != null)
				{
					CopyDependencyIfModified(dependencyFullPath, outputPath);
				}
			}
		}

		private AppDomain CreateTempAppDomain()
		{
			// Prepare to create a new application domain.
			var setup = new AppDomainSetup
			{
				ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,

				// Set the application name before setting the dynamic base.
				ApplicationName = _applicationName,

				// Set the location of the base directory where assembly resolution 
				// probes for dynamic assemblies. Note that the hash code of the 
				// application name is concatenated to the base directory name you 
				// supply. 
				DynamicBase = Path.Combine(Path.GetTempPath(), "S3D", "dynamic", _applicationName)
			};

			Console.WriteLine("DynamicBase is set to '{0}'.", setup.DynamicBase);

			return AppDomain.CreateDomain(Guid.NewGuid().ToString(), null, setup);
		}

		/// <summary>
		/// Gets *.cs files in specified folder and sorts them where Silent Hunter specific versions come first.
		/// </summary>
		/// <param name="path">The path where the controllers are located.</param>
		/// <returns>A list of controller files.</returns>
		private static IEnumerable<CacheFileReference> GetCSharpFiles(string path)
		{
			return new DirectoryInfo(path)
				.GetFiles("*.cs", SearchOption.AllDirectories)
				.Select(f =>
					new CacheFileReference
					{
						// Save relative name.
						Name = f.FullName.Substring(path.Length + 1),
						LastModified = f.LastWriteTimeUtc
					}
				);
		}

		private static void Compile(CSharpBuildCache assemblyCache, string controllerPath, string outputFile, string docFile)
		{
			string cacheFile = outputFile + ".cache";
			string outputPath = Path.GetDirectoryName(outputFile);

			var mustCompile = true;
			var serializer = new XmlSerializer(typeof(CSharpBuildCache));

			// Load the cache file.
			if (File.Exists(cacheFile))
			{
				CSharpBuildCache oldCache;
				using (FileStream fs = File.OpenRead(cacheFile))
				{
					oldCache = (CSharpBuildCache)serializer.Deserialize(fs);
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
			using (var compiler = new CSharpCompiler
			{
				OutputPath = outputFile,
				DocFile = docFile,
				ReferencedAssemblies = assemblyCache.Dependencies.Select(d =>
					{
						string localFilePath = Path.Combine(outputPath, d.Name);
						return File.Exists(localFilePath) ? localFilePath : d.Name;
					})
					.ToArray()
			})
			{
				compiler.CompileCode(assemblyCache.SourceFiles.Select(cs => Path.Combine(controllerPath, cs.Name)).ToArray());
			}

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
	}
}