namespace SilentHunter.FileFormats.Dat.Controllers;

/// <summary>
/// Controllers are grouped in a profile by Silent Hunter game version.
/// </summary>
/// <remarks>
/// Originally, it was on purposely ordered latest to oldest version, due to some logic in <see cref="ControllerFactory" /> and <see cref="ControllerReader" />.
/// </remarks>
public enum ControllerProfile
{
    /// <summary>
    /// </summary>
    Unknown,

    /// <summary>
    /// Controller profile Silent Hunter 5.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    SH5,

    /// <summary>
    /// Controller profile Silent Hunter 4.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    SH4,

    /// <summary>
    /// Controller profile Silent Hunter 3.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    SH3
}
