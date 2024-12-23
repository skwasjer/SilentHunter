﻿using System.IO;

namespace SilentHunter.FileFormats.Dat.Controllers;

/// <summary>
/// Reads controllers from a stream.
/// </summary>
public interface IControllerReader
{
    /// <summary>
    /// Reads a controller from a stream. If the controller is not implemented, the raw data is returned as byte array.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="controllerName">The name of the controller or null if the controller must be auto-detected (not optimal, some controllers cannot be detected).</param>
    /// <returns></returns>
    public object Read(Stream stream, string controllerName);
}
