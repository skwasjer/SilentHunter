using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace SilentHunter
{
	/// <summary>
	/// Describes a vector in three-dimensional (3-D) space.
	/// </summary>
	/// <remarks>This type is only a simple stub for parsing DAT-files and thus is not ment to be used to manipulate vectors. Instead, implicit operators are implemented to cast to other Vector3 types.</remarks>
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct Vector3
	{
		public float X;
		public float Y;
		public float Z;

		public static readonly Vector3 Empty;

		/// <inheritdoc />
		public override string ToString()
		{
			return string.Format("X: {1}{0} Y: {2}{0} Z: {3}", CultureInfo.CurrentUICulture.TextInfo.ListSeparator, X, Y, Z);
		}

		public bool Equals(Vector3 other)
		{
			return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
		}

		/// <inheritdoc />
		public override bool Equals(object obj)
		{
			return obj is Vector3 vector3 && Equals(vector3);
		}

		/// <inheritdoc />
		public override int GetHashCode()
		{
			unchecked
			{
				// ReSharper disable NonReadonlyMemberInGetHashCode
				int hashCode = X.GetHashCode();
				hashCode = (hashCode * 397) ^ Y.GetHashCode();
				hashCode = (hashCode * 397) ^ Z.GetHashCode();
				// ReSharper restore NonReadonlyMemberInGetHashCode
				return hashCode;
			}
		}

		public static bool operator ==(Vector3 left, Vector3 right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Vector3 left, Vector3 right)
		{
			return !left.Equals(right);
		}

		public static implicit operator SharpDX.Vector3(Vector3 vector)
		{
			return new SharpDX.Vector3
			{
				X = vector.X,
				Y = vector.Y,
				Z = vector.Z
			};
		}

		public static implicit operator Vector3(SharpDX.Vector3 vector)
		{
			return new Vector3
			{
				X = vector.X,
				Y = vector.Y,
				Z = vector.Z
			};
		}
	}
}