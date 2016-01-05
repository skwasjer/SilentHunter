using System.IO;

namespace SilentHunter.Dat
{
	/// <summary>
	/// Describes a controller that can be (de)serialized directly from a <see cref="Stream"/>.
	/// </summary>
	public interface IController
		: IRawController
	{
	}
}
