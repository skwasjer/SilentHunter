/* 
 * SHControllers.act - Focusable3DObj
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the Focusable3DObj controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Dat;
using System.Collections.Generic;
using SilentHunter;

namespace SHControllers
{
	/// <summary>
	/// Focusable3DObj user data.
	/// </summary>
	public class Focusable3DObj
		: Controller
	{
		/// <summary>
		/// Focusable object tooltip.
		/// </summary>
		public int Tooltip;
		/// <summary>
		/// Command executed on object click (max 32 chars, incl. nullchar).
		/// </summary>
		[FixedString(32)]
		[AutoCompleteList(@"Cfg\Commands.txt")]
		public string Command;
		/// <summary>
		/// Command parameter.
		/// </summary>
		public int Param;
		/// <summary>
		/// Hide object.
		/// </summary>
		public bool Hide;
		/// <summary>
		/// Allow highlight on mouse over.
		/// </summary>
		[Optional]
		public bool? Highlightable;
		/// <summary>
		/// Allow object selection.
		/// </summary>
		public bool AllowSelection;
		/// <summary>
		/// Command will be sent to this object.
		/// </summary>
		public ulong Link;
		/// <summary>
		/// List of cameras. The Focusable3DObj becomes interactive only when one of these cameras is active.
		/// </summary>
		public List<CameraManager.Cameras> CamerasList;
	}
}