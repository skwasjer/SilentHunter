﻿namespace SilentHunter.Controllers;

/// <summary>
/// Represents a key frame for key frame animations.
/// </summary>
public class AnimationKeyFrame
{
    /// <summary>
    /// The time of the key frame.
    /// </summary>
    public float Time;

    /// <summary>
    /// The index of the set of compressed vertices to use.
    /// </summary>
    public ushort FrameNumber;
}