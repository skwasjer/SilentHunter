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


		/// <summary>
		/// A <see cref="Matrix"/> with all of its components set to zero.
		/// </summary>
		public static readonly Matrix Zero = new Matrix();

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

		public void Invert()
		{
			Invert(ref this, out this);
		}

		/// <summary>
		/// Calculates the inverse of the specified matrix.
		/// </summary>
		/// <param name="value">The matrix whose inverse is to be calculated.</param>
		/// <param name="result">When the method completes, contains the inverse of the specified matrix.</param>
		public static void Invert(ref Matrix value, out Matrix result)
		{
			float b0 = (value.M31 * value.M42) - (value.M32 * value.M41);
			float b1 = (value.M31 * value.M43) - (value.M33 * value.M41);
			float b2 = (value.M34 * value.M41) - (value.M31 * value.M44);
			float b3 = (value.M32 * value.M43) - (value.M33 * value.M42);
			float b4 = (value.M34 * value.M42) - (value.M32 * value.M44);
			float b5 = (value.M33 * value.M44) - (value.M34 * value.M43);

			float d11 = value.M22 * b5 + value.M23 * b4 + value.M24 * b3;
			float d12 = value.M21 * b5 + value.M23 * b2 + value.M24 * b1;
			float d13 = value.M21 * -b4 + value.M22 * b2 + value.M24 * b0;
			float d14 = value.M21 * b3 + value.M22 * -b1 + value.M23 * b0;

			float det = value.M11 * d11 - value.M12 * d12 + value.M13 * d13 - value.M14 * d14;
			if (Math.Abs(det) == 0.0f)
			{
				result = Matrix.Zero;
				return;
			}

			det = 1f / det;

			float a0 = (value.M11 * value.M22) - (value.M12 * value.M21);
			float a1 = (value.M11 * value.M23) - (value.M13 * value.M21);
			float a2 = (value.M14 * value.M21) - (value.M11 * value.M24);
			float a3 = (value.M12 * value.M23) - (value.M13 * value.M22);
			float a4 = (value.M14 * value.M22) - (value.M12 * value.M24);
			float a5 = (value.M13 * value.M24) - (value.M14 * value.M23);

			float d21 = value.M12 * b5 + value.M13 * b4 + value.M14 * b3;
			float d22 = value.M11 * b5 + value.M13 * b2 + value.M14 * b1;
			float d23 = value.M11 * -b4 + value.M12 * b2 + value.M14 * b0;
			float d24 = value.M11 * b3 + value.M12 * -b1 + value.M13 * b0;

			float d31 = value.M42 * a5 + value.M43 * a4 + value.M44 * a3;
			float d32 = value.M41 * a5 + value.M43 * a2 + value.M44 * a1;
			float d33 = value.M41 * -a4 + value.M42 * a2 + value.M44 * a0;
			float d34 = value.M41 * a3 + value.M42 * -a1 + value.M43 * a0;

			float d41 = value.M32 * a5 + value.M33 * a4 + value.M34 * a3;
			float d42 = value.M31 * a5 + value.M33 * a2 + value.M34 * a1;
			float d43 = value.M31 * -a4 + value.M32 * a2 + value.M34 * a0;
			float d44 = value.M31 * a3 + value.M32 * -a1 + value.M33 * a0;

			result.M11 = +d11 * det; result.M12 = -d21 * det; result.M13 = +d31 * det; result.M14 = -d41 * det;
			result.M21 = -d12 * det; result.M22 = +d22 * det; result.M23 = -d32 * det; result.M24 = +d42 * det;
			result.M31 = +d13 * det; result.M32 = -d23 * det; result.M33 = +d33 * det; result.M34 = -d43 * det;
			result.M41 = -d14 * det; result.M42 = +d24 * det; result.M43 = -d34 * det; result.M44 = +d44 * det;
		}
	}
}
