using System.ComponentModel;
using System.Diagnostics;

namespace SilentHunter.Controllers;

/// <summary>
/// An action to perform after a condition is met.
/// </summary>
[DebuggerDisplay("ParentEntryIndex={ParentEntryIndex}, ParentConditionIndex={ParentConditionIndex}, Name={Name}, Value={Value}")]
public class StateMachineAction
{
    /// <summary>
    /// Keep internal, not for UI use. Is recalculated during serialization.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public int ParentEntryIndex;

    /// <summary>
    /// Keep internal, not for UI use. Is recalculated during serialization.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public int ParentConditionIndex;

    /// <summary>
    /// The name or command of the action.
    /// </summary>
    public string Name;

    /// <summary>
    /// The value or parameters to perform the action with.
    /// </summary>
    public string Value;

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