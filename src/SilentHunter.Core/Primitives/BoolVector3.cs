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
	}
}