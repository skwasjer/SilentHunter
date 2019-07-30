using System.Globalization;
using System.Runtime.InteropServices;

namespace SilentHunter
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Vector2XZ
	{
		public float X;
		public float Z;

		/// <summary>
		/// Returns the fully qualified type name of this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String" /> containing a fully qualified type name.
		/// </returns>
		public override string ToString()
		{
			return $"X: {X}{CultureInfo.CurrentCulture.TextInfo.ListSeparator} Z: {Z}";
		}

		public static implicit operator Vector2(Vector2XZ v)
		{
			return new Vector2
			{
				X = v.X,
				Y = v.Z
			};
		}

		public static implicit operator Vector2XZ(Vector2 v)
		{
			return new Vector2XZ
			{
				X = v.X,
				Z = v.Y
			};
		}

		#region Equality members

		public bool Equals(Vector2XZ other)
		{
			return X.Equals(other.X) && Z.Equals(other.Z);
		}

		/// <summary>
		/// Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <returns>
		/// true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false.
		/// </returns>
		/// <param name="obj">The object to compare with the current instance. </param>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}

			return obj is Vector2XZ && Equals((Vector2XZ)obj);
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>
		/// A 32-bit signed integer that is the hash code for this instance.
		/// </returns>
		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = X.GetHashCode();
				hashCode = (hashCode * 397) ^ Z.GetHashCode();
				return hashCode;
			}
		}

		public static bool operator ==(Vector2XZ left, Vector2XZ right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Vector2XZ left, Vector2XZ right)
		{
			return !left.Equals(right);
		}

		#endregion
	}
}