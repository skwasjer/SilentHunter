/* 
 * EnvSim.act - ShipWake
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the ShipWake controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using System.Collections.Generic;
using System.Numerics;
using SilentHunter;
using SilentHunter.Controllers;
using SilentHunter.Controllers.Decoration;

namespace SH3.EnvSim
{
	/// <summary>
	/// ShipWake render controller.
	/// </summary>
	public class ShipWake
		: Controller
	{
		/// <summary>
		/// If true the moving direction is the dummy Z axis, otherwise the moving direction is computed.
		/// </summary>
		public bool UseDummyForwardDirection;
		/// <summary>
		/// The time is takes for newly generated foam to become fully transparent and dissapear.
		/// </summary>
		public float TotalWakeTime;
		/// <summary>
		/// The total number of quads the ship wake is made of.
		/// </summary>
		public int MaxWakeQuads;
		/// <summary>
		/// The ship wake follows the ship while the generated vertices are above this distance on z axis from dummy.
		/// </summary>
		public float FollowShipDistance;
		/// <summary>
		/// V texture coordinate where ship wake vertices are out of the ship range.
		/// </summary>
		public float FollowShipVTextureCoordinate;
		/// <summary>
		/// The parameters used when generating a new quad.
		/// </summary>
		public ShipWakeStartParams StartParameters;
		/// <summary>
		/// VertexSpeed
		/// </summary>
		public List<ShipWakeSpeed> Speed;
		/// <summary>
		/// The texture used for boat wake shape.
		/// </summary>
		public string WakeTextureName;
		/// <summary>
		/// The distance in meters covered by the layer 1 foam texture.
		/// </summary>
		public float Layer1TextureSize;
		/// <summary>
		/// The distance in meters covered by the layer 2 foam texture.
		/// </summary>
		public float Layer2TextureSize;
		/// <summary>
		/// The distance in meters covered by the layer 3 foam texture.
		/// </summary>
		public float Layer3TextureSize;
		/// <summary>
		/// The distance in meters covered by the layer 4 foam texture.
		/// </summary>
		public float Layer4TextureSize;
		/// <summary>
		/// Wake height parameters.
		/// </summary>
		public ShipWakeHeight Height;
		/// <summary>
		/// The transparency of foam depending on ship speed.
		/// </summary>
		public ShipWakeTransparency GlobalTransparency;
		/// <summary>
		/// Texture keys.
		/// </summary>
		public List<string> LayerAnimation;
		/// <summary>
		/// Texture key.
		/// </summary>
		[Optional]
		public string TextureName;
		/// <summary>
		/// Number of texture keys per second for layer 1.
		/// </summary>
		public float Layer1Speed;
		/// <summary>
		/// Number of texture keys per second for layer 2.
		/// </summary>
		public float Layer2Speed;
		/// <summary>
		/// Number of texture keys per second for layer 3.
		/// </summary>
		public float Layer3Speed;
		/// <summary>
		/// Number of texture keys per second for layer 4.
		/// </summary>
		public float Layer4Speed;
		/// <summary>
		/// Show/hide layer (for debug).
		/// </summary>
		public bool DisplayLayer1;
		/// <summary>
		/// Show/hide layer (for debug).
		/// </summary>
		public bool DisplayLayer2;
		/// <summary>
		/// Show/hide layer (for debug).
		/// </summary>
		public bool DisplayLayer3;
		/// <summary>
		/// Show/hide layer (for debug).
		/// </summary>
		public bool DisplayLayer4;
	}

	[SHType]
	public class ShipWakeStartParams
		: Controller
	{
		/// <summary>
		/// The left vertex (viewed form above) params.
		/// </summary>
		public VertexStartPosition LeftVertex;
		/// <summary>
		/// The right vertex (viewed form above) params.
		/// </summary>
		public VertexStartPosition RightVertex;
	}

	[SHType]
	public class VertexStartPosition
		: Controller
	{
		/// <summary>
		/// Initial relative position.
		/// </summary>
		public Vector2XZ StartPosition;
	}

	[SHType]
	public class ShipWakeSpeed
		: Controller
	{
		public float ShipSpeed;
		public Vector2 Layer1TextureSpeed;
		public Vector2 Layer2TextureSpeed;
		public Vector2 Layer3TextureSpeed;
		public Vector2 Layer4TextureSpeed;
	}

	[SHType]
	public class ShipWakeHeight
		: Controller
	{
		/// <summary>
		/// If the object Y is under this minimum height the wake is not generated.
		/// </summary>
		public float MinHeight;
		/// <summary>
		/// If the object Y is above this maximum height the wake is not generated.
		/// </summary>
		public float MaxHeight;
		/// <summary>
		/// Wake transparency goes from 0 to 1 with this speed when the wake starts showing.
		/// </summary>
		public float FadeInSpeed;
		/// <summary>
		/// Wake transparency goes from 1 to 0 with this speed when the wake is disappearing.
		/// </summary>
		public float FadeOutSpeed;
	}

	[SHType]
	public class ShipWakeTransparency
		: Controller
	{
		/// <summary>
		/// Under this speed the foam is not visible (opacity is 0).
		/// </summary>
		public float MinOpacitySpeed;
		/// <summary>
		/// Over this speed the foam has full opacity.
		/// </summary>
		public float FullOpacitySpeed;
	}
}