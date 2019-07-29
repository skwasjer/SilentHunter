using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SilentHunter.Dat.Controllers
{
	public class ControllerAssembly
	{
		private readonly ControllerFactory _controllerFactory;

		public ControllerAssembly(Assembly controllerAssembly)
		{
			Assembly = controllerAssembly ?? throw new ArgumentNullException(nameof(controllerAssembly));

			_controllerFactory = new ControllerFactory(controllerAssembly);
			Reader = new ControllerReader(_controllerFactory);
			Writer = new ControllerWriter(_controllerFactory);

			string docFile = Path.Combine(Path.GetDirectoryName(controllerAssembly.Location), Path.GetFileNameWithoutExtension(controllerAssembly.Location) + ".xml");
			HelpText = new ControllerAssemblyHelpText(docFile);
		}

		public static ControllerAssembly Current { get; set; }

		public Assembly Assembly { get; }

		public IItemFactory ItemFactory => _controllerFactory;

		public IControllerFactory ControllerFactory => _controllerFactory;

		public IReadOnlyDictionary<ControllerProfile, Dictionary<string, Type>> Controllers => _controllerFactory.Controllers;

		public ControllerAssemblyHelpText HelpText { get; }

		public IControllerReader Reader { get; }

		public IControllerWriter Writer { get; }
	}
}
