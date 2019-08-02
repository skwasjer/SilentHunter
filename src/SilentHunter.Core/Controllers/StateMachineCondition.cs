using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using SilentHunter.Controllers.Decoration;

namespace SilentHunter.Controllers
{
	/// <summary>
	/// A condition that is checked in a gamestate update.
	/// </summary>
	[DebuggerDisplay("ConditionIndex={ParentEntryIndex}, GotoEntry={GotoEntry}, Type={Type}, Name={Expression}, Value={Value}")]
	public class StateMachineCondition
	{
		/// <summary>
		/// Keep internal, not for UI use. Is recalculated during serialization.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int ParentEntryIndex;

		/// <summary>
		/// The condition check type to perform.
		/// </summary>
		public int Type;

		/// <summary>
		/// The expression to evaluate.
		/// </summary>
		[AutoCompleteList(@"Cfg\StateMachineClass_Conditions.txt")]
		public string Expression;

		/// <summary>
		/// The value that the expression must evaluate to, to be true. Can be empty if the condition is a boolean evaluation, ie. 'true', 'OnCollision'.
		/// </summary>
		[AutoCompleteList(@"Cfg\Commands.txt")]
		public string Value;

		/// <summary>
		/// The state entry to jump to when this condition is met. The jump is performed in the next gamestate update.
		/// </summary>
		public int GotoEntry;

		/// <summary>
		/// A list of actions to perform when the condition is met.
		/// </summary>
		public List<StateMachineAction> Actions = new List<StateMachineAction>();

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>
		/// A string that represents the current object.
		/// </returns>
		public override string ToString()
		{
			return string.IsNullOrEmpty(Expression) ? "<empty>" : Expression;
		}
	}
}