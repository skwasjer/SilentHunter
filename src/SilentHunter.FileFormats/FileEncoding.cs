namespace SilentHunter.FileFormats
{
	public static class FileEncoding
	{
		/// <summary>
		/// The default encoding to use for parsing Silent Hunter game files.
		/// </summary>
		public static System.Text.Encoding Default { get; } = System.Text.Encoding.GetEncoding("ISO-8859-1");
	}
}