using System.IO;

namespace SilentHunter.FileFormats.Dat.Controllers
{
	/// <summary>
	/// Writes controllers to a stream.
	/// </summary>
	public interface IControllerWriter
	{
		/// <summary>
		/// Writes a controller to a stream.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="controller">The controller instance. If a byte array or stream is provided, it will be copied as is.</param>
		void Write(Stream stream, object controller);
	}
}