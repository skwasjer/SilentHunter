using System.IO;

namespace SilentHunter.Dat.Controllers.Serialization
{
	public interface IControllerSerializer
	{
		void Deserialize(Stream stream, IRawController controller);

		void Serialize(Stream stream, IRawController controller);
	}
}
