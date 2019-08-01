/* 
 * anim.act - CharacterInstantiation
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the CharacterInstantiation controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using SilentHunter.Controllers;

namespace anim
{
	/// <summary>
	/// CharacterInstantiation controller. Instantiate a character.
	/// </summary>
	public class CharacterInstantiation
		: Controller
	{
		/// <summary>
		/// Name of the character optionally followed by an animation list. E.g.: Character:anim1,anim2,anim3.
		/// </summary>
		public string CharacterName;
	}
}