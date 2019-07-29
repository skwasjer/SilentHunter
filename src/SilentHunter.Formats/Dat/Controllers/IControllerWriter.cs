using System.IO;

namespace SilentHunter.Dat.Controllers
{
	public interface IControllerWriter
	{
		/// <summary>
		/// Writes a controller to a stream.
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="controller"></param>
		void Write(Stream stream, object controller);
	}
}