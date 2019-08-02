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

		/// <inheritdoc />
		public override string ToString()
		{
			return string.Format("X: {1}{0} Y: {2}{0} Z: {3}{0} Z: {4}", CultureInfo.CurrentUICulture.TextInfo.ListSeparator, X, Y, Z, W);
		}

		public bool Equals(Quaternion other)
		{
			return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z) && W.Equals(other.W);
		}

		/// <inheritdoc />
		public override bool Equals(object obj)
		{
			return obj is Quaternion quaternion && Equals(quaternion);
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
				hashCode = (hashCode * 397) ^ W.GetHashCode();
				// ReSharper restore NonReadonlyMemberInGetHashCode
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