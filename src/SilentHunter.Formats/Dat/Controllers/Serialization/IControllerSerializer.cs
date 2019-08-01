using System.IO;

namespace SilentHunter.Dat.Controllers.Serialization
{
	public interface IControllerSerializer
	{
		void Deserialize(Stream stream, RawController controller);

		void Serialize(Stream stream, RawController controller);
	}
}
