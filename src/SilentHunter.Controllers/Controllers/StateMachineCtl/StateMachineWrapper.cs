/* 
 * StateMachineCtl.act - StateMachineWrapper
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the StateMachineWrapper controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Dat;

namespace StateMachineCtl
{
	/// <summary>
	/// StateMachineWrapper controller. State machine manager for characters or objects.
	/// </summary>
	public class StateMachineWrapper
		: Controller
	{
		/// <summary>
		/// Name of the graph used by the state machine.
		/// </summary>
		public string GraphName;
		/// <summary>
		/// The key code to activate the debug mode.
		/// </summary>
		/// <remarks>.NET equivalent: System.Windows.Forms.Keys</remarks>
		public System.Windows.Forms.Keys ShowDebug;
		/// <summary>
		/// Initially show debug.
		/// </summary>
		public bool DebugDefault;
	}
}