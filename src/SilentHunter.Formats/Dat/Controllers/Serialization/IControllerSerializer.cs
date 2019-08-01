using System.IO;
using SilentHunter.Controllers;

namespace SilentHunter.Dat.Controllers.Serialization
{
	public interface IControllerSerializer
	{
		void Deserialize(Stream stream, RawController controller);

		void Serialize(Stream stream, RawController controller);
	}
}
