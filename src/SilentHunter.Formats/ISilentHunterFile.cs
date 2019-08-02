using System.IO;
using System.Threading.Tasks;
using skwas.IO;

namespace SilentHunter
{
	/// <summary>
	/// Base interface for Silent Hunter files.
	/// </summary>
	public interface ISilentHunterFile : IRawSerializable
	{
		/// <summary>
		/// Loads the file from specified stream.
		/// </summary>
		/// <param name="stream">The stream to load from.</param>
		Task LoadAsync(Stream stream);

		/// <summary>
		/// Saves the file to specified stream.
		/// </summary>
		/// <param name="stream">The stream to write to.</param>
		Task SaveAsync(Stream stream);
	}
}