using System;
using System.IO;
using System.Threading.Tasks;

namespace SilentHunter.FileFormats;

/// <summary>
/// Describes means of loading/saving Silent Hunter files.
/// </summary>
public interface ISilentHunterFile
{
    /// <summary>
    /// Loads from specified <paramref name="stream" />.
    /// </summary>
    /// <param name="stream">The stream to load from.</param>
    /// <exception cref="IOException">Thrown when an IO error occurs.</exception>
    /// <exception cref="SilentHunterParserException">Thrown when a parsing error occurs.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="stream" /> is null.</exception>
    public Task LoadAsync(Stream stream);

    /// <summary>
    /// Saves to specified <paramref name="stream" />.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    /// <exception cref="IOException">Thrown when an IO error occurs.</exception>
    /// <exception cref="SilentHunterParserException">Thrown when a parsing error occurs.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="stream" /> is null.</exception>
    public Task SaveAsync(Stream stream);
}
