using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace SilentHunter
{
	/// <summary>
	/// Describes a quaternion.
	/// </summary>
	/// <remarks>This type is only a simple stub for parsing DAT-files and thus is not ment to be used to work with quaternion. Instead, implicit operators are implemented to cast to other Quaternion types.</remarks>
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct Quaternion
	{
		public float X;
		public float Y;
		public float Z;
		public float W;

		public static readonly Quaternion Identity = new Quaternion
		{
			X = 0.0f,
			Y = 0.0f,
			Z = 0.0f,
			W = 1f
		};

		/// <summary>
		/// Returns the fully qualified type name of this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String" /> containing a fully qualified type name.
		/// </returns>
		public override string ToString()
		{
			string sep = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
			return $"X: {X}{sep} Y: {Y}{sep} Z: {Z}{sep} W: {W}";
		}

		public bool Equals(Quaternion other)
		{
			return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z) && W.Equals(other.W);
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

			return obj is Quaternion && Equals((Quaternion)obj);
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
				hashCode = (hashCode * 397) ^ Z.GetHashCode();
				hashCode = (hashCode * 397) ^ W.GetHashCode();
				return hashCode;
			}
		}

		public static bool operator ==(Quaternion left, Quaternion right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Quaternion left, Quaternion right)
		{
			return !left.Equals(right);
		}

		public static implicit operator SharpDX.Quaternion(Quaternion quad)
		{
			return new SharpDX.Quaternion
			{
				X = quad.X,
				Y = quad.Y,
				Z = quad.Z,
				W = quad.W
			};
		}

		public static implicit operator Quaternion(SharpDX.Quaternion quad)
		{
			return new Quaternion
			{
				X = quad.X,
				Y = quad.Y,
				Z = quad.Z,
				W = quad.W
			};
		}
	}
}