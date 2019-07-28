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

		public static Matrix RotationX(float angle)
		{
			Matrix matrix = new Matrix();
			float num1 = (float)Math.Cos((double)angle);
			float num2 = (float)Math.Sin((double)angle);
			matrix.M11 = 1f;
			matrix.M12 = 0.0f;
			matrix.M13 = 0.0f;
			matrix.M14 = 0.0f;
			matrix.M21 = 0.0f;
			matrix.M22 = num1;
			matrix.M23 = num2;
			matrix.M24 = 0.0f;
			matrix.M31 = 0.0f;
			matrix.M32 = -num2;
			matrix.M33 = num1;
			matrix.M34 = 0.0f;
			matrix.M41 = 0.0f;
			matrix.M42 = 0.0f;
			matrix.M43 = 0.0f;
			matrix.M44 = 1f;
			return matrix;
		}
		public static Matrix RotationY(float angle)
		{
			Matrix matrix = new Matrix();
			float num1 = (float)Math.Cos((double)angle);
			float num2 = (float)Math.Sin((double)angle);
			matrix.M11 = num1;
			matrix.M12 = 0.0f;
			matrix.M13 = -num2;
			matrix.M14 = 0.0f;
			matrix.M21 = 0.0f;
			matrix.M22 = 1f;
			matrix.M23 = 0.0f;
			matrix.M24 = 0.0f;
			matrix.M31 = num2;
			matrix.M32 = 0.0f;
			matrix.M33 = num1;
			matrix.M34 = 0.0f;
			matrix.M41 = 0.0f;
			matrix.M42 = 0.0f;
			matrix.M43 = 0.0f;
			matrix.M44 = 1f;
			return matrix;
		}

		public static Matrix RotationZ(float angle)
		{
			Matrix matrix = new Matrix();
			float num1 = (float)Math.Cos((double)angle);
			float num2 = (float)Math.Sin((double)angle);
			matrix.M11 = num1;
			matrix.M12 = num2;
			matrix.M13 = 0.0f;
			matrix.M14 = 0.0f;
			matrix.M21 = -num2;
			matrix.M22 = num1;
			matrix.M23 = 0.0f;
			matrix.M24 = 0.0f;
			matrix.M31 = 0.0f;
			matrix.M32 = 0.0f;
			matrix.M33 = 1f;
			matrix.M34 = 0.0f;
			matrix.M41 = 0.0f;
			matrix.M42 = 0.0f;
			matrix.M43 = 0.0f;
			matrix.M44 = 1f;
			return matrix;
		}

		public static Matrix Translation(Vector3 amount)
		{
			return new Matrix()
			{
				M11 = 1f,
				M12 = 0.0f,
				M13 = 0.0f,
				M14 = 0.0f,
				M21 = 0.0f,
				M22 = 1f,
				M23 = 0.0f,
				M24 = 0.0f,
				M31 = 0.0f,
				M32 = 0.0f,
				M33 = 1f,
				M34 = 0.0f,
				M41 = amount.X,
				M42 = amount.Y,
				M43 = amount.Z,
				M44 = 1f
			};
		}

		#region Equality members

		public bool Equals(Matrix other)
		{
			return M11.Equals(other.M11) && M12.Equals(other.M12) && M13.Equals(other.M13) && M14.Equals(other.M14) && M21.Equals(other.M21) && M22.Equals(other.M22) && M23.Equals(other.M23) && M24.Equals(other.M24) && M31.Equals(other.M31) && M32.Equals(other.M32) && M33.Equals(other.M33) && M34.Equals(other.M34) && M41.Equals(other.M41) && M42.Equals(other.M42) && M43.Equals(other.M43) && M44.Equals(other.M44);
		}

		/// <summary>
		/// Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <returns>
		/// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false. 
		/// </returns>
		/// <param name="obj">The object to compare with the current instance. </param>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
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
				var hashCode = M11.GetHashCode();
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

		#endregion

		public static Matrix operator *(Matrix left, Matrix right)
		{
			return new Matrix()
			{
				M11 = (float)((double)right.M21 * (double)left.M12 + (double)left.M11 * (double)right.M11 + (double)right.M31 * (double)left.M13 + (double)right.M41 * (double)left.M14),
				M12 = (float)((double)right.M22 * (double)left.M12 + (double)right.M12 * (double)left.M11 + (double)right.M32 * (double)left.M13 + (double)right.M42 * (double)left.M14),
				M13 = (float)((double)right.M23 * (double)left.M12 + (double)right.M13 * (double)left.M11 + (double)right.M33 * (double)left.M13 + (double)right.M43 * (double)left.M14),
				M14 = (float)((double)right.M24 * (double)left.M12 + (double)right.M14 * (double)left.M11 + (double)right.M34 * (double)left.M13 + (double)right.M44 * (double)left.M14),
				M21 = (float)((double)left.M22 * (double)right.M21 + (double)left.M21 * (double)right.M11 + (double)left.M23 * (double)right.M31 + (double)left.M24 * (double)right.M41),
				M22 = (float)((double)left.M22 * (double)right.M22 + (double)left.M21 * (double)right.M12 + (double)left.M23 * (double)right.M32 + (double)left.M24 * (double)right.M42),
				M23 = (float)((double)right.M23 * (double)left.M22 + (double)right.M13 * (double)left.M21 + (double)right.M33 * (double)left.M23 + (double)left.M24 * (double)right.M43),
				M24 = (float)((double)right.M24 * (double)left.M22 + (double)right.M14 * (double)left.M21 + (double)right.M34 * (double)left.M23 + (double)right.M44 * (double)left.M24),
				M31 = (float)((double)left.M32 * (double)right.M21 + (double)left.M31 * (double)right.M11 + (double)left.M33 * (double)right.M31 + (double)left.M34 * (double)right.M41),
				M32 = (float)((double)left.M32 * (double)right.M22 + (double)left.M31 * (double)right.M12 + (double)left.M33 * (double)right.M32 + (double)left.M34 * (double)right.M42),
				M33 = (float)((double)right.M23 * (double)left.M32 + (double)left.M31 * (double)right.M13 + (double)left.M33 * (double)right.M33 + (double)left.M34 * (double)right.M43),
				M34 = (float)((double)right.M24 * (double)left.M32 + (double)right.M14 * (double)left.M31 + (double)right.M34 * (double)left.M33 + (double)right.M44 * (double)left.M34),
				M41 = (float)((double)left.M42 * (double)right.M21 + (double)left.M41 * (double)right.M11 + (double)left.M43 * (double)right.M31 + (double)left.M44 * (double)right.M41),
				M42 = (float)((double)left.M42 * (double)right.M22 + (double)left.M41 * (double)right.M12 + (double)left.M43 * (double)right.M32 + (double)left.M44 * (double)right.M42),
				M43 = (float)((double)right.M23 * (double)left.M42 + (double)left.M41 * (double)right.M13 + (double)left.M43 * (double)right.M33 + (double)left.M44 * (double)right.M43),
				M44 = (float)((double)right.M24 * (double)left.M42 + (double)left.M41 * (double)right.M14 + (double)right.M34 * (double)left.M43 + (double)left.M44 * (double)right.M44)
			};
		}
	}
}
