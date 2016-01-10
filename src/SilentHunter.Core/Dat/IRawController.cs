using System.IO;
using skwas.IO;

namespace SilentHunter.Dat
{
	/// <summary>
	/// Describes a controller that can be (de)serialized directly from a <see cref="Stream"/>.
	/// </summary>
	public interface IRawController		
		: IRawSerializable
	{
	}
}