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

		/// <inheritdoc />
		public override string ToString()
		{
			return string.Format("X: {1}{0} Y: {2}", CultureInfo.CurrentUICulture.TextInfo.ListSeparator, X, Y);
		}

		public bool Equals(Vector2 other)
		{
			return X.Equals(other.X) && Y.Equals(other.Y);
		}

		/// <inheritdoc />
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}

			return obj is Vector2 vector2 && Equals(vector2);
		}

		/// <inheritdoc />
		public override int GetHashCode()
		{
			unchecked
			{
				// ReSharper disable NonReadonlyMemberInGetHashCode
				int hashCode = X.GetHashCode();
				hashCode = (hashCode * 397) ^ Y.GetHashCode();
				// ReSharper restore NonReadonlyMemberInGetHashCode
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