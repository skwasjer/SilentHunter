using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace SilentHunter.Dat
{
	/// <summary>
	/// A state entry containing a list of conditions to check during each gamestate update.
	/// </summary>
	[DebuggerDisplay("{Name}")]
	public class StateMachineEntry
	{
		/// <summary>
		/// Keep internal, not for UI use.
		/// </summary>
		internal int Index;

		/// <summary>
		/// The name of the state loop.
		/// </summary>
		public string Name;

		/// <summary>
		/// The list of conditions to check in sequential order in a single gamestate update. If a condition is met, all other condition checks are skipped.
		/// </summary>
		public List<StateMachineCondition> Conditions = new List<StateMachineCondition>();

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>
		/// A string that represents the current object.
		/// </returns>
		public override string ToString()
		{
			return string.IsNullOrEmpty(Name) ? "<empty>" : Name;
		}
	}
}