using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SilentHunter.Formats;

namespace SilentHunter.Dat.Controllers
{
	public class ControllerFactory : IControllerFactory
	{
		private const BindingFlags ResolveBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Static;

		private readonly IItemFactory _itemFactory;
		private readonly Assembly _controllerAssembly;

		public ControllerFactory(IItemFactory itemFactory, Assembly controllerAssembly)
		{
			_itemFactory = itemFactory ?? throw new ArgumentNullException(nameof(itemFactory));
			_controllerAssembly = controllerAssembly ?? throw new ArgumentNullException(nameof(controllerAssembly));
			Controllers = EnumControllers(_controllerAssembly);
		}

		public IDictionary<ControllerProfile, Dictionary<string, Type>> Controllers { get; }

		public bool CanCreate(Type controllerType)
		{
			return Controllers
				.SelectMany(c => c.Value.Values)
				.Any(t => t == controllerType) && controllerType.IsController();
		}

		/// <summary>
		/// Creates the specified controller name for the requested profile.
		/// </summary>
		/// <param name="controllerName"></param>
		/// <param name="profile">The profile to search for the controller.</param>
		/// <returns>Returns the newly created controller. All (child) fields that are reference types are also pre-instantiated.</returns>
		/// <exception cref="ArgumentException">Thrown when the controller name is empty or cannot be found for the <paramref name="profile" />.</exception>
		public IRawController CreateController(string controllerName, ControllerProfile profile)
		{
			if (string.IsNullOrEmpty(controllerName))
			{
				throw new ArgumentNullException(nameof(controllerName));
			}

			if (Controllers[profile].ContainsKey(controllerName))
			{
				return CreateController(Controllers[profile][controllerName]);
			}

			throw new ArgumentException("Unknown controller type.");
		}

		/// <summary>
		/// Creates the controller for specified <paramref name="type" />.
		/// </summary>
		/// <param name="type">The controller type.</param>
		/// <returns>Returns the newly created controller. All (child) fields that are reference types are also instantiated using the default constructor.</returns>
		/// <exception cref="ArgumentException">Thrown when the controller name is empty or cannot be found for the <paramref name="profile" />.</exception>
		public IRawController CreateController(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			if (((ControllerProfile[])Enum.GetValues(typeof(ControllerProfile)))
				.Where(profile => profile != ControllerProfile.Unknown)
				.SelectMany(profile => Controllers[profile].Values)
				.Any(t => t == type))
			{
				return (IRawController)_itemFactory.CreateNewItem(type);
			}

			throw new ArgumentException("Unknown controller type.");
		}

		/// <summary>
		/// Loads controllers from the remote assembly. On order to qualify, the type must have ControllerAttribute applied.
		/// </summary>
		/// <param name="controllerAssembly"></param>
		private static IDictionary<ControllerProfile, Dictionary<string, Type>> EnumControllers(Assembly controllerAssembly)
		{
			var controllerTypes = new Dictionary<ControllerProfile, Dictionary<string, Type>>
			{
				{ ControllerProfile.SH5, new Dictionary<string, Type>() },
				{ ControllerProfile.SH4, new Dictionary<string, Type>() },
				{ ControllerProfile.SH3, new Dictionary<string, Type>() }
			};

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