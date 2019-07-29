using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SilentHunter.Dat
{
	public class CacheFileReference
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
			{
				LastModified = XmlConvert.ToDateTime(reader.Value, XmlDateTimeSerializationMode.Utc);
			}

			reader.Read();
		}

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			writer.WriteAttributeString("name", Name);
			if (LastModified.HasValue)
			{
				writer.WriteAttributeString("dt",
					XmlConvert.ToString(LastModified.Value, XmlDateTimeSerializationMode.Utc)
				);
			}
		}

		#endregion

		#region Equality members

		protected bool Equals(CacheFileReference other)
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

			return Equals((CacheFileReference)obj);
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

		public static bool operator ==(CacheFileReference left, CacheFileReference right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(CacheFileReference left, CacheFileReference right)
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