using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using SilentHunter.Formats;

namespace SilentHunter.Dat.Controllers
{
	public class ControllerAssembly
	{
		private const BindingFlags ResolveBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Static;

		private readonly ControllerFactory _controllerFactory;

		public ControllerAssembly(Assembly controllerAssembly)
		{
			Assembly = controllerAssembly ?? throw new ArgumentNullException(nameof(controllerAssembly));

			Controllers = new ReadOnlyDictionary<ControllerProfile, Dictionary<string, Type>>(
				EnumControllers(Assembly)
			);

			_controllerFactory = new ControllerFactory(this);
			Reader = new ControllerReader(this, _controllerFactory);
			Writer = new ControllerWriter(_controllerFactory);

			string docFile = Path.Combine(Path.GetDirectoryName(controllerAssembly.Location), Path.GetFileNameWithoutExtension(controllerAssembly.Location) + ".xml");
			HelpText = new ControllerAssemblyHelpText(docFile);
		}

		public static ControllerAssembly Current { get; set; }

		public Assembly Assembly { get; }

		public IItemFactory ItemFactory => _controllerFactory;

		public IControllerFactory ControllerFactory => _controllerFactory;

		public IReadOnlyDictionary<ControllerProfile, Dictionary<string, Type>> Controllers { get; }

		public ControllerAssemblyHelpText HelpText { get; }

		public IControllerReader Reader { get; }

		public IControllerWriter Writer { get; }

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
		/// Get the controller type for the first profile that implements the controller.
		/// </summary>
		/// <param name="controllerName">The controller name.</param>
		/// <returns>The first found profile.</returns>
		public IEnumerable<Type> GetControllerTypesByName(string controllerName)
		{
			IEnumerable<ControllerProfile> profiles = GetControllerProfilesByName(controllerName);
			return profiles
				.SelectMany(p => Controllers[p]
					.Where(c => c.Key == controllerName))
				.Select(c => c.Value);
		}

		/// <summary>
		/// Get the first profile that implements the controller.
		/// </summary>
		/// <param name="controllerName">The controller name.</param>
		/// <returns>The first found profile.</returns>
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
			var controllerTypes = new Dictionary<ControllerProfile, Dictionary<string, Type>>();
			var profilesWithoutUnknown = Enum.GetValues(typeof(ControllerProfile))
				.Cast<ControllerProfile>()
				.Where(p => p != ControllerProfile.Unknown);

			foreach (ControllerProfile cp in profilesWithoutUnknown)
			{
				controllerTypes.Add(cp, new Dictionary<string, Type>());
			}

			void NotSupportedMember(IList list, Type type, string memberName)
			{
				if (list.Count > 0)
				{
					throw new NotSupportedException($"The type '{type.Name}' defines one or more {memberName} which is not supported");
				}
			}

			foreach (Type remoteType in controllerAssembly.GetTypes())
			{
				// No interfaces allowed.
				if (remoteType.IsInterface)
				{
					throw new NotSupportedException($"The type '{remoteType.Name}' is an interface which is not supported.");
				}

				// No properties allowed.
				NotSupportedMember(remoteType.GetProperties(ResolveBindingFlags), remoteType, "properties");

				if (remoteType.IsValueType && !remoteType.IsDefined(typeof(SerializableAttribute), false))
				{
					throw new NotSupportedException(
						$"The type '{remoteType.Name}' is a value type, which requires a SerializableAttribute."
					);
				}

				// Check if type is a controller.
				if (!remoteType.IsController())
				{
					continue;
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

			return controllerTypes;
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
	}
}
