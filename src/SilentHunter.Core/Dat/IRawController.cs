using System.IO;

namespace SilentHunter.Dat
{
	/// <summary>
	/// Describes a controller that can be (de)serialized directly from a <see cref="Stream"/>.
	/// </summary>
	public interface IRawController		
	{
		/// <summary>
		/// When implemented, deserializes the controller from specified <paramref name="stream"/>.
		/// </summary>
		/// <param name="stream">The stream.</param>
		void Deserialize(Stream stream);

		/// <summary>
		/// When implemented, serializes the controller to specified <paramref name="stream"/>.
		/// </summary>
		/// <param name="stream">The stream.</param>
		void Serialize(Stream stream);
	}
}