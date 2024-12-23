using System;

namespace SilentHunter.Controllers.Decoration;

/// <summary>
/// Special attribute used in conjunction with animation controllers, to indicate the animation type.
/// </summary>
// TODO: Should probably obsolete this for something better.
[AttributeUsage(AttributeTargets.Class)]
public sealed class ControllerAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ControllerAttribute" /> class.
    /// </summary>
    public ControllerAttribute()
    {
    }

    /// <summary>
    /// Decorator for animation controllers, indicating the subType.
    /// </summary>
    /// <param name="subType"></param>
    public ControllerAttribute(ushort subType)
    {
        SubType = subType;
    }

    /// <summary>
    /// The sub/animation type.
    /// </summary>
    public ushort? SubType { get; }
}
