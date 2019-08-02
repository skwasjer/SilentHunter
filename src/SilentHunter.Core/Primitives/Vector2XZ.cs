using System.Globalization;
using System.Runtime.InteropServices;

namespace SilentHunter
{
	[StructLayout(LayoutKind.Sequential)]
	// ReSharper disable once InconsistentNaming
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
			return string.Format("X: {1}{0} Z: {2}", CultureInfo.CurrentUICulture.TextInfo.ListSeparator, X, Z);
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

		public bool Equals(Vector2XZ other)
		{
			return X.Equals(other.X) && Z.Equals(other.Z);
		}

		/// <inheritdoc />
		public override bool Equals(object obj)
		{
			return obj is Vector2XZ vector && Equals(vector);
		}

		/// <inheritdoc />
		public override int GetHashCode()
		{
			unchecked
			{
				// ReSharper disable NonReadonlyMemberInGetHashCode
				int hashCode = X.GetHashCode();
				hashCode = (hashCode * 397) ^ Z.GetHashCode();
				// ReSharper restore NonReadonlyMemberInGetHashCode
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
	}
}