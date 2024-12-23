using System.Collections.Generic;
using System.ComponentModel;

namespace SilentHunter.Controllers;

/// <summary>
/// Represents the state machine controller.
/// </summary>
public abstract class StateMachineController : Controller
{
    // NOTE: some unknown fields, but they always seem to be the same. So mark them advanced, so they don't show up in simple editor views.

    /// <summary>
    /// Unknown field, but always seems to be the same.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public int Unknown0 = 0x423B410F;

    /// <summary>
    /// The name that this StateMachineClass controller can be referenced by.
    /// </summary>
    public string GraphName;

    /// <summary>
    /// Unknown field, but always seems to be the same.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public int Unknown1 = 0x73A2500A;

    /// <summary>
    /// Unknown field, but always seems to be the same.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public int Unknown2 = 0x24CE7F70;

    /// <summary>
    /// Unknown field, but always seems to be the same.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public int Unknown3 = 0;

    /// <summary>
    /// A list of state entries that make up the state (behavior) of a character or object.
    /// </summary>
    public List<StateMachineEntry> StateEntries;
}