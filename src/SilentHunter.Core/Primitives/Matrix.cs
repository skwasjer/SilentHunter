using System;
using System.Runtime.InteropServices;

namespace SilentHunter
{
	/// <summary>
	/// Describes a matrix.
	/// </summary>
	/// <remarks>This type is only a simple stub for parsing DAT-files and thus is not ment to be used to work with matrices. Instead, implicit operators are implemented to cast to other Matrix types.</remarks>
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct Matrix
	{
		public float M11;
		public float M12;
		public float M13;
		public float M14;
		public float M21;
		public float M22;
		public float M23;
		public float M24;
		public float M31;
		public float M32;
		public float M33;
		public float M34;
		public float M41;
		public float M42;
		public float M43;
		public float M44;

		public static Matrix Identity =>
			new Matrix
			{
				M11 = 1f,
				M22 = 1f,
				M33 = 1f,
				M44 = 1f
			};

		public bool Equals(Matrix other)
		{
			return M11.Equals(other.M11) && M12.Equals(other.M12) && M13.Equals(other.M13) && M14.Equals(other.M14) && M21.Equals(other.M21) && M22.Equals(other.M22) && M23.Equals(other.M23) && M24.Equals(other.M24) && M31.Equals(other.M31) && M32.Equals(other.M32) && M33.Equals(other.M33) && M34.Equals(other.M34) && M41.Equals(other.M41) && M42.Equals(other.M42) && M43.Equals(other.M43) && M44.Equals(other.M44);
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

			return obj is Matrix && Equals((Matrix)obj);
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
				int hashCode = M11.GetHashCode();
				hashCode = (hashCode * 397) ^ M12.GetHashCode();
				hashCode = (hashCode * 397) ^ M13.GetHashCode();
				hashCode = (hashCode * 397) ^ M14.GetHashCode();
				hashCode = (hashCode * 397) ^ M21.GetHashCode();
				hashCode = (hashCode * 397) ^ M22.GetHashCode();
				hashCode = (hashCode * 397) ^ M23.GetHashCode();
				hashCode = (hashCode * 397) ^ M24.GetHashCode();
				hashCode = (hashCode * 397) ^ M31.GetHashCode();
				hashCode = (hashCode * 397) ^ M32.GetHashCode();
				hashCode = (hashCode * 397) ^ M33.GetHashCode();
				hashCode = (hashCode * 397) ^ M34.GetHashCode();
				hashCode = (hashCode * 397) ^ M41.GetHashCode();
				hashCode = (hashCode * 397) ^ M42.GetHashCode();
				hashCode = (hashCode * 397) ^ M43.GetHashCode();
				hashCode = (hashCode * 397) ^ M44.GetHashCode();
				return hashCode;
			}
		}

		public static bool operator ==(Matrix left, Matrix right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Matrix left, Matrix right)
		{
			return !left.Equals(right);
		}

		public static implicit operator SharpDX.Matrix(Matrix matrix)
		{
			return new SharpDX.Matrix
			{
				M11 = matrix.M11,
				M12 = matrix.M12,
				M13 = matrix.M13,
				M14 = matrix.M14,
				M21 = matrix.M21,
				M22 = matrix.M22,
				M23 = matrix.M23,
				M24 = matrix.M24,
				M31 = matrix.M31,
				M32 = matrix.M32,
				M33 = matrix.M33,
				M34 = matrix.M34,
				M41 = matrix.M41,
				M42 = matrix.M42,
				M43 = matrix.M43,
				M44 = matrix.M44
			};
		}

		public static implicit operator Matrix(SharpDX.Matrix matrix)
		{
			return new Matrix
			{
				M11 = matrix.M11,
				M12 = matrix.M12,
				M13 = matrix.M13,
				M14 = matrix.M14,
				M21 = matrix.M21,
				M22 = matrix.M22,
				M23 = matrix.M23,
				M24 = matrix.M24,
				M31 = matrix.M31,
				M32 = matrix.M32,
				M33 = matrix.M33,
				M34 = matrix.M34,
				M41 = matrix.M41,
				M42 = matrix.M42,
				M43 = matrix.M43,
				M44 = matrix.M44
			};
		}
	}
}