using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace SilentHunter.Dat
{
	public class ControllerAssemblyCompiler
	{
		private string _controllerPath;
		private string _applicationName;
		private string _assemblyName;

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

		public Assembly Compile()
		{
			AppDomain tempDomain = CreateTempAppDomain();
			string outputPath = tempDomain.DynamicDirectory;

			if (!Directory.Exists(outputPath))
			{
				Directory.CreateDirectory(outputPath);
			}

			// Copy our two dependencies.
			CopyDependencyIfModified("skwas.IO.dll", outputPath);
			CopyDependencyIfModified("SilentHunter.Core.dll", outputPath);

			string asmShortName = _assemblyName ?? Path.GetFileNameWithoutExtension(_controllerPath);
			string asmOutputFile = Path.Combine(outputPath, asmShortName + ".dll");
			string docFile = Path.Combine(outputPath, asmShortName + ".xml");

			// Generate cache file for the specified controller path, which is used to determine if
			// we have to rebuild the controller assembly. If one of the files has changed since last build
			// or a file is missing/added, the assembly will be rebuilt.
			var newCache = new ControllerCache
			{
#if DEBUG
				BuildConfiguration = "debug",
#else
				BuildConfiguration = "release",
#endif
				Version = FileVersionInfo.GetVersionInfo(typeof(ControllerAssemblyCompiler).Assembly.Location).FileVersion,
				Dependencies = new HashSet<ControllerFileReference>
				{
					new ControllerFileReference { Name = "System.dll" },
					new ControllerFileReference { Name = "System.Windows.Forms.dll" },
					new ControllerFileReference { Name = "System.Drawing.dll" },
					new ControllerFileReference {
						Name = "skwas.IO.dll",
						LastModified = File.GetLastWriteTimeUtc(Path.Combine(outputPath, "skwas.IO.dll"))
					},
					new ControllerFileReference {
						Name = "SilentHunter.Core.dll",
						LastModified = File.GetLastWriteTimeUtc(Path.Combine(outputPath, "SilentHunter.Core.dll"))
					}
				},
				SourceFiles = new HashSet<ControllerFileReference>(GetCSharpFiles(_controllerPath))
			};

			Compile(newCache, _controllerPath, asmOutputFile, docFile);

			// Load controller assembly also into local domain. You normally wouldn't want this, because you A) couldn't unload it and B) would create security issues. May have to look at using isolated domain.
			AssemblyName asmName = System.Reflection.AssemblyName.GetAssemblyName(asmOutputFile);
			return AppDomain.CurrentDomain.Load(asmName);
		}

		private AppDomain CreateTempAppDomain()
		{
			// Prepare to create a new application domain.
			AppDomainSetup setup = new AppDomainSetup();

			// Set the application name before setting the dynamic base.
			setup.ApplicationName = _applicationName;

			// Set the location of the base directory where assembly resolution 
			// probes for dynamic assemblies. Note that the hash code of the 
			// application name is concatenated to the base directory name you 
			// supply. 
			setup.DynamicBase = Path.Combine(Path.GetTempPath(), "S3D", "dynamic", _applicationName);
			Console.WriteLine("DynamicBase is set to '{0}'.", setup.DynamicBase);

			AppDomain tempDomain = AppDomain.CreateDomain(Guid.NewGuid().ToString(), null, setup);
			return tempDomain;
		}


		/// <summary>
		/// Gets *.cs files in specified folder and sorts them where Silent Hunter specific versions come first.
		/// </summary>
		/// <param name="path">The path where the controllers are located.</param>
		/// <returns>A list of controller files.</returns>
		private static IEnumerable<ControllerFileReference> GetCSharpFiles(string path)
		{
			return new DirectoryInfo(path)
				.GetFiles("*.cs", SearchOption.AllDirectories)
				.Select(f =>
					new ControllerFileReference
					{
						// Save relative name.
						Name = f.FullName.Substring(path.Length + 1),
						LastModified = f.LastWriteTimeUtc
					}
				);
		}

		private static void Compile(ControllerCache assemblyCache, string controllerPath, string outputFile, string docFile)
		{
			string cacheFile = outputFile + ".cache";

			var mustCompile = true;
			var serializer = new XmlSerializer(typeof(ControllerCache));

			// Load the cache file.
			if (File.Exists(cacheFile))
			{
				ControllerCache oldCache;
				using (var fs = File.OpenRead(cacheFile))
				{
					oldCache = (ControllerCache)serializer.Deserialize(fs);
				}

				// Check if cache is out of sync and that all files exist.
				mustCompile = !File.Exists(outputFile)
					|| !oldCache.Equals(assemblyCache)
					|| oldCache.SourceFiles.Any(src => !File.Exists(Path.Combine(controllerPath, src.Name)));
			}

			// If no changes in source files, return.
			if (!mustCompile) return;

			// NOTE: Ensure LoaderLock Managed Debugging Assistant in Exception settings is disabled, to allow VS to run dynamic compilation within IDE.

			// Compile the source files, etc.
			using (var compiler = new ControllerCompiler()
			{
				OutputPath = outputFile,
				DocFile = docFile,
				Dependencies = assemblyCache.Dependencies
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

			string srcPath = Path.GetFileName(sourceFile);
			string destPath = Path.Combine(destDirectory, sourceFile);
			if (!File.Exists(destPath) || File.GetLastWriteTimeUtc(destPath) != File.GetLastWriteTimeUtc(srcPath))
			{
				File.Copy(srcPath, destPath, true);
			}
		}
	}
}
