using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace SilentHunter
{
	[Serializable]
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct Color : IEquatable<Color>
	{
		[FieldOffset(0)]
		public byte A;
		[FieldOffset(3)]
		public byte R;
		[FieldOffset(2)]
		public byte G;
		[FieldOffset(1)]
		public byte B;

		public string ToString(bool ignoreAlpha)
		{
			string newValue;
			return A < 255 && !ignoreAlpha
				? string.Format("A: {1}{0} R: {2}{0} G: {3}{0} B: {4}", CultureInfo.CurrentUICulture.TextInfo.ListSeparator, A, R, G, B)
				: string.Format("R: {1}{0} G: {2}{0} B: {3}", CultureInfo.CurrentUICulture.TextInfo.ListSeparator, R, G, B);

			return newValue;
		}

		public override string ToString()
		{
			return ToString(true);
		}

		public static Color FromArgb(int alpha, Color color)
		{
			color.A = (byte)(alpha & byte.MaxValue);
			return color;
		}

		public static Color FromArgb(int alpha, int red, int green, int blue)
		{
			return new Color
			{
				A = (byte)(alpha & byte.MaxValue),
				R = (byte)(red & byte.MaxValue),
				G = (byte)(green & byte.MaxValue),
				B = (byte)(blue & byte.MaxValue)
			};
		}

		public static Color FromArgb(int red, int green, int blue)
		{
			return FromArgb(byte.MaxValue, red, green, blue);
		}

		public static Color FromArgb(int argb)
		{
			return new Color
			{
				A = (byte)(((long)argb >> 24) & byte.MaxValue),
				R = (byte)((argb >> 16) & byte.MaxValue),
				G = (byte)((argb >> 8) & byte.MaxValue),
				B = (byte)(argb & byte.MaxValue)
			};
		}

		public int ToArgb()
		{
			return (int)((long)A << 24 | (long)R << 16 | (long)G << 8 | B);
		}

		public bool Equals(Color other)
		{
			return A == other.A && R == other.R && G == other.G && B == other.B;
		}

		public override bool Equals(object obj)
		{
			return obj is Color other && Equals(other);
		}

		public override int GetHashCode()
		{
			return (A << 24) | (R << 16) | (G << 8) | B;
		}

		public static bool operator ==(Color left, Color right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Color left, Color right)
		{
			return !left.Equals(right);
		}
	}
}