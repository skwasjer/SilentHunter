using System.IO;

namespace skwas.IO
{
	/// <summary>
	/// Describes a type that can be (de)serialized directly from a <see cref="Stream" />.
	/// </summary>
	public interface IRawSerializable
	{
		/// <summary>
		/// When implemented, deserializes the implemented class from specified <paramref name="stream" />.
		/// </summary>
		/// <param name="stream">The stream.</param>
		void Deserialize(Stream stream);

		/// <summary>
		/// When implemented, serializes the implemented class to specified <paramref name="stream" />.
		/// </summary>
		/// <param name="stream">The stream.</param>
		void Serialize(Stream stream);
	}
}