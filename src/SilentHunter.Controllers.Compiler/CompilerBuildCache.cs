using System.Collections.Generic;
using System.Xml.Serialization;

namespace SilentHunter.Controllers.Compiler
{
	[XmlRoot("cache")]
	public class CompilerBuildCache
	{
		[XmlAttribute("appver")]
		public string Version { get; set; }

		[XmlAttribute("build")]
		public string BuildConfiguration { get; set; }

		[XmlArray("dependencies")]
		[XmlArrayItem("dependency")]
		public HashSet<CacheFileReference> Dependencies { get; set; }

		[XmlArray("files")]
		[XmlArrayItem("file")]
		public HashSet<CacheFileReference> SourceFiles { get; set; }

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

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <returns>
		/// true if the specified object  is equal to the current object; otherwise, false.
		/// </returns>
		/// <param name="obj">The object to compare with the current object. </param>
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

		/// <summary>
		/// Serves as the default hash function.
		/// </summary>
		/// <returns>
		/// A hash code for the current object.
		/// </returns>
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

		public static bool operator ==(CompilerBuildCache left, CompilerBuildCache right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(CompilerBuildCache left, CompilerBuildCache right)
		{
			return !Equals(left, right);
		}
	}
}