using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using SilentHunter.Controllers;
using SilentHunter.FileFormats.Extensions;

namespace SilentHunter.FileFormats.Dat.Controllers;

/// <summary>
/// Represents a wrapper for <see cref="Assembly" /> that gets all types that inherit from <see cref="Controller" />. Provides methods to get a specific controller type for a specific Silent Hunter game version.
/// </summary>
public class ControllerAssembly
{
    private const BindingFlags ResolveBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Static;

    /// <summary>
    /// Initializes a new instance of the <see cref="ControllerAssembly" /> using specified <paramref name="assembly" />.
    /// </summary>
    /// <param name="assembly">The assembly containing types that inherit from <see cref="Controller" />.</param>
    public ControllerAssembly(Assembly assembly)
    {
        Assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));

        Controllers = new ReadOnlyDictionary<ControllerProfile, Dictionary<string, Type>>(
            EnumControllers(Assembly)
        );

        LoadHelpText(assembly);
    }

    /// <summary>
    /// Gets the assembly.
    /// </summary>
    public Assembly Assembly { get; }

    /// <summary>
    /// Gets the controller types per SH-profile.
    /// </summary>
    public IReadOnlyDictionary<ControllerProfile, Dictionary<string, Type>> Controllers { get; }

    /// <summary>
    /// Gets the help text (documentation) helper.
    /// </summary>
    public ControllerAssemblyHelpText HelpText { get; private set; }

    /// <summary>
    /// Gets a controller type by <paramref name="controllerName" /> and <paramref name="profile" />.
    /// </summary>
    /// <param name="controllerName">The controller name.</param>
    /// <param name="profile">The SH-profile.</param>
    /// <param name="controllerType">Returns the controller type.</param>
    public bool TryGetControllerType(string controllerName, ControllerProfile profile, out Type controllerType)
    {
        if (Controllers.TryGetValue(profile, out Dictionary<string, Type> ctrls) && ctrls.TryGetValue(controllerName, out controllerType))
        {
            return true;
        }

        controllerType = null;
        return false;
    }

    /// <summary>
    /// Get the controller type for each profile that matches the <paramref name="controllerName" />.
    /// </summary>
    /// <param name="controllerName">The controller name.</param>
    /// <returns>All controller types that contain the controller by name.</returns>
    public IEnumerable<Type> GetControllerTypesByName(string controllerName)
    {
        IEnumerable<ControllerProfile> profiles = GetControllerProfilesByName(controllerName);
        return profiles
            .SelectMany(p => Controllers[p]
                .Where(c => c.Key == controllerName))
            .Select(c => c.Value);
    }

    /// <summary>
    /// Get each controller profile that contains the <paramref name="controllerName" />.
    /// </summary>
    /// <param name="controllerName">The controller name.</param>
    /// <returns>All profiles that contain the controller by name.</returns>
    public IEnumerable<ControllerProfile> GetControllerProfilesByName(string controllerName)
    {
        return Enum
            .GetValues(typeof(ControllerProfile))
            .Cast<ControllerProfile>()
            .Where(cp => cp != ControllerProfile.Unknown && TryGetControllerType(controllerName, cp, out _));
    }

    /// <summary>
    /// Loads controllers from the remote assembly. On order to qualify, the type must have ControllerAttribute applied.
    /// </summary>
    /// <param name="controllerAssembly"></param>
    private static IDictionary<ControllerProfile, Dictionary<string, Type>> EnumControllers(Assembly controllerAssembly)
    {
        IEnumerable<ControllerProfile> profilesWithoutUnknown = Enum.GetValues(typeof(ControllerProfile))
            .Cast<ControllerProfile>()
            .Where(p => p != ControllerProfile.Unknown);

        var controllerTypes = profilesWithoutUnknown.ToDictionary(cp => cp, cp => new Dictionary<string, Type>());

        foreach (TypeInfo typeInfo in controllerAssembly.DefinedTypes.Where(t => !t.IsEnum && !t.IsAbstract && !t.IsInterface))
        {
            AddOrUpdate(controllerTypes, typeInfo);
        }

        return controllerTypes;
    }

    private static void AddOrUpdate(Dictionary<ControllerProfile, Dictionary<string, Type>> controllerTypes, TypeInfo remoteType)
    {
        // No properties allowed.
        if (remoteType.GetProperties(ResolveBindingFlags).Any())
        {
            throw new NotSupportedException($"The type '{remoteType.Name}' defines one or more properties which is not supported.");
        }

        // Check if type is a controller.
        if (!remoteType.IsController())
        {
            return;
        }

        string controllerProfileStr = remoteType.FullName?.Substring(0, 3) ?? string.Empty;
        if (Enum.TryParse(controllerProfileStr, out ControllerProfile profile))
        {
            // Last wins.
            AddOrUpdate(controllerTypes, profile, remoteType);
        }
        else
        {
            // If not explicit, add to all.
            controllerTypes[ControllerProfile.SH3].Add(remoteType.Name, remoteType);
            controllerTypes[ControllerProfile.SH4].Add(remoteType.Name, remoteType);
            controllerTypes[ControllerProfile.SH5].Add(remoteType.Name, remoteType);
        }
    }

    private static void AddOrUpdate(IDictionary<ControllerProfile, Dictionary<string, Type>> controllerTypes, ControllerProfile profile, Type remoteType)
    {
        if (controllerTypes[profile].ContainsKey(remoteType.Name))
        {
            controllerTypes[profile][remoteType.Name] = remoteType;
        }
        else
        {
            controllerTypes[profile].Add(remoteType.Name, remoteType);
        }
    }

    private void LoadHelpText(Assembly controllerAssembly)
    {
        if (!File.Exists(controllerAssembly.Location))
        {
            return;
        }

        string assemblyDir = Path.GetDirectoryName(controllerAssembly.Location);
        if (assemblyDir == null || !Directory.Exists(assemblyDir))
        {
            return;
        }

        string docFile = Path.Combine(assemblyDir, Path.GetFileNameWithoutExtension(controllerAssembly.Location) + ".xml");
        if (File.Exists(docFile))
        {
            HelpText = new ControllerAssemblyHelpText(docFile);
        }
    }
}
