using System.Globalization;
using System.Runtime.InteropServices;

namespace SilentHunter
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct BoolVector3
	{
		[MarshalAs(UnmanagedType.U1)]
		public bool X;
		[MarshalAs(UnmanagedType.U1)]
		public bool Y;
		[MarshalAs(UnmanagedType.U1)]
		public bool Z;

		/// <inheritdoc />
		public override string ToString()
		{
			return string.Format("X: {1}{0} Y: {2}{0} Z: {3}", CultureInfo.CurrentUICulture.TextInfo.ListSeparator, X, Y, Z);
		}
	}
}