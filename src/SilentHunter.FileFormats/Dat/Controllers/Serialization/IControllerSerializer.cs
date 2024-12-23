using System.IO;
using SilentHunter.Controllers;

namespace SilentHunter.FileFormats.Dat.Controllers.Serialization;

/// <summary>
/// Describes a (de)serializer for controllers.
/// </summary>
public interface IControllerSerializer
{
    /// <summary>
    /// Deserializes specified <paramref name="controller"/> from the <paramref name="stream"/>.
    /// </summary>
    /// <param name="stream">The input stream.</param>
    /// <param name="controller">The controller to deserialize into.</param>
    void Deserialize(Stream stream, Controller controller);

    /// <summary>
    /// Serializes specified <paramref name="controller"/> to the <paramref name="stream"/>.
    /// </summary>
    /// <param name="stream">The output stream.</param>
    /// <param name="controller">The controller to serialize.</param>
    void Serialize(Stream stream, Controller controller);
}