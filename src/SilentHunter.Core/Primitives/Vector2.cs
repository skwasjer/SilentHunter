using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace SilentHunter
{
	/// <summary>
	/// Describes a vector in two-dimensional (2-D) space.
	/// </summary>
	/// <remarks>This type is only a simple stub for parsing DAT-files and thus is not ment to be used to manipulate vectors. Instead, implicit operators are implemented to cast to other Vector2 types.</remarks>
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct Vector2
	{
		public float X;
		public float Y;

		public static readonly Vector2 Empty;

		/// <summary>
		/// Returns the fully qualified type name of this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String" /> containing a fully qualified type name.
		/// </returns>
		public override string ToString()
		{
			return $"X: {X}{CultureInfo.CurrentCulture.TextInfo.ListSeparator} Y: {Y}";
		}

		public bool Equals(Vector2 other)
		{
			return X.Equals(other.X) && Y.Equals(other.Y);
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

			return obj is Vector2 && Equals((Vector2)obj);
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
				hashCode = (hashCode * 397) ^ Y.GetHashCode();
				return hashCode;
			}
		}

		public static bool operator ==(Vector2 left, Vector2 right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Vector2 left, Vector2 right)
		{
			return !left.Equals(right);
		}

		public static implicit operator SharpDX.Vector2(Vector2 vector)
		{
			return new SharpDX.Vector2
			{
				X = vector.X,
				Y = vector.Y
			};
		}

		public static implicit operator Vector2(SharpDX.Vector2 vector)
		{
			return new Vector2
			{
				X = vector.X,
				Y = vector.Y
			};
		}
	}
}