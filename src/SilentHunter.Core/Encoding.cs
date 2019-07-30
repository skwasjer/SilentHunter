namespace SilentHunter
{
	public static class Encoding
	{
		/// <summary>
		/// The default encoding to use for parsing Silent Hunter game files.
		/// </summary>
		public static System.Text.Encoding ParseEncoding { get; } = System.Text.Encoding.GetEncoding("ISO-8859-1");
	}
}