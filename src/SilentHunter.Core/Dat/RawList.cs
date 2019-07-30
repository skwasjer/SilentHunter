using System.Collections;
using System.Collections.Generic;

namespace SilentHunter.Dat
{
	/// <summary>
	/// Represents a list that is (de)serialized with a count field before it.
	/// </summary>
	public interface IRawList : IList
	{
	}

	/// <summary>
	/// Represents a list that is (de)serialized with a count field before it.
	/// </summary>
	public class RawList<T, TCountType>
		: List<T>, IRawList
	{
	}
}