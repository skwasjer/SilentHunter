using System.IO;
using SilentHunter.Controllers;

namespace SilentHunter.FileFormats.Dat.Controllers.Serialization
{
	public interface IControllerSerializer
	{
		void Deserialize(Stream stream, Controller controller);

		void Serialize(Stream stream, Controller controller);
	}
}
