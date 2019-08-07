using System.Collections.Generic;
using System.Xml.Serialization;

namespace SilentHunter.Controllers.Compiler.BuildCache
{
	// TODO: switch to DataContractSerializer so this does not have to be public.

	/// <summary>
	/// The compiler build cache file format.
	/// </summary>
	[XmlRoot("cache")]
	internal class CompilerBuildCache
	{
		/// <summary>
		/// Gets or sets the compiler version.
		/// </summary>
		[XmlAttribute("appver")]
		public string Version { get; set; }

		/// <summary>
		/// Gets or sets the last used build configuration.
		/// </summary>
		[XmlAttribute("build")]
		public string BuildConfiguration { get; set; }

		/// <summary>
		/// Gets or sets the dependencies.
		/// </summary>
		[XmlArray("dependencies")]
		[XmlArrayItem("dependency")]
		public HashSet<CacheFileReference> Dependencies { get; set; }

		/// <summary>
		/// Gets or sets the source files.
		/// </summary>
		[XmlArray("files")]
		[XmlArrayItem("file")]
		public HashSet<CacheFileReference> SourceFiles { get; set; }

		/// <summary>
		/// </summary>
		protected bool Equals(CompilerBuildCache other)
		{
			return string.Equals(Version, other.Version) && string.Equals(BuildConfiguration, other.BuildConfiguration)
			 && SetEquals(Dependencies, other.Dependencies)
			 && SetEquals(SourceFiles, other.SourceFiles);
		}

		private bool SetEquals(HashSet<CacheFileReference> self, HashSet<CacheFileReference> other)
		{
			if (ReferenceEquals(null, self))
			{
				return false;
			}

			if (ReferenceEquals(self, other))
			{
				return true;
			}

			return self.SetEquals(other);
		}

		/// <inheritdoc />
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}

			if (ReferenceEquals(this, obj))
			{
				return true;
			}

			if (obj.GetType() != GetType())
			{
				return false;
			}

			return Equals((CompilerBuildCache)obj);
		}

		/// <inheritdoc />
		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = Version != null ? Version.GetHashCode() : 0;
				hashCode = (hashCode * 397) ^ (BuildConfiguration != null ? BuildConfiguration.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Dependencies != null ? Dependencies.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (SourceFiles != null ? SourceFiles.GetHashCode() : 0);
				return hashCode;
			}
		}

		/// <summary>
		/// </summary>
		public static bool operator ==(CompilerBuildCache left, CompilerBuildCache right)
		{
			return Equals(left, right);
		}

		/// <summary>
		/// </summary>
		public static bool operator !=(CompilerBuildCache left, CompilerBuildCache right)
		{
			return !Equals(left, right);
		}
	}
}