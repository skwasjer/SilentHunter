using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SilentHunter.Dat
{
	[XmlRoot("cache")]
	public class ControllerCache
	{
		[XmlAttribute("appver")]
		public string Version { get; set; }

		[XmlAttribute("build")]
		public string BuildConfiguration { get; set; }

		[XmlArray("dependencies")]
		[XmlArrayItem("dependency")]
		public HashSet<ControllerFileReference> Dependencies { get; set; }

		[XmlArray("files")]
		[XmlArrayItem("file")]
		public HashSet<ControllerFileReference> SourceFiles { get; set; }

		#region Equality members

		protected bool Equals(ControllerCache other)
		{
			return string.Equals(Version, other.Version) && string.Equals(BuildConfiguration, other.BuildConfiguration) 
				&& SetEquals(Dependencies, other.Dependencies) 
				&& SetEquals(SourceFiles, other.SourceFiles);
		}

		private bool SetEquals(HashSet<ControllerFileReference> self, HashSet<ControllerFileReference> other)
		{
			if (ReferenceEquals(null, self)) return false;
			if (ReferenceEquals(self, other)) return true;
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
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((ControllerCache)obj);
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
				var hashCode = (Version != null ? Version.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (BuildConfiguration != null ? BuildConfiguration.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Dependencies != null ? Dependencies.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (SourceFiles != null ? SourceFiles.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static bool operator ==(ControllerCache left, ControllerCache right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(ControllerCache left, ControllerCache right)
		{
			return !Equals(left, right);
		}

		#endregion
	}

	public class ControllerFileReference
		: IXmlSerializable
	{
		public string Name { get; set; }

		public DateTime? LastModified { get; set; }

		#region Implementation of IXmlSerializable

		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			reader.MoveToAttribute("name");
			Name = reader.Value;

			LastModified = null;
			if (reader.MoveToAttribute("dt"))
				LastModified = XmlConvert.ToDateTime(reader.Value, XmlDateTimeSerializationMode.Utc);

			reader.Read();
		}

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			writer.WriteAttributeString("name", Name);
			if (LastModified.HasValue)
				writer.WriteAttributeString("dt",
					XmlConvert.ToString(LastModified.Value, XmlDateTimeSerializationMode.Utc)
				);
		}

		#endregion

		#region Equality members

		protected bool Equals(ControllerFileReference other)
		{
			return string.Equals(Name, other.Name) && LastModified.Equals(other.LastModified);
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
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((ControllerFileReference)obj);
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
				return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ LastModified.GetHashCode();
			}
		}

		public static bool operator ==(ControllerFileReference left, ControllerFileReference right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(ControllerFileReference left, ControllerFileReference right)
		{
			return !Equals(left, right);
		}

		#endregion

		public override string ToString()
		{
			return Name;
		}
	}
}
